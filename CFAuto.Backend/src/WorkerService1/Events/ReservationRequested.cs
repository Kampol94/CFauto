using System;

namespace WorkerService1.Events
{
    public class ReservationRequested
    {
        public Guid ReservationId { get; set; }
    }

    public class ReservationCompleted
    {
        public Guid ReservationId { get; set; }
    }
}