namespace Hair.Application.Common.Dto.Company;

public record HaircutResponseDto(Guid HaircutId, string HaircutType, decimal Price,int Duration,Guid CompanyId);