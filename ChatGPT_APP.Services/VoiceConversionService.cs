using System.Diagnostics;
using System.Net;
using Telegram.Bot.Types;
using Telegram.Bot;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Audio;
using ChatGPT_APP.Models;
using ChatGPT_APP.Services.Contract;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace ChatGPT_APP.Services
{
    public class VoiceConversionService : IVoiceConversionService
    {
        private readonly string apiKey = Configuration.OpenAI_Api_Key;
        private readonly string ffmpegPath = Configuration.ffmpegPath;
        private readonly string SaveFilePath = Configuration.SaveFilePath;
        private readonly ITelegramBotClient _telegramBotClient;
        // private readonly ILogger<UpdateHandler> _logger;

        public VoiceConversionService(ITelegramBotClient botClient)
        {
            _telegramBotClient = botClient; 
        }


        public async IAsyncEnumerable<VoiceAndTextResponce> TelegramAudioToTextConverter(Telegram.Bot.Types.Message message)
        {
            var voiceAndTextResponce = GetVoiceAndTextIteratorAsync(message);

            await foreach (var responce in voiceAndTextResponce)
            {
                await foreach (var responcee in responce)
                {
                    yield return responcee;
                }
            }
        }

        private async IAsyncEnumerable<IAsyncEnumerable<VoiceAndTextResponce>> GetVoiceAndTextIteratorAsync(Telegram.Bot.Types.Message message)
        {
            var duration = message.Voice.Duration;
            const int twoMinute = 4 * 60;

            switch (duration)
            {
                case >= twoMinute:
                    yield return GetAudioSegmentsWithTextAsync(message);
                    break;

                default:
                    yield return GetSinglAudioWithTextAsync(message);
                    break;
            }
        }

        private async IAsyncEnumerable<VoiceAndTextResponce> GetAudioSegmentsWithTextAsync(Telegram.Bot.Types.Message message)
        {
            await Console.Out.WriteLineAsync("Long vosie message");

            var splitedAudioList = await GetSplitAudioFilePathByUnitOfTimeList(message, 3);

            for (int partNumber = 0; partNumber < splitedAudioList.Count; partNumber++)
            {
               var voiceStream = await GetVoiceStream(splitedAudioList[partNumber]);
               var synthesTest = await TelegramVoiseMessageSynthesizer(splitedAudioList[partNumber]);

               yield return new VoiceAndTextResponce(voiceStream, synthesTest, partNumber);
            }
            await Console.Out.WriteLineAsync($"voice message{message.MessageId} completely converted ");
        }

        private async IAsyncEnumerable<VoiceAndTextResponce> GetSinglAudioWithTextAsync(Telegram.Bot.Types.Message message)
        {
            await Console.Out.WriteLineAsync("Short vosie message ");

            var VosieFile = await GetVoisFileAsync(message);
            var synthesizedText = await TelegramVoiseMessageSynthesizer(VosieFile);

            yield return new VoiceAndTextResponce(synthesizedText);
        }

        private async Task<List<string>> GetSplitAudioFilePathByUnitOfTimeList(Telegram.Bot.Types.Message message, int TimeInterval)
        {
            var outputFiles = new List<string>();
  
            var inputFilePath = await GetVoisFileAsync(message);
            await Console.Out.WriteLineAsync("Opa");

            int minutesCount = message.Voice.Duration / (TimeInterval* 60);

            Console.WriteLine(minutesCount);

            for (int i = 0; i < minutesCount; i++)
            {
                int start = i * 60;
                int end = (i + TimeInterval) * 60;

                string convertedAudioFilePath = $"{inputFilePath}{i}.mp3";

                await SplitAudio(inputFilePath, convertedAudioFilePath, start, end);

                outputFiles.Add(convertedAudioFilePath);
            }
            return outputFiles;
        }
        private async Task<Stream> GetVoiceStream(string voiceFilePath)
        {
            var voiceStream =  System.IO.File.OpenRead(voiceFilePath);
            return voiceStream;
        }

        public async Task<string> GetVoisFileAsync(Telegram.Bot.Types.Message message)
        {
            //Создание пустого файла в директорию
            await Console.Out.WriteLineAsync(message.MessageId.ToString());

            var VoiseFilePath = $"{SaveFilePath}+{message.MessageId}AudiofileMP3.mp3";
            using Stream saveFileStream = System.IO.File.Create(VoiseFilePath);

            //получение файла 
            try
            {   
                var size = message.Voice.FileSize;
                Console.WriteLine(size);
                var voiceFile = await _telegramBotClient.GetFileAsync(message.Voice.FileId);
                await _telegramBotClient.DownloadFileAsync(voiceFile.FilePath, saveFileStream);
            }
            catch(Exception ex) 
            {
                await Console.Out.WriteLineAsync($"{ex}");
            }

            saveFileStream.Close();

            Console.WriteLine("Voise-Файл загружен ");
            return VoiseFilePath;
        }

        private async Task SplitAudio(string inputFile, string outputFile, int start, int end)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = ffmpegPath;
            startInfo.Arguments = $"-i \"{inputFile}\" -ss {start} -to {end} \"{outputFile}\"";
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();
                await process.WaitForExitAsync();
            }
        }

        public async Task<string> TelegramVoiseMessageSynthesizer(string AudioFilePath)
        {
            try
            {
                var api = new OpenAIClient(apiKey);
                var request = new AudioTranscriptionRequest(AudioFilePath);
                string result = await api.AudioEndpoint.CreateTranscriptionAsync(request);

                return result;
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    Console.WriteLine("Програма на паузе 5 сек");
                   // Console.WriteLine($"Erorr: {ex.Message}");
                    return await TelegramVoiseMessageSynthesizer(AudioFilePath);
                }
                else
                {
                    Console.WriteLine($"Erorr: {ex.Message}");
                    throw;
                }
            }
        }



        // Checking for read and write access to a file   
        private bool IsFileReadable(string filePath)
        {
            try
            {
                using (var fileStream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    Console.WriteLine("Файл открыт только для чтения");
                    return true;
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Файл заблокирован или не доступен для чтения");
                return false;
            }
        }

        #region Work with API ChatGPT 
        //Work with API ChatGPT 
        //async Task<string> chatFunkAsync(string audioText)
        //{

        //    while (true)
        //    {
        //        Console.Write("Prompt: ");

        //        var message = new Message() { Role = "user", Content = audioText };

        //        messages.Add(message);

        //        var requestData = new Request()
        //        {
        //            ModelId = "gpt-3.5-turbo",
        //            Messages = messages
        //        };
        //        //Отправка запроса и получение ответа 
        //        using var response = await httpClient.PostAsJsonAsync(endpoint, requestData);

        //        //Обработка исключений
        //        if (!response.IsSuccessStatusCode)
        //        {
        //            Console.WriteLine($"{(int)response.StatusCode} {response.StatusCode}");
        //            return "Ошибка в конссоле детально";
        //        }

        //        //Обработка ответа 
        //        ResponseData? responseData = await response.Content.ReadFromJsonAsync<ResponseData>();

        //        var choices = responseData?.Choices ?? new List<Choice>();
        //        if (choices.Count == 0)
        //        {
        //            Console.WriteLine("No choices were returned by the API");
        //            continue;
        //        }

        //        var choice = choices[0];
        //        var responseMessage = choice.Message;

        //        messages.Add(responseMessage);
        //        var responseText = responseMessage.Content.Trim();
        //        Console.WriteLine(responseText);

        //        return responseText;
        //    }
        //}
        #endregion

        #region Convert Ogg To Mp3
        //private async Task ConvertOggToMp3(string ffmpegPath, string inputFile, string outputFile)
        //{
        //    ProcessStartInfo startInfo = new ProcessStartInfo();
        //    startInfo.FileName = ffmpegPath;
        //    startInfo.Arguments = $"-i \"{inputFile}\" \"{outputFile}\"";
        //    startInfo.RedirectStandardOutput = true;
        //    startInfo.RedirectStandardError = true;
        //    startInfo.UseShellExecute = false;
        //    startInfo.CreateNoWindow = true;

        //    using (Process process = new Process { StartInfo = startInfo })
        //    {
        //        process.Start();
        //        process.WaitForExit();
        //    }

        //    Console.WriteLine("Конвертация завершена.");
        //}
        #endregion

    }
}