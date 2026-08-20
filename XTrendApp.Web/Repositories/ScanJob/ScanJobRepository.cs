using XTrendApp.Web.Models.ScanJob;

namespace XTrendApp.Web.Repositories.ScanJob
{
    public class ScanJobRepository
    {
        public List<ScanJobModel> GetAll()
        {
            return new List<ScanJobModel>
            {
                new ScanJobModel
                {
                    Id = 1,
                    Code = "AMAZON_US",
                    Name = "Amazon US",
                    Source = "Amazon",
                    IsEnabled = true
                },

                new ScanJobModel
                {
                    Id = 2,
                    Code = "AMAZON_UK",
                    Name = "Amazon UK",
                    Source = "Amazon",
                    IsEnabled = true
                },

                new ScanJobModel
                {
                    Id = 3,
                    Code = "WAYFAIR",
                    Name = "Wayfair",
                    Source = "Wayfair",
                    IsEnabled = true
                }
            };
        }
    }
}