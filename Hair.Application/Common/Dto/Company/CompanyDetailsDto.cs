namespace Hair.Application.Common.Dto.Company;

public record CompanyDetailsDto
{
    public CompanyDetailsDto(Guid CompanyId, string CompanyName,string? ImageUrl)
    {
        this.CompanyId = CompanyId;
        this.CompanyName = CompanyName;
        this.ImageUrl = ImageUrl;
    }
    public CompanyDetailsDto(){}

    public Guid CompanyId { get; init; }
    public string CompanyName { get; init; }
    public string? ImageUrl { get; init; }

    public void Deconstruct(out Guid CompanyId, out string CompanyName)
    {
        CompanyId = this.CompanyId;
        CompanyName = this.CompanyName;
    }
}