using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChatGPT_APP.Models
{
    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = "";

        [JsonPropertyName("content")]
        public string Content { get; set; } = "";
    }
    class Request
    {
        [JsonPropertyName("model")]

        public string ModelId { get; set; } = "";

        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; } = new();
    }

    class ResponseData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("object")]
        public string Object { get; set; } = "";

        [JsonPropertyName("created")]
        public ulong Created { get; set; }

        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; } = new();

        [JsonPropertyName("usage")]
        public Usage Usage { get; set; } = new();
    }

    class Choice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("message")]
        public Message Message { get; set; } = new();

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; } = "";
    }

    class Usage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }

    class ResponseDataForImage
    {
        public ulong Created { get; set; }

        [JsonPropertyName("data")]
        public List<UrlData> data { get; set; } = new();
    }

    public class UrlData
    {
        [JsonPropertyName("Url")]
        public string Url { get; set; } = "";
    }

    class RequestForImage
    {
        [JsonPropertyName("prompt")]
        public string prompt { get; set; } = "";

        [JsonPropertyName("n")]
        public int n { get; set; }

        [JsonPropertyName("size")]
        public string size { get; set; } = "";
    }


    class RequestForAudio
    {
        [JsonPropertyName("file")]
        public byte[] File { get; set; }

        [JsonPropertyName("model")]
        public string model { get; set; } = "";
    }

    class ResponseDataForAudio
    {
        [JsonPropertyName("text")]
        public string text { get; set; }
    }

    class RequestForConversation
    {



    }
}
