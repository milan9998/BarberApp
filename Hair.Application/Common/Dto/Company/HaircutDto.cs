namespace Hair.Application.Common.Dto.Company;

public record HaircutDto(Guid HaircutId, string HaircutType, decimal Price,int Duration,Guid CompanyId);