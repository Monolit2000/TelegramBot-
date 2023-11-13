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
            // ������������� ������������ (��������, Mock ��� ITelegramBotClient)
            _telegramBotClient = new Mock<ITelegramBotClient>().Object;
            _voiceConversionService = new VoiceConversionService(_telegramBotClient);
        }

        [Test]
        public async Task TelegramAudioToTextConverter_LongVoiceMessage_ReturnsExpectedResults()
        {
            // ���������� ������ ��� ����� (��������, �������� mock-������� Telegram.Bot.Types.Message)
            var message = new Message
            {
                Voice = new Voice { Duration = 300 } // ����������������� � �������� (5 �����)
                // ������ ����������� ���������
            };

            // ���������� �����
            var result = _voiceConversionService.TelegramAudioToTextConverter(message);

            await foreach (var voice in result)
            {
                // �������� ����������
                await Console.Out.WriteLineAsync($"Test: {voice.Text}");
                Assert.NotNull(voice);
            }
   
            Assert.NotNull(result);

            // �������� �������������� �������� � ������������ � ��������� ���������� ������
        }

        //[Test]
        //public async Task GetAudioSegmentsWithTextAsync_LongVoiceMessage_ReturnsExpectedResults()
        //{
        //    // ���������� ������ ��� �����

        //    // ���������� �����
        //    var result = await _voiceConversionService.GetAudioSegmentsWithTextAsync(new Message()).ToListAsync();
            
        //    // �������� ����������
        //    Assert.NotNull(result);
        //    // �������� �������������� �������� � ������������ � ��������� ���������� ������
        //}

        //[Test]
        //public async Task GetSinglAudioWithTextAsync_ShortVoiceMessage_ReturnsExpectedResults()
        //{
        //    // ���������� ������ ��� �����

        //    // ���������� �����
        //    var result = await _voiceConversionService.GetSinglAudioWithTextAsync(new Message()).ToListAsync();

        //    // �������� ����������
        //    Assert.NotNull(result);
        //    // �������� �������������� �������� � ������������ � ��������� ���������� ������
        //}

        //[Test]
        //public async Task GetSplitAudioFilePathByUnitOfTimeList_ReturnsExpectedResults()
        //{
        //    // ���������� ������ ��� �����

        //    // ���������� �����
        //    var result = await _voiceConversionService.GetSplitAudioFilePathByUnitOfTimeList(new Message(), 3);

        //    // �������� ����������
        //    Assert.NotNull(result);
        //    Assert.Greater(result.Count, 0);
        //    // �������� �������������� �������� � ������������ � ��������� ���������� ������
        //}

        //[Test]
        //public async Task GetVoiceStream_ReturnsExpectedResults()
        //{
        //    // ���������� ������ ��� �����

        //    // ���������� �����
        //    var result = await _voiceConversionService.GetVoiceStream("voiceFilePath");

        //    // �������� ����������
        //    Assert.NotNull(result);
        //    // �������� �������������� �������� � ������������ � ��������� ���������� ������
        //}

        //[Test]
        //public async Task GetVoisFileAsync_ReturnsExpectedResults()
        //{
        //    // ���������� ������ ��� �����

        //    // ���������� �����
        //    var result = await _voiceConversionService.GetVoisFileAsync(new Message());

        //    // �������� ����������
        //    Assert.NotNull(result);
        //    Assert.IsTrue(File.Exists(result));
        //    // �������� �������������� �������� � ������������ � ��������� ���������� ������
        //}

        //[Test]
        //public async Task SplitAudio_ReturnsExpectedResults()
        //{
        //    // ���������� ������ ��� �����

        //    // ���������� �����
        //    await _voiceConversionService.SplitAudio("inputFile", "outputFile", 0, 60);

        //    // �������� ���������� (��������, ���������, ��� ���� outputFile ����������)
        //    Assert.IsTrue(File.Exists("outputFile"));
        //    // �������� �������������� �������� � ������������ � ��������� ���������� ������
        //}

        //[Test]
        //public async Task TelegramVoiseMessageSynthesizer_ReturnsExpectedResults()
        //{
        //    // ���������� ������ ��� �����

        //    // ���������� �����
        //    var result = await _voiceConversionService.TelegramVoiseMessageSynthesizer("AudioFilePath");

        //    // �������� ����������
        //    Assert.NotNull(result);
        //    // �������� �������������� �������� � ������������ � ��������� ���������� ������
        //}

        // ������ ����� ��� �������, ������� ������� ��������
    }
}