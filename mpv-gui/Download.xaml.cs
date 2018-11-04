using System.Windows;
using System.IO;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using SevenZipExtractor;
using mpv_gui;

namespace mpv_gui
{
    /// <summary>
    /// Interaction logic for Download.xaml
    /// </summary>
    public partial class Download : Window
    {
        public Download()
        {
            InitializeComponent();
        }

        string filename;

        public void setTitle(string title)
        {
            downloadLabel.Content = title;
            Title = title;
            Show();
        }

        public async Task start (string download, string filename, bool isArchive)
        {
            this.filename = filename;
            var destPath = Path.GetFullPath(filename);
            var extracted = false;
            var client = new HttpClientDownloadWithProgress(download, destPath);

            client.ProgressChanged += async (fileSize, downloadedSize, percent) =>
            {
                if (downloadProgress.IsIndeterminate)
                    downloadProgress.IsIndeterminate = false;

                downloadBytes.Content = downloadedSize + "/" + fileSize;
                downloadProgress.Value = (double)percent;

                if ( downloadedSize >= fileSize)
                {
                    Trace.WriteLine(isArchive);
                    if (isArchive && !extracted)
                    {
                        extracted = true;
                        downloadLabel.Content = "Extracting " + filename;
                        downloadBytes.Content = "";
                        downloadProgress.IsIndeterminate = true;
                        await Task.Delay(1000);
                        await extractDownload();
                    }
                    else
                    {
                        Close();
                    }
                }
            };

            await client.StartDownload();
        }

        private async Task extractDownload()
        {
            await Task.Yield();
            using (ArchiveFile archive = new ArchiveFile(filename))
            {
                archive.Extract(Directory.GetCurrentDirectory(),true);
            }
            Close();
        }


    }
}
