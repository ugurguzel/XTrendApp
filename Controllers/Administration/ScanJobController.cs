using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XTrendApp.Web.Services.ScanJob;

namespace XTrendApp.Web.Controllers.Administration
{
    [Authorize(Roles = "Admin")]
    public class ScanJobController : Controller
    {
        private readonly ScanJobService _scanJobService;

        public ScanJobController(ScanJobService scanJobService)
        {
            _scanJobService = scanJobService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetList()
        {
            return Json(_scanJobService.GetAll());
        }

        [HttpPost]
        public async Task<IActionResult> Run(string code)
        {
            try
            {
                await _scanJobService.RunAsync(code);

                return Json(new
                {
                    success = true,
                    message = "Scan job completed successfully."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}