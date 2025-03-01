
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("redis");

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
                      .WithLifetime(ContainerLifetime.Persistent)
                      .WithManagementPlugin();

var worker = builder.AddProject<Projects.WorkerService1>("worker")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithReference(cache)
    .WaitFor(cache);

builder.AddProject<Projects.Web>("web")
    .WithExternalHttpEndpoints()
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithReference(worker);

// builder.AddProject<Projects.CFAuto_Backend_Web>("webfrontend")
//     .WithExternalHttpEndpoints()
//     .WithReference(cache)
//     .WaitFor(cache)
//     .WithReference(apiService)
//     .WaitFor(apiService);

builder.Build().Run();
