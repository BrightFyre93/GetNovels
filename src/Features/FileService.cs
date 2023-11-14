using System;
using System.IO;
using System.Net.Http;
using System.Windows;

namespace GetNovels.Features
{
    public static class FileService
    {
        public static void WriteToFile(string filePath, HttpResponseMessage response)
        {
            using HttpContent content = response.Content;
            string result = content.ReadAsStringAsync().GetAwaiter().GetResult();
            result = HttpStringService.RemoveJunk(result);
            File.AppendAllText(filePath, result);
        }

        public static string TryGetPath(string folderPath)
        {
            string path;
            try
            {
                path = Path.GetFullPath(folderPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Invalid folder path {folderPath}. Exception: {ex.Message}");
                return "";
            }

            return path;
        }

        public static bool TryCreateDirectory(string folderPath)
        {
            try
            {
                Directory.CreateDirectory(folderPath);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message} Failed at creating Directory.");
                return false;
            }
        }
    }
}
