using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Testcontainers.RabbitMq;
using Xunit;

namespace Web.FunctionalTests;

public class WebAPIFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly RabbitMqContainer _rabbitMqContainer;
    private string? RabbitMqConnectionString { get; set; }
    public ConcurrentBag<CheckReservationStatus> StatusCheckMessages { get; } = new();

    public WebAPIFixture()
    {
        _rabbitMqContainer = new RabbitMqBuilder()
            .WithName("test-rabbitmq")
            .WithPortBinding(5672, true)
            .WithPortBinding(15672, true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _rabbitMqContainer.StartAsync();
        RabbitMqConnectionString = _rabbitMqContainer.GetConnectionString();
    }

    public async new Task DisposeAsync()
    {
        await _rabbitMqContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:rabbitmq"] = RabbitMqConnectionString
            });
        });

        builder.ConfigureServices(services =>
        {
            services.AddSingleton(StatusCheckMessages);
            services.AddMassTransitTestHarness(x => x.AddConsumer<TestStatusCheckConsumer>());
        });
    }
}

public class TestStatusCheckConsumer : IConsumer<CheckReservationStatus>
{
    private readonly ConcurrentBag<CheckReservationStatus> _messages;

    public TestStatusCheckConsumer(ConcurrentBag<CheckReservationStatus> messages)
    {
        _messages = messages;
    }

    public Task Consume(ConsumeContext<CheckReservationStatus> context)
    {
        _messages.Add(context.Message);
        return context.RespondAsync(new ReservationStatus { CurrentState = "Test" });
    }
}
