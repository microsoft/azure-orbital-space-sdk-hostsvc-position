using Microsoft.Azure.SpaceFx.MessageFormats.HostServices.Position;

namespace Microsoft.Azure.SpaceFx.HostServices.Position;

public partial class MessageHandler<T> {
    private void PositionRequestHandler(MessageFormats.HostServices.Position.PositionRequest? message, MessageFormats.Common.DirectToApp fullMessage) {
        // Check if the message is null. If it is, exit the method.
        if (message == null) return;

        // Create a new scope for the service provider.
        using (var scope = _serviceProvider.CreateScope()) {
            // Initialize a new PositionResponse object.
            MessageFormats.HostServices.Position.PositionResponse returnResponse = new() { };

            // Log the information about the processing message.
            _logger.LogInformation("Processing message type '{messageType}' from '{sourceApp}' (trackingId: '{trackingId}' / correlationId: '{correlationId}')", message.GetType().Name, fullMessage.SourceAppId, message.RequestHeader.TrackingId, message.RequestHeader.CorrelationId);

            // Create a new response with the tracking and correlation IDs from the request, and a successful status.
            returnResponse = new() {
                ResponseHeader = new() {
                    TrackingId = message.RequestHeader.TrackingId,
                    CorrelationId = message.RequestHeader.CorrelationId,
                    Status = MessageFormats.Common.StatusCodes.Successful
                }
            };

            // Retrieve the last known position from the cache.
            MessageFormats.HostServices.Position.Position? lastKnownPosition = _client.GetCacheItem<MessageFormats.HostServices.Position.Position>(cacheItemName: CACHE_KEYS.LAST_KNOWN_POSITION).Result;

            // Check if the last known position is null. If it is, update the response header status and message.
            if (lastKnownPosition == null) {
                returnResponse.ResponseHeader.Status = MessageFormats.Common.StatusCodes.NotFound;
                returnResponse.ResponseHeader.Message = "Current position is unknown - no update has been received.  Please try again later.";
            } else {
                // If the last known position is not null, update the return response position.
                returnResponse.Position = lastKnownPosition;
            }

            // Log the debug information about the message and response types being passed to plugins.
            _logger.LogDebug("Passing message '{messageType}' and '{responseType}' to plugins (trackingId: '{trackingId}' / correlationId: '{correlationId}')", message.GetType().Name, returnResponse.GetType().Name, message.RequestHeader.TrackingId, message.RequestHeader.CorrelationId);

            // Call the plugins with the original request and response.
            (MessageFormats.HostServices.Position.PositionRequest? output_request, MessageFormats.HostServices.Position.PositionResponse? output_response) =
                                                _pluginLoader.CallPlugins<MessageFormats.HostServices.Position.PositionRequest?, Plugins.PluginBase, MessageFormats.HostServices.Position.PositionResponse>(
                                                    orig_request: message, orig_response: returnResponse,
                                                    pluginDelegate: _pluginDelegates.PositionRequest);

            // Log the debug information about the plugins finishing processing the message and response.
            _logger.LogDebug("Plugins finished processing '{messageType}' and '{responseType}' (trackingId: '{trackingId}' / correlationId: '{correlationId}')", message.GetType().Name, returnResponse.GetType().Name, message.RequestHeader.TrackingId, message.RequestHeader.CorrelationId);

            // Check if the output response or request is null. If either is, log the information and exit the method.
            if (output_response == null || output_request == null) {
                _logger.LogInformation("Plugins nullified '{messageType}' or '{output_requestMessageType}'.  Dropping Message (trackingId: '{trackingId}' / correlationId: '{correlationId}')", returnResponse.GetType().Name, message.GetType().Name, message.RequestHeader.TrackingId, message.RequestHeader.CorrelationId);
                return;
            }

            // Update the return response and message with the output from the plugins.
            returnResponse = output_response;
            message = output_request;

            // Log the information about the message being sent to the source app.
            _logger.LogInformation("Sending message '{messageType}' to '{appId}'  (trackingId: '{trackingId}' / correlationId: '{correlationId}')", returnResponse.GetType().Name, fullMessage.SourceAppId, returnResponse.ResponseHeader.TrackingId, returnResponse.ResponseHeader.CorrelationId);
            // Send the return response to the source app.
            _client.DirectToApp(appId: fullMessage.SourceAppId, message: returnResponse);
        };
    }
}
