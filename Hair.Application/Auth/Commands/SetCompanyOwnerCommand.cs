using Hair.Application.Common.Interfaces;
using Hair.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hair.Application.Auth.Commands;

public record SetCompanyOwnerCommand(string OwnerId, Guid CompanyId) : IRequest<string>;

public class SetCompanyOwnerCommandHandler(IHairDbContext dbContext, UserManager<ApplicationUser> userManager) : IRequestHandler<SetCompanyOwnerCommand, string>
{
    public async Task<string> Handle(SetCompanyOwnerCommand request, CancellationToken cancellationToken)
    {
        var company = await dbContext.Companies.Where(c => c.Id == request.CompanyId).FirstOrDefaultAsync();
        
        //company.SetCompanyOwnerId(request.OwnerId);
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return "Success";
        
        
    }
}