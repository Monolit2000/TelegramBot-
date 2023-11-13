using System.IO;
using ChatGPT_APP.Services;
using ChatGPT_APP.Services.Contract;
using Moq;
using NUnit.Framework;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace ChatGPT_APP.Test_Conversion
{

    [TestFixture]
    public class VoiceConversionServiceTests
    {
        private IVoiceConversionService _voiceConversionService;
        private ITelegramBotClient _telegramBotClient;

        [SetUp]
        public void Setup()
        {
            // Инициализация зависимостей (например, Mock для ITelegramBotClient)
            _telegramBotClient = new Mock<ITelegramBotClient>().Object;
            _voiceConversionService = new VoiceConversionService(_telegramBotClient);
        }

        [Test]
        public async Task TelegramAudioToTextConverter_LongVoiceMessage_ReturnsExpectedResults()
        {
            // Подготовка данных для теста (например, создание mock-объекта Telegram.Bot.Types.Message)
            var message = new Message
            {
                Voice = new Voice { Duration = 300 } // Продолжительность в секундах (5 минут)
                // Другие необходимые параметры
            };

            // Выполнение теста
            var result = _voiceConversionService.TelegramAudioToTextConverter(message);

            await foreach (var voice in result)
            {
                // Проверки результата
                await Console.Out.WriteLineAsync($"Test: {voice.Text}");
                Assert.NotNull(voice);
            }
   
            Assert.NotNull(result);

            // Добавьте дополнительные проверки в соответствии с ожидаемым поведением метода
        }

        //[Test]
        //public async Task GetAudioSegmentsWithTextAsync_LongVoiceMessage_ReturnsExpectedResults()
        //{
        //    // Подготовка данных для теста

        //    // Выполнение теста
        //    var result = await _voiceConversionService.GetAudioSegmentsWithTextAsync(new Message()).ToListAsync();
            
        //    // Проверки результата
        //    Assert.NotNull(result);
        //    // Добавьте дополнительные проверки в соответствии с ожидаемым поведением метода
        //}

        //[Test]
        //public async Task GetSinglAudioWithTextAsync_ShortVoiceMessage_ReturnsExpectedResults()
        //{
        //    // Подготовка данных для теста

        //    // Выполнение теста
        //    var result = await _voiceConversionService.GetSinglAudioWithTextAsync(new Message()).ToListAsync();

        //    // Проверки результата
        //    Assert.NotNull(result);
        //    // Добавьте дополнительные проверки в соответствии с ожидаемым поведением метода
        //}

        //[Test]
        //public async Task GetSplitAudioFilePathByUnitOfTimeList_ReturnsExpectedResults()
        //{
        //    // Подготовка данных для теста

        //    // Выполнение теста
        //    var result = await _voiceConversionService.GetSplitAudioFilePathByUnitOfTimeList(new Message(), 3);

        //    // Проверки результата
        //    Assert.NotNull(result);
        //    Assert.Greater(result.Count, 0);
        //    // Добавьте дополнительные проверки в соответствии с ожидаемым поведением метода
        //}

        //[Test]
        //public async Task GetVoiceStream_ReturnsExpectedResults()
        //{
        //    // Подготовка данных для теста

        //    // Выполнение теста
        //    var result = await _voiceConversionService.GetVoiceStream("voiceFilePath");

        //    // Проверки результата
        //    Assert.NotNull(result);
        //    // Добавьте дополнительные проверки в соответствии с ожидаемым поведением метода
        //}

        //[Test]
        //public async Task GetVoisFileAsync_ReturnsExpectedResults()
        //{
        //    // Подготовка данных для теста

        //    // Выполнение теста
        //    var result = await _voiceConversionService.GetVoisFileAsync(new Message());

        //    // Проверки результата
        //    Assert.NotNull(result);
        //    Assert.IsTrue(File.Exists(result));
        //    // Добавьте дополнительные проверки в соответствии с ожидаемым поведением метода
        //}

        //[Test]
        //public async Task SplitAudio_ReturnsExpectedResults()
        //{
        //    // Подготовка данных для теста

        //    // Выполнение теста
        //    await _voiceConversionService.SplitAudio("inputFile", "outputFile", 0, 60);

        //    // Проверки результата (например, убедитесь, что файл outputFile существует)
        //    Assert.IsTrue(File.Exists("outputFile"));
        //    // Добавьте дополнительные проверки в соответствии с ожидаемым поведением метода
        //}

        //[Test]
        //public async Task TelegramVoiseMessageSynthesizer_ReturnsExpectedResults()
        //{
        //    // Подготовка данных для теста

        //    // Выполнение теста
        //    var result = await _voiceConversionService.TelegramVoiseMessageSynthesizer("AudioFilePath");

        //    // Проверки результата
        //    Assert.NotNull(result);
        //    // Добавьте дополнительные проверки в соответствии с ожидаемым поведением метода
        //}

        // Другие тесты для методов, которые требуют проверки
    }
}