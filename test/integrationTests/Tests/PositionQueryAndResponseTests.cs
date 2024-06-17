namespace Microsoft.Azure.SpaceFx.HostServices.Position.IntegrationTests.Tests;

[Collection(nameof(TestSharedContext))]
public class PositionRequestTests : IClassFixture<TestSharedContext> {
    readonly TestSharedContext _context;

    public PositionRequestTests(TestSharedContext context) {
        _context = context;
    }

    [Fact]
    public async Task PositionUpdateAndQuery() {
        string trackingId = Guid.NewGuid().ToString();
        DateTime maxTimeToWait = DateTime.Now.Add(TestSharedContext.MAX_TIMESPAN_TO_WAIT_FOR_MSG);
        MessageFormats.HostServices.Position.PositionResponse? response = null;
        MessageFormats.HostServices.Position.PositionUpdateResponse? update_response = null;

        // Register a callback event to catch the response
        void PositionUpdateResponseEventHandler(object? _, MessageFormats.HostServices.Position.PositionUpdateResponse _response) {
            update_response = _response;
            MessageHandler<MessageFormats.HostServices.Position.PositionUpdateResponse>.MessageReceivedEvent -= PositionUpdateResponseEventHandler;
        }

        void PositionResponseEventHandler(object? _, MessageFormats.HostServices.Position.PositionResponse _response) {
            response = _response;
            MessageHandler<MessageFormats.HostServices.Position.PositionResponse>.MessageReceivedEvent -= PositionResponseEventHandler;
        }

        MessageHandler<MessageFormats.HostServices.Position.PositionResponse>.MessageReceivedEvent += PositionResponseEventHandler;
        MessageHandler<MessageFormats.HostServices.Position.PositionUpdateResponse>.MessageReceivedEvent += PositionUpdateResponseEventHandler;


        MessageFormats.HostServices.Position.PositionUpdateRequest testMessage = new() {
            RequestHeader = new() {
                TrackingId = trackingId,
                CorrelationId = trackingId
            },
            Position = new Microsoft.Azure.SpaceFx.MessageFormats.HostServices.Position.Position() {
                PositionTime = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow),
                Point = new Microsoft.Azure.SpaceFx.MessageFormats.HostServices.Position.Position.Types.Point() {
                    X = 1,
                    Y = 2,
                    Z = 3,
                },
                Attitude = new Microsoft.Azure.SpaceFx.MessageFormats.HostServices.Position.Position.Types.Attitude() {
                    X = 1,
                    Y = 2,
                    Z = 3,
                    K = 4
                }
            }
        };


        Console.WriteLine($"Sending '{testMessage.GetType().Name}' (TrackingId: '{testMessage.RequestHeader.TrackingId}')");
        await TestSharedContext.SPACEFX_CLIENT.DirectToApp(TestSharedContext.TARGET_SVC_APP_ID, testMessage);



        Console.WriteLine($"Waiting for response to '{testMessage.GetType().Name}' (TrackingId: '{testMessage.RequestHeader.TrackingId}')...");
        while (update_response == null && DateTime.Now <= maxTimeToWait) {
            Thread.Sleep(100);
        }

        if (update_response == null) throw new TimeoutException($"Failed to hear {nameof(update_response)} heartbeat after {TestSharedContext.MAX_TIMESPAN_TO_WAIT_FOR_MSG}.  Please check that {TestSharedContext.TARGET_SVC_APP_ID} is deployed");

        Assert.NotNull(update_response);
        Assert.Equal(MessageFormats.Common.StatusCodes.Successful, update_response.ResponseHeader.Status);

        Console.WriteLine($"Position Update successful. (TrackingId: '{testMessage.RequestHeader.TrackingId}')...");

        var request = new MessageFormats.HostServices.Position.PositionRequest() {
            RequestHeader = new() {
                TrackingId = trackingId,
                CorrelationId = trackingId
            }
        };

        Console.WriteLine($"Sending '{request.GetType().Name}' (TrackingId: '{request.RequestHeader.TrackingId}')");
        await TestSharedContext.SPACEFX_CLIENT.DirectToApp(TestSharedContext.TARGET_SVC_APP_ID, request);


        Console.WriteLine($"Waiting for response to '{request.GetType().Name}' (TrackingId: '{request.RequestHeader.TrackingId}')...");
        while (response == null && DateTime.Now <= maxTimeToWait) {
            Thread.Sleep(100);
        }

        if (response == null) throw new TimeoutException($"Failed to hear {nameof(response)} after {TestSharedContext.MAX_TIMESPAN_TO_WAIT_FOR_MSG}.  Please check that {TestSharedContext.TARGET_SVC_APP_ID} is deployed");

        Assert.NotNull(response);

    }
}