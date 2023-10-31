
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using NAudio.Wave;
using NAudio.Lame;
using Telegram.Bot.Types;
using System.IO;
using static System.Net.WebRequestMethods;
using OpenAI;
using OpenAI.Audio;
using NVorbis;
using VisioForge.MediaFramework.FFMPEGEXEv2;
using FFmpeg.AutoGen;
using Microsoft.Extensions.Logging;
using CloudConvert.API;
using CloudConvert.API.Models.ExportOperations;
using CloudConvert.API.Models.ImportOperations;
using CloudConvert.API.Models.JobModels;
using CloudConvert.API.Models.TaskOperations;
using System.Net;
using Concentus.Oggfile;
using Concentus.Structs;
using NAudio.Vorbis;
using System.Diagnostics;
using Telegram.Bot.Exceptions;
using ChatGPT_APP.Models;
using Message = ChatGPT_APP.Models.Message;
using VisioForge.MediaFramework.FFMPEGCore.Enums;
using VisioForge.Libs.ZXing;
using DocumentFormat.OpenXml.Drawing;

string apiKey = "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";

string endpoint = "https://api.openai.com/v1/chat/completions";

//string endpointForIamge = "https://api.openai.com/v1/images/generations";

//string endpointForAudio = "https://api.openai.com/v1/audio/transcription";

List<Message> messages = new List<Message>();

var httpClient = new HttpClient();



httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

var telegramBotClient = new TelegramBotClient("6121902784:AAF2klnJ0RYe84AH-h0-i4UoDzfAS66vf_w");

var gorillaGIF = "E:\\C#_WORK\\ChatGPT_APP\\ChatGPT_APP\\Gif\\gorilla-monke.mp4";

telegramBotClient.StartReceiving(Update, HandlePollingErrorAsync);
//telegramBotClient.StartReceiving()



