using MassTransit;
using WorkerService1;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CreateReservationJobConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("rabbitmq")!);
        
    
        cfg.ConfigureEndpoints(context);
    });
    x.AddSagaStateMachine<ReservationStateMachine, ReservationState>()
        .RedisRepository(r =>
        {
            r.DatabaseConfiguration(builder.Configuration.GetConnectionString("redis"));
        });
});
builder.ConfigureOpenTelemetry();
//builder.Services.AddHostedService<MassTransitWorker>();

var host = builder.Build();
host.Run();
