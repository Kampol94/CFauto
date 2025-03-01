namespace Contracts;

public record class CreateReservationJob
{
    public string TrainingId { get; init; } = default!;

    public string MemberId { get; init; } = default!;

    public string UserToken { get; init; } = default!;
}
