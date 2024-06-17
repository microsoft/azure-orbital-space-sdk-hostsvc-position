namespace Microsoft.Azure.SpaceFx.HostServices.Position.Plugins;
public abstract class PluginBase : Core.IPluginBase, IPluginBase {
    public abstract ILogger Logger { get; set; }
    public abstract Task BackgroundTask();
    public abstract void ConfigureLogging(ILoggerFactory loggerFactory);
    public abstract Task<PluginHealthCheckResponse> PluginHealthCheckResponse();

    // Position Service Stuff
    public abstract Task<(MessageFormats.HostServices.Position.PositionRequest?, MessageFormats.HostServices.Position.PositionResponse?)> PositionRequest(MessageFormats.HostServices.Position.PositionRequest? input_request, MessageFormats.HostServices.Position.PositionResponse? input_response);
    public abstract Task<(MessageFormats.HostServices.Position.PositionUpdateRequest?, MessageFormats.HostServices.Position.PositionUpdateResponse?)> PositionUpdateRequest(MessageFormats.HostServices.Position.PositionUpdateRequest? input_request, MessageFormats.HostServices.Position.PositionUpdateResponse? input_response);
}

public interface IPluginBase {
    ILogger Logger { get; set; }
    Task<(MessageFormats.HostServices.Position.PositionRequest?, MessageFormats.HostServices.Position.PositionResponse?)> PositionRequest(MessageFormats.HostServices.Position.PositionRequest? input_request, MessageFormats.HostServices.Position.PositionResponse? input_response);
    Task<(MessageFormats.HostServices.Position.PositionUpdateRequest?, MessageFormats.HostServices.Position.PositionUpdateResponse?)> PositionUpdateRequest(MessageFormats.HostServices.Position.PositionUpdateRequest? input_request, MessageFormats.HostServices.Position.PositionUpdateResponse? input_response);
}
