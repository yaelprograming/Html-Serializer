using Html_Serializer;
using System.Text.RegularExpressions;
static async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}

string url = Console.ReadLine();

var html = await Load(url);

var htmlLines=new Regex("<(.*?)>").Split(new Regex("\\s").Replace(html, "")).Where(l=>l.Length>0).ToList();


 static void PrintHtmlTree(HtmlElement element, int level = 0)
{
    Console.WriteLine(new string(' ', level * 2) + element.Name + (element.Id != null ? " (ID: " + element.Id + ")" : ""));
    foreach (var child in element.Children)
    {
        PrintHtmlTree(child, level + 1);
    }
}


static HtmlElement BuildTree(List<string>lines)
{
    HtmlElement root = new HtmlElement();
    HtmlElement currentElement = root;

    foreach (var line in lines)
    {
        string[] words = line.Split(' ');

        if (!words[0].Equals("/html"))
        {
            if (words[0].StartsWith("/"))
            {
                if (currentElement.Parent != null)// רק אם יש הורה
                {
                    currentElement = currentElement.Parent; // חזרה לאלמנט ההורה
                }
                else
                {
                    Console.WriteLine("Error: No parent found for closing tag.");
                }
            }
            else if (HtmlHelper.Instance.ClosingTags.Contains(words[0]) || HtmlHelper.Instance.SelfClosingTags.Contains(words[0]))
            {
                //add base
                HtmlElement newElement = new HtmlElement();
                newElement.Parent = currentElement;
                newElement.Name = words[0];
                currentElement.Children.Add(newElement);
                currentElement = newElement;

                if (line.IndexOf(' ') > 0)
                {
                    //add atribute and classes
                    var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(line.Substring(line.IndexOf(" "))).ToList();
                    foreach (var attribute in attributes)
                    {
                        string[] arrA = attribute.ToString().Split('=');
                        if (arrA[0].Equals("id"))
                            currentElement.Id = arrA[1];
                        else if (arrA[0].Equals("class"))
                            currentElement.Classes = arrA[1].Split(" ").ToList();

                        else
                            currentElement.Attributes.Add(arrA[0], arrA[1]);
                    }
                }
                if (HtmlHelper.Instance.SelfClosingTags.Contains(words[0])) //אם הסגירה באותה שורה
                    currentElement = currentElement.Parent;
               
            }
            else
                currentElement.InnerHtml = line;
        }
    }
    return root;
}

HtmlElement root = BuildTree(htmlLines);
PrintHtmlTree(root);

Console.WriteLine("Enter a selector (e.g., div#main.class1.class2):");
string selectorQuery = Console.ReadLine();

// המרת הסלקטור לאובייקט Selector
Selector selector = Selector.ConventToSelector(selectorQuery);

// חיפוש אלמנטים בעץ עם הסלקטור
var matchingElements = root.FindElementsBySelector(selector);

// הדפסת התוצאות
Console.WriteLine($"Found {matchingElements.Count()} matching elements:");
foreach (var element in matchingElements)
{
    Console.WriteLine($"Tag: {element.Name}, ID: {element.Id}, Classes: {string.Join(", ", element.Classes)}");
}