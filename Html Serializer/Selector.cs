using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Html_Serializer
{
    internal class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; }
        public Selector Parent { get; set; }
        public Selector Child { get; set; }

       public static Selector ConventToSelector(string query)
        {
            string[] levels = query.Split(' ');
            Selector root = new Selector();
            Selector currentSelector = root;

            foreach (string level in levels)
            {
                Match matchs = Regex.Match(level, @"(?<tag>\w+)(?:#(?<id>\w+))?(?:\.(?<class>\w+))*");
                
                string tagName = matchs.Groups["tag"].Value;
               string id = matchs.Groups["id"].Value;
                string classes = string.Join("", matchs.Groups["class"].Captures.Cast<Capture>().Select(c => c.Value));
                //string[] arrClasses = classes.Split(' ');

                Selector newSelector = new Selector();

                if (HtmlHelper.Instance.SelfClosingTags.Contains(tagName) || HtmlHelper.Instance.ClosingTags.Contains(tagName))
                {
                    newSelector.TagName = tagName;
                }

               

                if(string.IsNullOrEmpty(id)) {
                newSelector.Id = id;}

                if (!string.IsNullOrEmpty(classes)) { 
               
                        newSelector.Classes = classes.Split('.').Where(c => !string.IsNullOrEmpty(c)).ToList();

                    }

                currentSelector.Child = newSelector;
                currentSelector = currentSelector.Child;

            }
            return root.Child;
        }

    }
}
