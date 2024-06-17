namespace Microsoft.Azure.SpaceFx.HostServices.Position;
public class Utils {
    public class PluginDelegates {
        private readonly ILogger<PluginDelegates> _logger;
        private readonly List<Core.Models.PLUG_IN> _plugins;
        public PluginDelegates(ILogger<PluginDelegates> logger, IServiceProvider serviceProvider) {
            _logger = logger;
            _plugins = serviceProvider.GetService<List<Core.Models.PLUG_IN>>() ?? new List<Core.Models.PLUG_IN>();
        }

        internal (MessageFormats.HostServices.Position.PositionRequest? output_request, MessageFormats.HostServices.Position.PositionResponse? output_response) PositionRequest((MessageFormats.HostServices.Position.PositionRequest? input_request, MessageFormats.HostServices.Position.PositionResponse? input_response, Plugins.PluginBase plugin) input) {
            const string methodName = nameof(input.plugin.PositionRequest);
            if (input.input_request is null || input.input_request is default(MessageFormats.HostServices.Position.PositionRequest)) {
                _logger.LogDebug("Plugin {Plugin_Name} / {methodName}: Received empty input.  Returning empty results", input.plugin.ToString(), methodName);
                return (input.input_request, input.input_response);
            }
            _logger.LogDebug("Plugin {Plugin_Name} / {methodName}: START", input.plugin.ToString(), methodName);

            try {
                Task<(MessageFormats.HostServices.Position.PositionRequest? output_request, MessageFormats.HostServices.Position.PositionResponse? output_response)> pluginTask = input.plugin.PositionRequest(input_request: input.input_request, input_response: input.input_response);
                pluginTask.Wait();

                input.input_request = pluginTask.Result.output_request;
                input.input_response = pluginTask.Result.output_response;
            } catch (Exception ex) {
                _logger.LogError("Error in plugin '{Plugin_Name}:{methodName}'.  Error: {errMsg}", input.plugin.ToString(), methodName, ex.Message);
            }

            _logger.LogDebug("Plugin {Plugin_Name} / {methodName}: END", input.plugin.ToString(), methodName);
            return (input.input_request, input.input_response);
        }

        internal (MessageFormats.HostServices.Position.PositionUpdateRequest? output_request, MessageFormats.HostServices.Position.PositionUpdateResponse? output_response) PositionUpdateRequest((MessageFormats.HostServices.Position.PositionUpdateRequest? input_request, MessageFormats.HostServices.Position.PositionUpdateResponse? input_response, Plugins.PluginBase plugin) input) {
            const string methodName = nameof(input.plugin.PositionUpdateRequest);
            if (input.input_request is null || input.input_request is default(MessageFormats.HostServices.Position.PositionUpdateRequest)) {
                _logger.LogDebug("Plugin {Plugin_Name} / {methodName}: Received empty input.  Returning empty results", input.plugin.ToString(), methodName);
                return (input.input_request, input.input_response);
            }
            _logger.LogDebug("Plugin {Plugin_Name} / {methodName}: START", input.plugin.ToString(), methodName);

            try {
                Task<(MessageFormats.HostServices.Position.PositionUpdateRequest? output_request, MessageFormats.HostServices.Position.PositionUpdateResponse? output_response)> pluginTask = input.plugin.PositionUpdateRequest(input_request: input.input_request, input_response: input.input_response);
                pluginTask.Wait();

                input.input_request = pluginTask.Result.output_request;
                input.input_response = pluginTask.Result.output_response;
            } catch (Exception ex) {
                _logger.LogError("Error in plugin '{Plugin_Name}:{methodName}'.  Error: {errMsg}", input.plugin.ToString(), methodName, ex.Message);
            }

            _logger.LogDebug("Plugin {Plugin_Name} / {methodName}: END", input.plugin.ToString(), methodName);
            return (input.input_request, input.input_response);
        }
    }
}