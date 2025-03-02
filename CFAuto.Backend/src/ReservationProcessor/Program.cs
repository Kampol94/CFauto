using MassTransit;
using ReservationProcessor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CreateReservationJobConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("rabbitmq")!);
        cfg.ConfigureEndpoints(context);
    });
});

builder.ConfigureOpenTelemetry();
//builder.Services.AddHostedService<MassTransitWorker>();

var app = builder.Build();
app.Run();
