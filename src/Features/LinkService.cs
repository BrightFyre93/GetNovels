using System.Collections.Generic;
using System.Windows;

namespace GetNovels.Features
{
    public static class LinkService
    {
        public static List<string> GetLinks(string data, string baseUrl)
        {
            var links = new List<string>();
            var startIndex = data.IndexOf("\"/");
            int count = 1;
            while (startIndex != -1)
            {
                var secondIndex = data.IndexOf(".html\"", startIndex);
                var link = $"{baseUrl}{data.Substring(startIndex + 1, secondIndex - startIndex + 4)}";
                if (!link.Contains("html"))
                {
                    MessageBox.Show($"Issue with link at {link} {count}");
                    return new List<string>();
                }

                links.Add(link);
                count++;
                startIndex = data.IndexOf("\"/", secondIndex + 1);
            }

            return links;
        }
    }
}
