using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Contracts;
using MassTransit;

namespace WorkerService1;

public class CreateReservationJobConsumer : IConsumer<CreateReservationJob>
{
    public async Task Consume(ConsumeContext<CreateReservationJob> context)
    {
        var message = context.Message;
        Console.WriteLine($"Received message: {JsonSerializer.Serialize(message)}");
        await Task.CompletedTask;
    }
}