//обработка голосовых сообщений 
async Task Update(ITelegramBotClient telegramBotClient, Telegram.Bot.Types.Update update, CancellationToken token)
{
    string ffmpegPath = "C:\\Users\\DIO\\Desktop\\FFmpeg\\FFmpeg\\ffmpeg-master-latest-win64-gpl-shared\\bin\\ffmpeg.exe";
    var message = update.Message;
    var Id = message.MessageId;
    // "C:\Users\DIO\Desktop\FFmpeg\FFmpeg\ffmpe"
    #region Inpuе\output path
    //Input file 
    var inputFile = $"E:\\C#_WORK\\ChatGPT_APP\\ChatGPT_APP\\InputFiles\\{Id}AudiofileMP3.mp3";
    //Output file 
    var outputFile = $"E:\\C#_WORK\\ChatGPT_APP\\ChatGPT_APP\\OutputFiles\\{Id}AudiofileFromTGoGG.ogg";
    #endregion

    #region gifs path
    //"E:\\C#_WORK\\ChatGPT_APP\\ChatGPT_APP\\Gif\\gorilla-monke.mp4"


    #endregion


    if (message != null && message.Type is MessageType.Voice)
    {
        try
        {
          
                //Просто отравка любого комментария в чат с ботом 
                var messegeMessageReceived = await telegramBotClient.SendTextMessageAsync(message.Chat.Id, "message received ");

                Console.WriteLine($"{message.Chat.Id} - {message.Chat.FirstName} | Time: {DateTime.Now}");

                //Получение файл ID
                var voiceMessageID = message.Voice.FileId;

                await getVoisFileAsync(voiceMessageID, inputFile);

                IsFileReadable(inputFile);

                //  await ConvertOggToMp3(ffmpegPath, inputFile, outputFile);
                // var duration = await GetDuration(ffmpegPath, inputFile);



                var messageText = "Process of synthesizing a voice message into text . . .";

                var messageProcessOfSynthesizing = await telegramBotClient.SendTextMessageAsync(message.Chat.Id, messageText);

                await telegramBotClient.DeleteMessageAsync(message.Chat.Id, messegeMessageReceived.MessageId);


                await Console.Out.WriteLineAsync($"{message.Voice.Duration}");


                if (message.Voice.Duration >= 120)
                {
                    int messageGifId = await SendGif(gorillaGIF, message.Chat.Id);

                    await SendAndSplitSendVoiceMessage(apiKey, ffmpegPath, message, inputFile, message.Voice.Duration, 1);

                    await telegramBotClient.DeleteMessageAsync(message.Chat.Id, messageGifId);
                }
                else
                {
                    //Working with the speech synthesizer API
                    string result = await speechSynthesizer(apiKey, inputFile);

                    await telegramBotClient.DeleteMessageAsync(message.Chat.Id, messageProcessOfSynthesizing.MessageId);

                    await telegramBotClient.SendTextMessageAsync(message.Chat.Id, $" Your voice message: \r {result}");

                    Console.WriteLine($" Сообщение чату: ' {message.Chat.Id} - {message.Chat.FirstName}  '  -  отправленно | Time: {DateTime.Now}");
                }

                #region work with ChatGPT API
                //Sending a GIF message from a selected directory 
                //  int messageGifId = await SendGif(GIFPath, message.Chat.Id);

                //var procesChat =  await telegramBotClient.SendTextMessageAsync(message.Chat.Id, $"Chat processing...");

                // var responceFromCHATgptAPI = await chatFunkAsync(result);

                // await telegramBotClient.DeleteMessageAsync(message.Chat.Id, procesChat.MessageId);

                // await telegramBotClient.DeleteMessageAsync(message.Chat.Id, messageGifId);

                // await telegramBotClient.SendTextMessageAsync(message.Chat.Id, $" Reply from  chatGPT:\r {responceFromCHATgptAPI}");
                #endregion
            
          

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

    }
}

async Task SendAndSplitSendVoiceMessage(string apiKey, string ffmpegPath, Telegram.Bot.Types.Message message, string inputFile, int duration, int v)
{
    await Console.Out.WriteLineAsync("Сообщение длинее 2 минут");
    int partNumber = 0;

    var SplitedAudioList = await SplitAudioByMinute(ffmpegPath, inputFile, inputFile, duration, 1);

   
    foreach (var audio in SplitedAudioList)
    {
        partNumber++;

        int messageGifId = await SendGif(gorillaGIF, message.Chat.Id);
        string result = await speechSynthesizer(apiKey, audio);


        await using Stream stream = System.IO.File.OpenRead(audio);
        await telegramBotClient.SendVoiceAsync(
            chatId: message.Chat.Id,
            voice: InputFile.FromStream(stream));


        await telegramBotClient.SendTextMessageAsync(message.Chat.Id, $" Your voice message part: {partNumber}: \r {result}");

        await telegramBotClient.DeleteMessageAsync(message.Chat.Id, messageGifId);

        Thread.Sleep(3000);
    }

    //Loging
    await Console.Out.WriteLineAsync($"voice message{message.MessageId} completely converted ");
    await telegramBotClient.SendTextMessageAsync(message.Chat.Id, $"Your voice message completely converted {partNumber}");
}

async Task getVoisFileAsync(string voiceMessageID, string inputFile)
{
    //Сохранение файла в директорию
    var voiceFile = await telegramBotClient.GetFileAsync(voiceMessageID);

    using Stream saveFileStream = System.IO.File.Create(inputFile);
    //получение файла 
    await telegramBotClient.DownloadFileAsync(voiceFile.FilePath, saveFileStream);

    saveFileStream.Close();
    Console.WriteLine("Файл загружен ");

    Console.WriteLine(inputFile);
}

async Task ConvertOggToMp3(string ffmpegPath, string inputFile, string outputFile)
{
    ProcessStartInfo startInfo = new ProcessStartInfo();
    startInfo.FileName = ffmpegPath;
    startInfo.Arguments = $"-i \"{inputFile}\" \"{outputFile}\"";
    startInfo.RedirectStandardOutput = true;
    startInfo.RedirectStandardError = true;
    startInfo.UseShellExecute = false;
    startInfo.CreateNoWindow = true;

    using (Process process = new Process { StartInfo = startInfo })
    {
        process.Start();
        process.WaitForExit();
    }

    Console.WriteLine("Конвертация завершена.");
}



async Task<int> SendGif(string GIFPath,long ChatId)
{
    using (var gifStream = System.IO.File.Open(GIFPath, FileMode.Open))
    {
        // InputFile gifFile = new InputFile(gifStream, "my_gif.gif");
        var gifFile = InputFile.FromStream(gifStream, "monkey_gif.gif");
        var messageT = await telegramBotClient.SendAnimationAsync(ChatId, gifFile, caption: "Processing...");

        int messageId = messageT.MessageId;

        return messageId;
        // Console.WriteLine($"Sent message ID: {messageId}");
    }
}

async Task<string> speechSynthesizer(string apiKey, string outputFile)
{
    try
    {
        var api = new OpenAIClient(apiKey);
        var request = new AudioTranscriptionRequest(outputFile);
        string result = await api.AudioEndpoint.CreateTranscriptionAsync(request);

        return result;
    }
    catch (HttpRequestException ex)
    {
        if (ex.StatusCode == HttpStatusCode.TooManyRequests)
        {
            Thread.Sleep(5000);
            Console.WriteLine("Програма на паузе 5 сек");
            return await speechSynthesizer(apiKey, outputFile);
        }
        else
        {

            Console.WriteLine($"Erorr: {ex.Message}");
            throw;
        }
    }
}



// Checking for read and write access to a file   
bool IsFileReadable(string filePath)
{
    try
    {
        using (var fileStream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            Console.WriteLine("Файл открыт только для чтения");
            // 
            return true;
        }
    }
    catch (IOException)
    {
        Console.WriteLine("Файл заблокирован или не доступен для чтения");
        // 
        return false;
    }
}



Console.ReadLine();


//Error hendler 
Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}



