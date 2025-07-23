using Hair.Application.Common.Dto.Auth;
using Hair.Domain.Entities;
using Hair.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hair.Application.Auth.Queries;

public record GetAllOwnersQuery():IRequest<List<CompanyOwnerDetails>>;


public class GetAllOwnersQueryHandler(UserManager<ApplicationUser> userManager) 
    : IRequestHandler<GetAllOwnersQuery, List<CompanyOwnerDetails>>
{
    public async Task<List<CompanyOwnerDetails>> Handle(GetAllOwnersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var owners = await userManager.Users
                .Where(u => u.Role == Role.CompanyOwner)
                .ToListAsync();
            var response = owners.Select(x => new CompanyOwnerDetails(
                OwnerId: x.Id,
                Email: x.Email,
                //CompanyId: x.CompanyId,
                FirstName: x.FirstName,
                LastName: x.LastName,
                PhoneNumber: x.PhoneNumber
               
            )).ToList();
            return response;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}