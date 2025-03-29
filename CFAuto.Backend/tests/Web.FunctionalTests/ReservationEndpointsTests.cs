using System.Net;
using System.Net.Http.Json;
using Contracts;
using MassTransit;
using Xunit;

namespace Web.FunctionalTests;

public class ReservationEndpointsTests(WebAPIFixture fixture) : IClassFixture<WebAPIFixture>
{
    private readonly WebAPIFixture _fixture = fixture;
    private readonly HttpClient _client = fixture.CreateClient();

    [Fact]
    public async Task CreateReservationJob_ShouldPublishMessage()
    {
        // Arrange
        var request = new
        {
            trainingId = "test-training-1",
            memberId = "test-member-1",
            userToken = "test-token"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/createReservationJob", request);

        // Assert
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        var reservationId = await response.Content.ReadAsStringAsync();
        Assert.True(Guid.TryParse(reservationId, out _));
    }

    [Fact]
    public async Task GetReservationStatus_WhenReservationExists_ShouldReturnStatus()
    {
        // Arrange
        var reservationId = Guid.NewGuid();
        
        // Act
        var response = await _client.GetAsync($"/api/reservationStatus/{reservationId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ReservationStatus>();
        Assert.NotNull(result);
        Assert.Equal("Test", result.CurrentState);
    }

    [Fact]
    public async Task GetReservationStatus_WhenReservationNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/reservationStatus/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetReservationStatus_ShouldSendMessageToRabbitMQ()
    {
        // Arrange
        var reservationId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/reservationStatus/{reservationId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        
        // Verify the message was sent to RabbitMQ
        var messages = _fixture.StatusCheckMessages;
        Assert.Single(messages);
        var message = messages.First();
        Assert.Equal(reservationId, message.ReservationId);
    }
}
