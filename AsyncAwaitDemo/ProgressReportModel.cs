namespace AsyncAwaitDemo
{
    public class ProgressReportModel
    {
        public int PercentageComplete { get; set; }

        public WebsiteDataModel DownloadedSite { get; set; }

        public ProgressReportModel()
        {
            PercentageComplete = 0;
            DownloadedSite = new WebsiteDataModel();
        }
    }
}
