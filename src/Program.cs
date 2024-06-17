namespace Microsoft.Azure.SpaceFx.HostServices.Position;

public class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile("/workspaces/hostsvc-position-config/appsettings.json", optional: true, reloadOnChange: true)
                             .AddJsonFile("/workspaces/hostsvc-position/src/appsettings.json", optional: true, reloadOnChange: true)
                             .AddJsonFile("/workspaces/hostsvc-position/src/appsettings.{env:DOTNET_ENVIRONMENT}.json", optional: true, reloadOnChange: true)
                             .Build();

        builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(50051, o => o.Protocols = HttpProtocols.Http2))
        .ConfigureServices((services) => {
            services.AddAzureOrbitalFramework();
            services.AddSingleton<Core.IMessageHandler<MessageFormats.HostServices.Position.PositionRequest>, MessageHandler<MessageFormats.HostServices.Position.PositionRequest>>();
            services.AddSingleton<Core.IMessageHandler<MessageFormats.HostServices.Position.PositionUpdateRequest>, MessageHandler<MessageFormats.HostServices.Position.PositionUpdateRequest>>();
            services.AddSingleton<Utils.PluginDelegates>();

        }).ConfigureLogging((logging) => {
            logging.AddProvider(new Microsoft.Extensions.Logging.SpaceFX.Logger.HostSvcLoggerProvider());
            logging.AddConsole();
        });

        var app = builder.Build();

        app.UseRouting();
        app.UseEndpoints(endpoints => {
            endpoints.MapGrpcService<Microsoft.Azure.SpaceFx.Core.Services.MessageReceiver>();
            endpoints.MapGet("/", async context => {
                await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            });
        });
        app.Run();
    }
}