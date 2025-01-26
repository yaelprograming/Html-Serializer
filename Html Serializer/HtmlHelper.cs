using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace Html_Serializer
{
    internal class HtmlHelper
    {

        public string[] ClosingTags { get; set; }
        public string[] SelfClosingTags { get; set; }

        private readonly static HtmlHelper _instance = new HtmlHelper();
        public static HtmlHelper Instance => _instance;
        private HtmlHelper()
        {
            var content = File.ReadAllText("HtmlTags.json");
            this.ClosingTags = JsonSerializer.Deserialize<string[]>(content);
            content = File.ReadAllText("HtmlVoidTags.json");
            this.SelfClosingTags = JsonSerializer.Deserialize<string[]>(content);
        }
    }
}
