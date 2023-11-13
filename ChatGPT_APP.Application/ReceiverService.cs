using ChatGPT_APP;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

public class ReceiverService : BackgroundService
{
    private ITelegramBotClient _botClient { get; }
    private IUpdateHandler _updateHandler { get; }

    private ILogger<ReceiverService> _logger;


    public ReceiverService(
        IUpdateHandler updateHandler, 
        ITelegramBotClient telegramBot, 
        ILogger<ReceiverService> logger)
    {
        _updateHandler = updateHandler;
        _botClient = telegramBot;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_updateHandler == null)
        {
            throw new ArgumentNullException("_updateHandler");
        }

        await StartReceiveAsync(stoppingToken);
    }

    public async Task StartReceiveAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions()
        {
            AllowedUpdates = Array.Empty<UpdateType>(),
            ThrowPendingUpdates = true,
        };

        var me = await _botClient.GetMeAsync(stoppingToken);
        await Console.Out.WriteLineAsync($"Start receiving updates for {me.Username ?? "My Awesome Bot"}");

         _botClient.StartReceiving(
             updateHandler: _updateHandler,
             receiverOptions: receiverOptions,
             cancellationToken: stoppingToken);
    }
}