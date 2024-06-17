


namespace Microsoft.Azure.SpaceFx.HostServices.Position.Plugins;
public class IntegrationTestPlugin : Microsoft.Azure.SpaceFx.HostServices.Position.Plugins.PluginBase {

    public override ILogger Logger { get; set; }

    public IntegrationTestPlugin() {
        LoggerFactory loggerFactory = new();
        Logger = loggerFactory.CreateLogger<IntegrationTestPlugin>();
    }

    public override Task BackgroundTask() => Task.Run(async () => {
        Console.WriteLine("I started a background task!");
    });

    public override void ConfigureLogging(ILoggerFactory loggerFactory) => Logger = loggerFactory.CreateLogger<IntegrationTestPlugin>();

    public override Task<PluginHealthCheckResponse> PluginHealthCheckResponse() => Task.FromResult(new MessageFormats.Common.PluginHealthCheckResponse());

    public override Task<(PositionRequest?, PositionResponse?)> PositionRequest(PositionRequest? input_request, PositionResponse? input_response) => Task.Run(() => {
        return (input_request, input_response);
    });

    public override Task<(PositionUpdateRequest?, PositionUpdateResponse?)> PositionUpdateRequest(PositionUpdateRequest? input_request, PositionUpdateResponse? input_response) => Task.Run(() => {
        return (input_request, input_response);
    });
}
