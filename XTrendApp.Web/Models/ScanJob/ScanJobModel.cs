namespace XTrendApp.Web.Models.ScanJob
{
    public class ScanJobModel
    {
        public int Id { get; set; }

        public string Code { get; set; } = "";

        public string Name { get; set; } = "";

        public string Source { get; set; } = "";

        public bool IsEnabled { get; set; }

        public bool IsRunning { get; set; }

        public DateTime? LastRun { get; set; }

        public DateTime? NextRun { get; set; }

        public int ProductLimit { get; set; } = 25;

        public int CurrentPage { get; set; } = 1;
    }
}