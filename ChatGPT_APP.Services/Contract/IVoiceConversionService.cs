using ChatGPT_APP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPT_APP.Services.Contract
{
    public interface IVoiceConversionService
    {
        IAsyncEnumerable<VoiceAndTextResponce> TelegramAudioToTextConverter(Telegram.Bot.Types.Message message);
    }
}
