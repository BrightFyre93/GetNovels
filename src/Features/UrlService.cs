using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Windows;
using System;
using System.Windows.Controls;
using System.IO;

namespace GetNovels.Features
{
    public static class UrlService
    {
        public static void ConvertUrlToHtml(string filePath, List<string> links, Label ChapterCountLabel)
        {
            var rowCount = links.Count;
            ChapterCountLabel.Content = $"Converted Chapter Count: 1 of {rowCount}";
            int retryCount = 100;
            int chapterCount = 1;

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
                            FileService.WriteToFile(filePath, response);
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
    }
}
