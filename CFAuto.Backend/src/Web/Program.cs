using System.Text.Json;
using System.Text.Json.Serialization;
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

    x.AddRequestClient<GetReservationStatus>();
});

var app = builder.Build();

app.MapReservetionEndpoints();

app.MapOpenApi();

app.MapDefaultEndpoints();
app.Run();

[JsonSerializable(typeof(CreateReservationJobRequest))]
[JsonSerializable(typeof(CreateReservationJobResponse))]
[JsonSerializable(typeof(GetReservationStatus))]
[JsonSerializable(typeof(ReservationStatus))]
[JsonSerializable(typeof(Response<ReservationStatus>))]
[JsonSerializable(typeof(List<string>))] 
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
