using MassTransit;

namespace WorkerService1;

public class MassTransitWorker : BackgroundService
{
    private readonly IBusControl _bus;
    public MassTransitWorker(IBusControl bus) => _bus = bus;

    protected override async Task ExecuteAsync(System.Threading.CancellationToken stoppingToken)
    {
        await _bus.StartAsync(stoppingToken);
    }

    public override async Task StopAsync(System.Threading.CancellationToken stoppingToken)
    {
        await _bus.StopAsync(stoppingToken);
    }
}
