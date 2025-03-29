
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("redis")
                    .WithRedisCommander();

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
                      .WithLifetime(ContainerLifetime.Persistent)
                      .WithManagementPlugin();

var postgres = builder.AddPostgres("postgres")
                      .WithPgAdmin();

var postgresdb = postgres.AddDatabase("postgresdb");

var reservationProccessor = builder.AddProject<Projects.ReservationProcessor>("reservationprocessor")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

var reservationSummery = builder.AddProject<Projects.ReservationSummery>("reservationsummery")
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);


builder.AddProject<Projects.Web>("web")
    .WithExternalHttpEndpoints()
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

// builder.AddProject<Projects.CFAuto_Backend_Web>("webfrontend")
//     .WithExternalHttpEndpoints()
//     .WithReference(cache)
//     .WaitFor(cache)
//     .WithReference(apiService)
//     .WaitFor(apiService);

builder.Build().Run();
