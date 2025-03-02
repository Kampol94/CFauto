namespace Contracts;

public record class CreateReservationJob
{
    public Guid ReservationId { get; init; } = default!;
    public string TrainingId { get; init; } = default!;

    public string MemberId { get; init; } = default!;

    public string UserToken { get; init; } = default!;
}
