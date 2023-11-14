using GetNovels.Features;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Windows;

namespace GetNovels
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
            var folderPath = FileService.TryGetPath(StorageLocation.Text);
            if(string.IsNullOrEmpty(folderPath))
                return;

            if (!FileService.TryCreateDirectory(folderPath))
                return;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var links = LinkService.GetLinks(LinksBox.Text, WebsiteUrl.Text);
            if (!links.Any()) return;

            string filePath = $"{folderPath}\\{BookName.Text}.html";
            UrlService.ConvertUrlToHtml(filePath, links, ChapterCountLabel);
            DoneLabel.Visibility = Visibility.Visible;
            MessageBox.Show("Export Completed.");
        }
    }
}
