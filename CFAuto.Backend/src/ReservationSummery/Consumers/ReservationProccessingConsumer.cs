using System;
using Contracts;
using Marten;
using MassTransit;

namespace ReservationSummery.Consumers;

public class ReservationProccessingConsumer(IDocumentSession documentSession) : IConsumer<ReservationProccessing>
{

    public Task Consume(ConsumeContext<ReservationProccessing> context)
    {
        documentSession.Events.WriteToAggregate<Reservation>(context.Message.ReservationId, stream => stream.AppendOne(context.Message));
        return Task.CompletedTask;
    }
}

public class ReservationCompletedConsumer(IDocumentSession documentSession) : IConsumer<ReservationCompleted>
{
    public Task Consume(ConsumeContext<ReservationCompleted> context)
    {
        documentSession.Events.WriteToAggregate<Reservation>(context.Message.ReservationId, stream => stream.AppendOne(context.Message));
        return Task.CompletedTask;
    }
}

public class CreateReservationJobConsumer(IDocumentSession documentSession) : IConsumer<CreateReservationJob>
{
    public Task Consume(ConsumeContext<CreateReservationJob> context)
    {
        documentSession.Events.WriteToAggregate<Reservation>(context.Message.ReservationId, stream => stream.AppendOne(context.Message));
        return Task.CompletedTask;
    }
}

