using MassTransit;
using System;

namespace WorkerService1;
public class ReservationState : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public string? CurrentState { get; set; }
    public Guid ReservationId { get; set; }
    public DateTime Timestamp { get; set; }
    public int Version { get; set; }
}