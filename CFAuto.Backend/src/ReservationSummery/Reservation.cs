using System;
using Contracts;
using Marten.Schema;

namespace ReservationSummery;

public class Reservation(CreateReservationJob createReservationJob)
{
    [Identity]
    public Guid ReservationId { get; set; } = createReservationJob.ReservationId;
    public ReservationStatus1 Status { get; set; } = ReservationStatus1.AwaitingConfirmation;

    public void Apllly(ReservationProccessing reservationProccessing)
    {
        Status = ReservationStatus1.Pending;
    }

    public void Apllly(ReservationCompleted _)
    {
        Status = ReservationStatus1.Confirmed;
    }
}

public enum ReservationStatus1
{
    AwaitingConfirmation,
    Pending,
    Confirmed,
    Cancelled
}