using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XTrendApp.Web.Models.User;
using XTrendApp.Web.Services.User;

namespace XTrendApp.Web.Controllers.Administration
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllAsync();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Please fill in the required fields.");

            try
            {
                await _userService.InsertAsync(model);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _userService.DeactivateAsync(id);

                if (!result)
                    return BadRequest("This user is already inactive.");

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            try
            {
                var result = await _userService.ActivateAsync(id);

                if (!result)
                    return BadRequest("This user is already active.");

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return Json(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Please fill in the required fields.");

            try
            {
                var result = await _userService.UpdateAsync(model);

                if (!result)
                    return BadRequest("User could not be updated.");

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(UserPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Please enter a valid password.");

            try
            {
                var result = await _userService.ChangePasswordAsync(model);

                if (!result)
                    return BadRequest("Password could not be updated.");

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}