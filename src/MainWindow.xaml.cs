using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConvertUrlToHtmlBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(StorageLocation.Text))
            {
                MessageBox.Show("Please enter Storage Location.");
                return;
            }

            if (string.IsNullOrEmpty(BookName.Text))
            {
                MessageBox.Show("Please enter Book Name.");
                return;
            }

            if (BookName.Text.Contains(':'))
            {
                MessageBox.Show("Please remove ':' from book name.");
                return;
            }

            if (string.IsNullOrEmpty(WebsiteUrl.Text))
            {
                MessageBox.Show("Please enter Website URL.");
                return;
            }

            DoneLabel.Visibility = Visibility.Hidden;
            var folderPath = $"{StorageLocation.Text}";
            try
            {
               folderPath = Path.GetFullPath(folderPath);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Invalid folder path {folderPath}. Exception: {ex.Message}");
                return;
            }

            try
            {
                Directory.CreateDirectory(folderPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message} Failed at creating Directory.");
                return;
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var baseUrl = WebsiteUrl.Text;
            var links = new List<string>();
            var data = LinksBox.Text;
            var startIndex = data.IndexOf("\"/");
            int count = 1;
            while (startIndex != -1)
            {
                var secondIndex = data.IndexOf(".html\"", startIndex);
                var link = $"{baseUrl}{data.Substring(startIndex + 1, secondIndex - startIndex + 4)}";
                if (!link.Contains("html"))
                {
                    MessageBox.Show($"Issue with link at {link} {count}");
                    return;
                }

                links.Add(link);
                count++;
                startIndex = data.IndexOf("\"/", secondIndex + 1);
            }

            ConvertUrlToDocx(folderPath, links);
            DoneLabel.Visibility = Visibility.Visible;
            MessageBox.Show("Export Completed.");

        }

        private void ConvertUrlToDocx(string folderPath, List<string> links)
        {
            var rowCount = links.Count;
            ChapterCountLabel.Content = $"Converted Chapter Count: 1 of {rowCount}";
            int retryCount = 100;
            int chapterCount = 1;
            var filePath = $"{folderPath}\\{BookName.Text}.html";

            string startString = "<!DOCTYPE html><html lang=\"en-US\">";
            File.WriteAllText(filePath, startString);
            using HttpClient client = new();
            foreach (var link in links)
            {
                var reTriedCount = 1;
                while (reTriedCount <= retryCount)
                {
                    try
                    {
                        using HttpResponseMessage response = client.GetAsync(link).GetAwaiter().GetResult();
                        if (response.IsSuccessStatusCode)
                        {
                            WriteToFile(filePath, response);
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed at getting url with message {ex.Message}.");
                        return;
                    }
                    reTriedCount++;
                    Thread.Sleep(2000);
                }

                if (reTriedCount >= retryCount)
                {
                    MessageBox.Show($"Failed at saving url to html.");
                    return;
                }

                chapterCount++;
                ChapterCountLabel.Content = $"Converted Pages Count: {chapterCount} of {rowCount}";
            }

            string endString = "</html>";
            File.AppendAllText(filePath, endString);
        }

        private static void WriteToFile(string filePath, HttpResponseMessage response)
        {
            using HttpContent content = response.Content;
            string result = content.ReadAsStringAsync().GetAwaiter().GetResult();
            result = RemoveJunk(result);
            File.AppendAllText(filePath, result);
        }

        private static string RemoveJunk(string result)
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
                ("<!-- TODO", ">")
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
