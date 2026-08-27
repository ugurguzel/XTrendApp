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

        [HttpPost]
        public async Task<IActionResult> UpdateProductLimit(
    int id,
    int productLimit)
        {
            try
            {
                if (productLimit < 1 || productLimit > 100)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Product limit must be between 1 and 100."
                    });
                }

                var updated =
                    await _scanJobService.UpdateProductLimitAsync(
                        id,
                        productLimit);

                if (!updated)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Scan job could not be updated."
                    });
                }

                return Json(new
                {
                    success = true,
                    message = "Product limit updated successfully."
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