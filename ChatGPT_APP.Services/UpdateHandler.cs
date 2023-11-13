using ChatGPT_APP.Models;
using ChatGPT_APP.Services.Contract;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace ChatGPT_APP
{
    public class UpdateHandler : IUpdateHandler
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly ILogger<UpdateHandler> _logger;
        private readonly IVoiceConversionService _voiceConversionService;

        public UpdateHandler(
            ITelegramBotClient botClient, 
            ILogger<UpdateHandler> logger, 
            IVoiceConversionService voiceConversionService)
        {
            _voiceConversionService = voiceConversionService;
            _telegramBotClient = botClient;
            _logger = logger;
        }
        public async Task HandleUpdateAsync(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message;

            if (message != null && message.Type is MessageType.Voice)
            {
                DateTime start = DateTime.Now;
                await BotOnVoiseMessageReceived(message);
                DateTime end = DateTime.Now;
                await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, "Voice message completely converted!");
                TimeSpan duration = end - start;
                await Console.Out.WriteLineAsync($"{duration}");
            }
        }

        private async Task BotOnVoiseMessageReceived(Telegram.Bot.Types.Message message)
        {
            try
            {
                var messageGifId = await SendGif(Configuration.spiderMan_GIF, message.Chat.Id); 

                var responceListIterator = _voiceConversionService.TelegramAudioToTextConverter(message);
                await foreach(var responce in responceListIterator)
                {
                    await SendAudioWithText(message.Chat.Id, responce);
                }

                await _telegramBotClient.DeleteMessageAsync(message.Chat.Id, messageGifId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, $" Я сламалса :( {ex.Message}");

            }
        }

        #region Send Audio With Text
        private async Task SendAudioWithText(long chatId, VoiceAndTextResponce VoisWithText)
        {              
            if (VoisWithText.VoiceFileSream != null)
                await _telegramBotClient.SendVoiceAsync(chatId: chatId, voice: InputFile.FromStream(VoisWithText.VoiceFileSream));

            await _telegramBotClient.SendTextMessageAsync(chatId, $" Your voice message part: {VoisWithText.PartNumber}: \r {VoisWithText.Text}");

            await Console.Out.WriteLineAsync($"voice message part: {VoisWithText.PartNumber} sended ");  
        }
        #endregion

        #region Send Gif
        async Task<int> SendGif(string GIFPath, long ChatId)
        {
            using (var gifStream = System.IO.File.Open(GIFPath, FileMode.Open))
            {
                // InputFile gifFile = new InputFile(gifStream, "my_gif.gif");
                var gifFile = InputFile.FromStream(gifStream, "monkey_gif.gif");
                var messageT = await _telegramBotClient.SendAnimationAsync(ChatId, gifFile, caption: "Processing...");

                int messageId = messageT.MessageId;

                return messageId;
                // Console.WriteLine($"Sent message ID: {messageId}");
            }
        }
        #endregion

        #region Handle Polling Error
        public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);

            // Cooldown in case of network connection error
            if (exception is RequestException)
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }
        #endregion

        #region work with ChatGPT API
        private async Task SendResponceFromChatGPT()
        {
            //Sending a GIF message from a selected directory 
            //  int messageGifId = await SendGif(GIFPath, message.Chat.Id);

            //var procesChat =  await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, $"Chat processing...");

            // var responceFromCHATgptAPI = await chatFunkAsync(result);

            // await _telegramBotClient.DeleteMessageAsync(message.Chat.Id, procesChat.MessageId);

            // await _telegramBotClient.DeleteMessageAsync(message.Chat.Id, messageGifId);

            // await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, $" Reply from  chatGPT:\r {responceFromCHATgptAPI}");
        }
        #endregion

    }
}
