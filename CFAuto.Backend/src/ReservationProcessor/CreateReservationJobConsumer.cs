using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Contracts;
using MassTransit;

namespace ReservationProcessor;

public class CreateReservationJobConsumer(IPublishEndpoint publishEndpoint) : IConsumer<CreateReservationJob>
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task Consume(ConsumeContext<CreateReservationJob> context)
    {
        var message = context.Message;
        Console.WriteLine($"Received message: {JsonSerializer.Serialize(message)}");
        await Task.Delay(20000);
        await _publishEndpoint.Publish(new ReservationProccessing
        {
            ReservationId = message.ReservationId
        });
        
        Console.WriteLine($"Proccessing message: {JsonSerializer.Serialize(message)}");
        await Task.Delay(20000);

        await _publishEndpoint.Publish(new ReservationCompleted
        {
            ReservationId = message.ReservationId
        });
        Console.WriteLine($"ReservationCompleted message: {JsonSerializer.Serialize(message)}");
        await Task.CompletedTask;
    }
}