using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

#pragma warning disable IDE1006 // Naming Styles

namespace AsyncAwaitDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void executeSync_Click(object sender, RoutedEventArgs e)
        {
            ClearTextBox();
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var results = DemoMethods.RunDownloadSync();
            PrintResults(results);

            watch.Stop();

            var elapsedMs = watch.ElapsedMilliseconds;
            resultsWindow.Text += $"\nTotal execution time: { elapsedMs }";
        }

        private void executeSyncParallel_Click(object sender, RoutedEventArgs e)
        {
            ClearTextBox();
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var results = DemoMethods.RunDownloadSyncParallel();
            PrintResults(results);

            watch.Stop();

            var elapsedMs = watch.ElapsedMilliseconds;
            resultsWindow.Text += $"\nTotal execution time: { elapsedMs }";
        }

        private async void executeAsync_Click(object sender, RoutedEventArgs e)
        {
            ClearTextBox();

            var progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += OnProgressChanged;

            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var results = await DemoMethods.RunDownloadAsync(progress, cts.Token);
            }
            catch (OperationCanceledException)
            {
                resultsWindow.Text += $"\nOperation was cancelled by user.{ Environment.NewLine }";
            }

            watch.Stop();

            var elapsedMs = watch.ElapsedMilliseconds;
            resultsWindow.Text += $"\nTotal execution time: { elapsedMs }";
        }

        private async void executeAsyncParallel_Click(object sender, RoutedEventArgs e)
        {
            ClearTextBox();
            var progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += OnProgressChanged;

            var watch = System.Diagnostics.Stopwatch.StartNew();

            var results = await DemoMethods.RunDownloadAsyncParallel(progress);
            PrintResults(results);

            watch.Stop();

            var elapsedMs = watch.ElapsedMilliseconds;
            resultsWindow.Text += $"\nTotal execution time: { elapsedMs }";
        }

        private void cancelOperation_Click(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
        }

        private void OnProgressChanged(object sender, ProgressReportModel e)
        {
            progressBar.Value = e.PercentageComplete;
            PrintResult(e.DownloadedSite);
        }

        private void ClearTextBox()
        {
            resultsWindow.Text = string.Empty;
        }

        private void PrintResult(WebsiteDataModel result)
        {
            resultsWindow.Text += $"{ result.WebsiteUrl } downloaded: { result.WebsiteData.Length } characters long.{ Environment.NewLine }";
        }

        private void PrintResults(List<WebsiteDataModel> results)
        {
            ClearTextBox();

            foreach (var item in results)
            {
                resultsWindow.Text += $"{ item.WebsiteUrl } downloaded: { item.WebsiteData.Length } characters long.{ Environment.NewLine }";
            }
        }
    }
}