//Work with API ChatGPT 
async Task<string> chatFunkAsync(string audioText)
{


    while (true)
    {

        Console.Write("Prompt: ");
       // var content = Console.ReadLine();

        var message = new Message() { Role = "user", Content = audioText };

        messages.Add(message);

        var requestData = new Request()
        {
            ModelId = "gpt-3.5-turbo",
            Messages = messages
        };


        //Отправка запроса и получение ответа 
        using var response = await httpClient.PostAsJsonAsync(endpoint, requestData);



        //Обработка исключений
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"{(int)response.StatusCode} {response.StatusCode}");
            return "Ошибка в конссоле детально";


        }

        //Обработка ответа 
        ResponseData? responseData = await response.Content.ReadFromJsonAsync<ResponseData>();


        var choices = responseData?.Choices ?? new List<Choice>();
        if (choices.Count == 0)
        {
            Console.WriteLine("No choices were returned by the API");
            continue;
        }

        var choice = choices[0];
        var responseMessage = choice.Message;

        messages.Add(responseMessage);
        var responseText = responseMessage.Content.Trim();
        Console.WriteLine( responseText);

        return responseText;
    }
}

async Task<List<string>> SplitAudioByMinute(string ffmpegPath, string inputFile, string outputFilePrefix, int duration, int minutesInterval)
{
  
    int minutesCount = duration / 60;
  
    var outputFiles = new List<string>();

    for (int i = 0; i < minutesCount; i++)
    {

        int start = i * 60;
        int end = (i + minutesInterval) * 60;

        string outputFile = $"{outputFilePrefix}{i}.mp3";

        await Console.Out.WriteLineAsync(outputFile);
     
        await SplitAudio(ffmpegPath, inputFile, outputFile, start, end);

        outputFiles.Add(outputFile);
    }

    foreach (var outputFile in outputFiles)
    {
        Console.WriteLine(outputFile);
    }

    return outputFiles;
}


async Task SplitAudio(string ffmpegPath, string inputFile, string outputFile, int start, int end)
{

    Console.WriteLine(start);
    Console.WriteLine(end);

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
