using Marten;
using Marten.Events.Projections;
using MassTransit;
using ReservationSummery;
using ReservationSummery.Consumers;
using Weasel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMarten(options =>
{
    // Establish the connection string to your Marten database
    options.Connection(builder.Configuration.GetConnectionString("postgresdb")!);

    // Specify that we want to use STJ as our serializer
    options.UseSystemTextJsonForSerialization();

    // If we're running in development mode, let Marten just take care
    // of all necessary schema building and patching behind the scenes

    options.AutoCreateSchemaObjects = AutoCreate.All;

    options.Projections.LiveStreamAggregation<Reservation>();
    
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CreateReservationJobConsumer>();
    x.AddConsumer<CheckReservationStatusConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("rabbitmq")!);
        
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
