using Contracts;
using MassTransit;
using System;
using WorkerService1.Events;
namespace WorkerService1;
public class ReservationStateMachine : MassTransitStateMachine<ReservationState>
{
    public State Unknown { get; private set; }
    public State Reserved { get; private set; }
    public State Completed { get; private set; }

    public Event<ReservationRequested> ReservationRequested { get; private set; }
    public Event<ReservationCompleted> ReservationCompleted { get; private set; }
    public Event<CheckReservationStatus> CheckOrderStatusRequested { get; private set; }

    public ReservationStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => ReservationRequested, x =>
        {
            x.CorrelateById(context => context.Message.ReservationId);
        });
        Event(() => ReservationCompleted, x => x.CorrelateById(context => context.Message.ReservationId));
        Event(() => CheckOrderStatusRequested, x => 
        {
            x.CorrelateById(context => context.Message.ReservationId);
            x.OnMissingInstance(m => m.ExecuteAsync(async context => await context.RespondAsync(new ReservationStatus
            {
                CurrentState = "Unknown"
            })));
        });

        Initially(
            When(ReservationRequested)
                .Then(context => 
                {
                    context.Saga.ReservationId = context.Message.ReservationId;
                    context.Saga.Timestamp = DateTime.UtcNow;
                })
                .TransitionTo(Reserved)
        );

        During(Reserved,
            When(ReservationCompleted)
                .Then(context => context.Saga.Timestamp = DateTime.UtcNow)
                .TransitionTo(Completed)
        );

        SetCompletedWhenFinalized();
    }
}