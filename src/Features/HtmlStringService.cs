using System;
using System.Collections.Generic;
using System.Windows;

namespace GetNovels.Features
{
    public static class HtmlStringService
    {
        public static string RemoveJunk(string result)
        {
            result = result.Replace("</html>", "");
            result = result.Replace("<!DOCTYPE html><html lang=\"en-US\">", "");

            var tagsToRemove = new List<(string Start, string End)>()
            {
                ("<a", "</a>"),
                ("<button", "</button>"),
                ("<form", "</form>"),
                ("<ul class=\"control nav navbar-nav \"", "</ul>"),
                ("<ul class=\"dropdown-menu\"", "</ul>"),
                ("ol class=\"breadcrumb\"", "</ol>"),
                ("<script", "</script>"),
                ("<meta", ">"),
                ("<link", ">"),
                ("<!-- Google tag", ">"),
                ("<!-- TODO", ">"),
                ("<iframe", "</iframe>")
            };
            try
            {
                foreach (var (Start, End) in tagsToRemove)
                {
                    while (result.Contains(Start))
                    {
                        var index = result.IndexOf(Start);
                        var closeIndex = result.IndexOf(End, index);
                        result = result.Remove(index, closeIndex - index + End.Length);
                    }
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show($"Exception: {ex.Message}. Trying to remove tags failed.");
            }

            return result;
        }
    }
}
