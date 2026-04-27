using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Models;
using PortfolioApp.Services;
using System.Security.Claims;

namespace PortfolioApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AccountController> _logger;

        // Simple lockout: in-memory
        private static readonly Dictionary<string, (int Attempts, DateTime LockUntil)> _loginAttempts = new();
        private const int MaxAttempts = 5;
        private static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(15);

        public AccountController(IAuthService authService, ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Portfolio");

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var key = model.Username.Trim().ToLowerInvariant();

            // protection 
            if (_loginAttempts.TryGetValue(key, out var attempt) && attempt.LockUntil > DateTime.UtcNow)
            {
                var remaining = (int)(attempt.LockUntil - DateTime.UtcNow).TotalMinutes + 1;
                ModelState.AddModelError(string.Empty, $"Account locked. Try again in {remaining} minute(s).");
                return View(model);
            }

            var valid = await _authService.ValidateUserAsync(model.Username, model.Password);

            if (!valid)
            {
                _loginAttempts.TryGetValue(key, out var prev);
                var newAttempts = prev.Attempts + 1;
                var lockUntil = newAttempts >= MaxAttempts
                    ? DateTime.UtcNow.Add(LockDuration)
                    : DateTime.MinValue;
                _loginAttempts[key] = (newAttempts, lockUntil);

                _logger.LogWarning("Failed login for user {Username} (attempt {Attempt})", key, newAttempts);
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            // Clear failed attempts on success
            _loginAttempts.Remove(key);
            _logger.LogInformation("Successful login for user {Username}", key);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, model.Username),
                new(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProps = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(7)
                    : DateTimeOffset.UtcNow.AddHours(1)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, principal, authProps);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("Index", "Portfolio");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
