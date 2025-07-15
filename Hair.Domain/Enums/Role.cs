namespace Hair.Domain.Enums;

[Flags]
public enum Role
{
    RegisteredUser = 0,
    Barber = 1,
    CompanyOwner = 2,
    Admin = 3

}