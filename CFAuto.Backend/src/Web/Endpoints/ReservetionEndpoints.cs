using System;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Contracts;

namespace Web.Endpoints;

public static class ReservetionEndpoints
{
    public static void MapReservetionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/createReservationJob", CreateReservationAsync);
        endpoints.MapGet("/api/reservationStatus/{reservationId}", GetReservationStatusAsync);
    }

    private static async Task<IResult> CreateReservationAsync(IPublishEndpoint publishEndpoint, CreateReservationJobRequest request)
    {
        await publishEndpoint.Publish<CreateReservationJob>(new
        {
            request.TrainingId,
            request.MemberId,
            request.UserToken
        });
        return Results.Accepted();
    }

    private static async Task<IResult> GetReservationStatusAsync(Guid reservationId, IRequestClient<GetReservationStatus> client)
    {
        var response = await client.GetResponse<ReservationStatus, string>(new { ReservationId = reservationId });

        return response switch
        {
            (Response<ReservationStatus> reservationStatusResponse, _) => HandleSuccessResponse(reservationStatusResponse),
            (_, Response<string> errorResponse) => HandleErrorResponse(errorResponse),
            _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    private static IResult HandleSuccessResponse(Response<ReservationStatus> reservationStatusResponse)
    {
        return Results.Ok(new
        {
            reservationStatusResponse.Message.CurrentState
        });
    }

    private static IResult HandleErrorResponse(Response<string> errorResponse)
    {
        return Results.BadRequest(new { Error = errorResponse.Message });
    }

    public class CreateReservationJobRequest
    {
        [JsonPropertyName("trainingId")]
        public string TrainingId { get; set; } = default!;

        [JsonPropertyName("memberId")]
        public string MemberId { get; set; } = default!;

        [JsonPropertyName("userToken")]
        public string UserToken { get; set; } = default!;
    }

    public class CreateReservationJobResponse
    {
        [JsonPropertyName("jobId")]
        public string JobId { get; set; } = default!;
    }

    public class GetReservationStatus
    {
        [JsonPropertyName("reservationId")]
        public Guid ReservationId { get; set; }
    }

    public class ReservationStatus
    {
        [JsonPropertyName("currentState")]
        public string CurrentState { get; set; } = default!;
    }
}
