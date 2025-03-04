namespace Hair.Application.Common.Dto.Schedule;

public record GetAllSchedulesByBarberIdDto
{
    public GetAllSchedulesByBarberIdDto(Guid BarberId,DateTime Time)
    {
        barberId = BarberId;
        time = Time;
    }

    public GetAllSchedulesByBarberIdDto()
    {
        
    }

    public Guid barberId { get; init; }
    public DateTime time { get; init; }

    public void Deconstruct(out Guid barberId, out DateTime time)
    {
        barberId = this.barberId;
        time = this.time;
    }
}