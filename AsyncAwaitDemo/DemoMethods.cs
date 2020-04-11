using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitDemo
{
    internal static class DemoMethods
    {
        public static List<string> PrepareData()
        {
            List<string> output = new List<string>
            {
                "https://www.yahoo.com",
                "https://www.google.com",
                "https://www.microsoft.com",
                "https://www.amazon.com",
                "https://www.facebook.com",
                "https://www.twitter.com",
                "https://www.codeproject.com",
                "https://www.stackoverflow.com"
            };

            return output;
        }

        public static List<WebsiteDataModel> RunDownloadSync()
        {
            var output = new List<WebsiteDataModel>();

            List<string> websites = PrepareData();

            foreach (string site in websites)
            {
                WebsiteDataModel results = DownloadWebsite(site);
                output.Add(results);
            }

            return output;
        }

        public static List<WebsiteDataModel> RunDownloadSyncParallel()
        {
            var output = new List<WebsiteDataModel>();

            List<string> websites = PrepareData();

            Parallel.ForEach(websites, (site) =>
            {
                WebsiteDataModel results = DownloadWebsite(site);
                output.Add(results);
            });

            return output;
        }

        public static async Task<List<WebsiteDataModel>> RunDownloadAsync(IProgress<ProgressReportModel> progress, CancellationToken cts)
        {
            var output = new List<WebsiteDataModel>();
            var report = new ProgressReportModel();

            List<string> websites = PrepareData();

            foreach (string site in websites)
            {
                WebsiteDataModel result = await DownloadWebsiteAsync(site);
                output.Add(result);

                cts.ThrowIfCancellationRequested();

                report.PercentageComplete = (output.Count * 100) / websites.Count;
                report.DownloadedSite = result;

                progress.Report(report);
            }

            return output;
        }

        public static async Task<List<WebsiteDataModel>> RunDownloadAsyncParallel(IProgress<ProgressReportModel> progress)
        {
            var output = new List<WebsiteDataModel>();
            var report = new ProgressReportModel();

            List<string> websites = PrepareData();

            await Task.Run(() =>
            {
                Parallel.ForEach(websites, (site) =>
                {
                    WebsiteDataModel result = DownloadWebsite(site);
                    output.Add(result);

                    report.PercentageComplete = (output.Count * 100) / websites.Count;
                    report.DownloadedSite = result;

                    progress.Report(report);
                });
            });

            return new List<WebsiteDataModel>(output);
        }

        private static async Task<WebsiteDataModel> DownloadWebsiteAsync(string websiteURL)
        {
            var output = new WebsiteDataModel();
            var client = new WebClient();

            output.WebsiteUrl = websiteURL;
            output.WebsiteData = await client.DownloadStringTaskAsync(websiteURL);

            return output;
        }

        private static WebsiteDataModel DownloadWebsite(string websiteURL)
        {
            var output = new WebsiteDataModel();
            var client = new WebClient();

            output.WebsiteUrl = websiteURL;
            output.WebsiteData = client.DownloadString(websiteURL);

            return output;
        }
    }
}