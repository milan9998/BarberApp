using Hair.Application.Common.Dto.Auth;
using Hair.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hair.Application.Auth.Queries;

public record GetOwnerDetailsByIdQuery(string OwnerId) : IRequest<CompanyOwnerDetails>;

public class GetOwnerDetailsByIdQueryHandler(UserManager<ApplicationUser> userManager) 
    : IRequestHandler<GetOwnerDetailsByIdQuery, CompanyOwnerDetails>
{
    public async Task<CompanyOwnerDetails> Handle(GetOwnerDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        var owner = await userManager.Users.Where(u => u.Id == request.OwnerId)
            .FirstOrDefaultAsync(cancellationToken);

        var response = new CompanyOwnerDetails(
            OwnerId: owner.Id,
            Email: owner.Email,
            FirstName: owner.FirstName,
            LastName: owner.LastName,
            PhoneNumber: owner.PhoneNumber
        );
        
        return response;
    }
}