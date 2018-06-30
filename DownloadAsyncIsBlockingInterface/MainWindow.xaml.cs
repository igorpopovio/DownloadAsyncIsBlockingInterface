using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace DownloadAsyncIsBlockingInterface
{
    public partial class MainWindow
    {
        const string Url = "https://apod.nasa.gov/apod/image/1806/IMG_5938Mauduit_2048.jpg";
        const string File = "file.jpg";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            button.IsEnabled = false;

            var tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                // tasks.Add(IoBoundOperationBadAsync(i));  // 29 seconds, 10 worker threads
                // tasks.Add(IoBoundOperationGoodAsync(i)); // 24 seconds,  0 worker threads
                // tasks.Add(CpuBoundOperationGoodAsync()); // 12 seconds,  8 worker threads
            }

            var watch = Stopwatch.StartNew();

            await Task.WhenAll(tasks);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            button.IsEnabled = true;
            MessageBox.Show(this,
                $"Async task completed in {elapsedMs / 1000} seconds!",
                "Information",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private Task CpuBoundOperationGoodAsync()
        {
            return Task.Run(() =>
            {
                for (int i = 0; i < 99999999; i++) Math.Sin(i);
            });
        }

        private Task IoBoundOperationGoodAsync(int i)
        {
            return new WebClient().DownloadFileTaskAsync(Url, i + File);
        }

        private Task IoBoundOperationBadAsync(int i)
        {
            return Task.Run(() =>
            {
                new WebClient().DownloadFile(Url, i + File);
            });
        }

        private static void WcDownload()
        {
            var wc = new WebClient();
            wc.DownloadFileCompleted += (s, args) =>
            {
                MessageBox.Show("Download completed!!!");
            };
            wc.DownloadFileAsync(new Uri("https://apod.nasa.gov/apod/image/1806/IMG_5938Mauduit_2048.jpg"), @"file.jpg");
        }

        private async void MyDownload()
        {
            await Task.Run(() =>
            {
                var wc = new WebClient();
                wc.DownloadFile(new Uri("https://apod.nasa.gov/apod/image/1806/IMG_5938Mauduit_2048.jpg"), @"file.jpg");
            });
            MessageBox.Show("Download completed!!!");
        }
    }
}
