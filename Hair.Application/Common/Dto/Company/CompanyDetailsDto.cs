namespace Hair.Application.Common.Dto.Company;

public record CompanyDetailsDto
{
    public CompanyDetailsDto(Guid CompanyId, string CompanyName)
    {
        this.CompanyId = CompanyId;
        this.CompanyName = CompanyName;
    }
    public CompanyDetailsDto(){}

    public Guid CompanyId { get; init; }
    public string CompanyName { get; init; }

    public void Deconstruct(out Guid CompanyId, out string CompanyName)
    {
        CompanyId = this.CompanyId;
        CompanyName = this.CompanyName;
    }
}