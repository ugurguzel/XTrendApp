using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using XTrendApp.Web.Models.User;
using XTrendApp.Web.Repositories.User;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace XTrendApp.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserRepository _userRepository;

        public AccountController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user =
                await _userRepository.GetByUsernameAsync(model.Username);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError("", "This user is inactive. Please contact your system administrator..");
                return View(model);
            }

            if (!BCrypt.Net.BCrypt.Verify(
                    model.Password,
                    user.PasswordHash))
            {
                ModelState.AddModelError("", "Incorrect password.");
                return View(model);
            }

            // Son başarılı giriş zamanını güncelle
            //await _userRepository.UpdateLastLoginAsync(user.Id);

            // LastLogin güncellendiği için tekrar oku
            //user = await _userRepository.GetUserAsync(user.Id);

            //if (user == null)
            //{
            //    ModelState.AddModelError("", "User not found.");
            //    return View(model);
            //}

            // Oturumu oluştur
            //await SignInUserAsync(user, model.RememberMe);

            await _userRepository.UpdateLastLoginAsync(user.Id);

            await SignInUserAsync(user, model.RememberMe);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            int id = int.Parse(User.FindFirst("UserId")!.Value);

            var model = await _userRepository.GetProfileAsync(id);

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            //await _userRepository.UpdateProfileAsync(model);

            //TempData["Success"] = "Your profile has been updated successfully.";

            //return RedirectToAction(nameof(Profile));

            await _userRepository.UpdateProfileAsync(model);

            // Güncel kullanıcıyı tekrar oku
            var user = await _userRepository.GetUserAsync(model.Id);

            // Eski cookie'yi kaldır
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            // Claim'leri yeniden oluştur
            await SignInUserAsync(user!, false);

            TempData["Success"] = "Your profile has been updated successfully.";

            return RedirectToAction(nameof(Profile));
        }

        private async Task SignInUserAsync(UserModel user, bool rememberMe)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim("FullName", user.FullName ?? ""),
        new Claim("Email", user.Email ?? ""),
        new Claim("UserId", user.Id.ToString()),
        new Claim("IsAdmin", user.IsAdmin.ToString()),
        new Claim("LastLogin", user.LastLogin?.ToString("dd.MM.yyyy HH:mm") ?? "-")
    };

            if (user.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = rememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(
                        rememberMe ? 30 : 1)
                });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeMyPassword(ChangeMyPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Please fill in all required fields.");

            if (model.CurrentPassword == model.NewPassword)
            {
                return BadRequest("The new password must be different from your current password.");
            }

            var userId = int.Parse(User.FindFirst("UserId")!.Value);

            var result = await _userRepository.ChangeMyPasswordAsync(
                userId,
                model.CurrentPassword,
                model.NewPassword);

            if (!result)
                return BadRequest("Current password is incorrect.");

            await HttpContext.SignOutAsync(
    CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok();
        }
    }
}