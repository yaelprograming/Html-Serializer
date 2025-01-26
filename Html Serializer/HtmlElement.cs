using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Html_Serializer
{
    internal class HtmlElement
    {
        public HtmlElement() { }
        public HtmlElement(string name)
        {
            Name = name;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<string> Classes { get; set; } = new List<string>();
        public string InnerHtml { get; set; }

        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; } = new List<HtmlElement>();

        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> q = new Queue<HtmlElement>();
            q.Enqueue(this);
            while (q.Count > 0)
            {
                HtmlElement element = q.Dequeue();
                yield return element;

                foreach (var child in element.Children)
                    q.Enqueue(child);
            }
        }

        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement element = this;

            while (element != null)
            {
                yield return element;
                element = element.Parent;
            }
        }

        public IEnumerable<HtmlElement> FindElementsBySelector(Selector selector)
        {
            List<HtmlElement> result = new List<HtmlElement>();
 
            foreach (var descendant in this.Descendants())
            {
                if (MatchesSelector( selector, descendant))  // בודקים אם האלמנט תואם לסלקטור
                {
                    result.Add(descendant);
                }
            }
            if (selector.Child != null)
            {
                foreach (var child in result.ToList())
                {
                    var filteredResults = child.FindElementsBySelector(selector.Child);
                    result.AddRange(filteredResults);  // מוסיפים את התוצאות המועדות של הילד
                }
            }

            Console.WriteLine("Elements found:");
            foreach (var element in result)
            {
                Console.WriteLine($"Tag: {element.Name}, ID: {element.Id}, Classes: {string.Join(", ", element.Classes)}");
            }

            return result;
        }

        public bool MatchesSelector(Selector selector,HtmlElement element)
        {
            if (!string.IsNullOrEmpty(selector.TagName) &&element.Name != selector.TagName)
                return false;

            if (!string.IsNullOrEmpty(selector.Id) && element.Id != selector.Id)
                return false;

            if (selector.Classes!=null && !selector.Classes.All(c => element.Classes.Contains(c)))
                return false;

            return true;
        }


    }
}
