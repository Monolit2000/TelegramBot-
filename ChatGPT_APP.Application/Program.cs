using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ChatGPT_APP.Services;
using Telegram.Bot.Polling;
using Microsoft.Bot.Configuration;
using Telegram.Bot;
using ChatGPT_APP;
using ChatGPT_APP.Services.Contract;
using Microsoft.Extensions.Logging;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.SetMinimumLevel(LogLevel.None);    
    })
    .ConfigureServices((context, services) =>
    {
        // Register Bot configuration
        services.Configure<BotConfiguration>(
            context.Configuration.GetSection(BotConfiguration.Configuration));

        // Register named HttpClient to benefits from IHttpClientFactory
        // and consume it with ITelegramBotClient typed client.
        // More read:
        //  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0#typed-clients
        //  https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
        services.AddHttpClient("telegram_bot_client")
                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                {
                    //BotConfiguration? botConfig = sp.GetConfiguration<BotConfiguration>();
                    TelegramBotClientOptions options = new(BotConfiguration.BotToken);
                    return new TelegramBotClient(options, httpClient);
                });
        services.AddScoped<IVoiceConversionService, VoiceConversionService>();
        services.AddScoped<IUpdateHandler, UpdateHandler>();
        services.AddHostedService<ReceiverService>();
    }).Build();

await host.RunAsync();


public class BotConfiguration

{
    public static readonly string Configuration = "BotConfiguration";

    public static string BotToken { get; set; } = ""; 
}
