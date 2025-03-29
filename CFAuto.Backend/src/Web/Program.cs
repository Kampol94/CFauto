using System.Text.Json;
using System.Text.Json.Serialization;
using Contracts;
using MassTransit;
using Web.Endpoints;
using static Web.Endpoints.ReservetionEndpoints;

var builder = WebApplication.CreateSlimBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

// Create JsonSerializerOptions with the custom context
var jsonSerializerOptions = new JsonSerializerOptions
{
    TypeInfoResolverChain = { AppJsonSerializerContext.Default }
};

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("rabbitmq")!);
        cfg.ConfigureEndpoints(context);
        cfg.ConfigureJsonSerializerOptions(opts =>
        {
            opts.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
            return opts;
        });
    });
});

// Add RequestClient for CheckReservationStatus
builder.Services.AddScoped(sp =>
{
    var bus = sp.GetRequiredService<IBus>();
    return bus.CreateRequestClient<CheckReservationStatus>();
});

var app = builder.Build();

app.MapReservetionEndpoints();

app.MapOpenApi();

app.MapDefaultEndpoints();
app.Run();

[JsonSerializable(typeof(CreateReservationJobRequest))]
[JsonSerializable(typeof(CreateReservationJobResponse))]
[JsonSerializable(typeof(GetReservationStatus))]
[JsonSerializable(typeof(ReservetionEndpoints.ReservationStatusResponse))]
[JsonSerializable(typeof(Contracts.CheckReservationStatus))]
[JsonSerializable(typeof(Contracts.NotFound))]
[JsonSerializable(typeof(Contracts.ReservationStatus))]
[JsonSerializable(typeof(Response<ReservetionEndpoints.ReservationStatusResponse>))]
[JsonSerializable(typeof(List<string>))] 
[JsonSerializable(typeof(CreateReservationJob))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}

public partial class Program { }
