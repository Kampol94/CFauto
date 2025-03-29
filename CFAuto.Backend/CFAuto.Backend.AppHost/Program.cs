
var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
                      .WithLifetime(ContainerLifetime.Persistent)
                      .WithManagementPlugin();

var postgres = builder.AddPostgres("postgres")
                      .WithPgAdmin();

var postgresdb = postgres.AddDatabase("postgresdb");

builder.AddProject<Projects.ReservationProcessor>("reservationprocessor")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder.AddProject<Projects.ReservationSummery>("reservationsummery")
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);


builder.AddProject<Projects.Web>("web")
    .WithExternalHttpEndpoints()
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder.Build().Run();
