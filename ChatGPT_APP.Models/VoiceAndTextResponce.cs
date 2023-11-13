using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPT_APP.Models
{
    public class VoiceAndTextResponce
    {
        public Stream? VoiceFileSream { get; set; } = null;
        public string Text { get; set; }
        public int PartNumber { get; set; } 

        public VoiceAndTextResponce(Stream VoiceFileSream, string Text, int PartNumber)
        {
            this.VoiceFileSream = VoiceFileSream;
            this.Text = Text;
            this.PartNumber = PartNumber;
        }
        public VoiceAndTextResponce(string Text)
        {
           this.Text = Text;
        }
    }
}
