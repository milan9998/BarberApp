using Microsoft.AspNetCore.Identity;

namespace Hair.Domain.Entities;

public class AnonymousUser
{
    public AnonymousUser(string? firstName, string? lastName, string? email, string phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Id = Guid.NewGuid();
    }

    public Guid Id { get; private set; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? Email { get; private set; }
    public string PhoneNumber { get; private set; }

    public AnonymousUser()
    {
    }
}