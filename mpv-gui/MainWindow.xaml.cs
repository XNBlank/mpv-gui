using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Shapes;

namespace mpv_gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            checkPath();
        }

        private async void checkPath()
        {
            bool hasMPV = ExistsOnPath("mpv.exe");
            bool hasYTDL = ExistsOnPath("youtube-dl.exe");
            Download download = new Download();

            if ( !hasMPV )
            {
                MessageBoxResult result = MessageBox.Show("MPV is not installed. Would you like to download it?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    download.setTitle("Downloading MPV");
                    await download.start("https://mpv.srsfckn.biz/mpv-x86_64-20181002.7z", "mpv.7z", true);
                }
            }

            Download download2 = new Download();

            if ( !hasYTDL )
            {
                MessageBoxResult result = MessageBox.Show("YoutubeDL is not installed and is needed to play streamed content from sites like Youtube and Twitch. Would you like to download it?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    download2.setTitle("Downloading Youtube-DL");
                    await download2.start("https://youtube-dl.org/downloads/latest/youtube-dl.exe", "youtube-dl.exe", false);
                }
            }

        }


        private void OnExit(object sender, ExitEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        Console console = new Console();
        Options options = new Options();
        public Process p;

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            string url = urlBox.Text;

            p = new Process();

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.FileName = "CMD.EXE";

            p.StartInfo.CreateNoWindow = true;

            if (checkFullscreen.IsChecked == true)
            {
                url += " --fs";
                p.StartInfo.CreateNoWindow = true;
            }

            if (checkNoAudio.IsChecked == true)
            {
                url += " --no-audio";
                p.StartInfo.CreateNoWindow = true;
            }

            if (checkNoVideo.IsChecked == true)
            {
                url += " --no-video";
                p.StartInfo.CreateNoWindow = false;
            }

            if (options.vidAutoCenterCheck.IsChecked == true)
            {
                url += String.Format(" --geometry={0}x{1}+50%+50%",Properties.Settings.Default.vidWidth,Properties.Settings.Default.vidHeight);
            }
            else if (options.vidAutoCenterCheck.IsChecked == false)
            {
                url += String.Format(" --geometry={0}x{1}+{2}+{3}", Properties.Settings.Default.vidWidth, Properties.Settings.Default.vidHeight, Properties.Settings.Default.vidPosX, Properties.Settings.Default.vidPosY);
            }


            // /c mpv
            p.StartInfo.Arguments = "/c mpv " + url;
            //this.Hide();
            p.Start();

            if (console.IsVisible)
            {
                p.OutputDataReceived += new DataReceivedEventHandler(MyProcOutputHandler);

                p.ErrorDataReceived += new DataReceivedEventHandler(MyProcErrorHandler);
            }

            
            p.BeginErrorReadLine();
            p.BeginOutputReadLine();


            //this.Show();
        }

        private void browseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fs = new OpenFileDialog();
            string path = string.Empty;
            fs.Title = "Open Video File";
            fs.DefaultExt = ".mp4";
            fs.Filter = "MP4 Files|*.mp4|AVI Files|*.avi";
            fs.InitialDirectory = @"C:\";
            Nullable<bool> result = fs.ShowDialog();

            if (result == true)
            {
                path = fs.FileName;
                urlBox.Text = path;
            }

        }

        private void pasteButton_Click(object sender, RoutedEventArgs e)
        {
            string tmp = Clipboard.GetText();
            if(tmp == "")
            {
                MessageBoxResult box = MessageBox.Show("Clipboard did not contain a string.", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
            } else
            {
                urlBox.Text = tmp;
            }
        }

        private void buttonAbout_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.Show();
        }

        private void buttonConsole_Click(object sender, RoutedEventArgs e)
        {
            //console = new Console();
            console.Show();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            urlBox.Text = "";
        }

        private void buttonOptions_Click(object sender, RoutedEventArgs e)
        {
            options.Show();
        }

        private void MyProcOutputHandler(object sendingProcess,
                    DataReceivedEventArgs ev)
        {
            if (!String.IsNullOrEmpty(ev.Data))
            {
                Debug.WriteLine(ev.Data);
                string output = ev.Data;

                if ((ev.Data.Contains("AV:")) || (ev.Data.Contains("V:")) || (ev.Data.Contains("A:")))
                {

                    this.Dispatcher.Invoke(() =>
                    {
                        console.consoleBox.Text = console.consoleBox.Text.Remove(console.consoleBox.Text.LastIndexOf(Environment.NewLine));
                        console.consoleBox.Text += output + "\r\n";
                        console.consoleBox.ScrollToEnd();
                    });
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        
                        console.consoleBox.Text += output + "\r\n";
                        console.consoleBox.Text = console.consoleBox.Text.Remove(console.consoleBox.Text.LastIndexOf(Environment.NewLine));
                        console.consoleBox.ScrollToEnd();
                    });
                }


            }
            else
            {
                p.CancelErrorRead();
                p.CancelOutputRead();
                p.Close();
            }
        }


        private void MyProcErrorHandler(object sendingProcess,
                    DataReceivedEventArgs ev)
        {
            if (!String.IsNullOrEmpty(ev.Data))
            {
                Debug.WriteLine(ev.Data);
                string errorout = ev.Data;

                this.Dispatcher.Invoke(() =>
                {
                    console.consoleBox.Text += errorout + "\r\n";
                    console.consoleBox.ScrollToEnd();
                });

            }
            else
            {
                p.CancelErrorRead();
                p.CancelOutputRead();
                p.Close();
            }
        }

        public static bool ExistsOnPath(string fileName)
        {
            return GetFullPath(fileName) != null;
        }

        public static string GetFullPath(string fileName)
        {
            if (File.Exists(fileName))
                return System.IO.Path.GetFullPath(fileName);

            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(';'))
            {
                var fullPath = System.IO.Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                    return fullPath;
            }
            return null;
        }


    }
}
