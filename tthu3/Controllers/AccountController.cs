using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using tthu3.Models;

namespace tthu3.Controllers
{
    public class AccountController : Controller
    {
        // Simple in-memory users for demo purposes
        // Username/password are plain text here for simplicity.
        // Replace with secure user store + hashed passwords for production.
        private static readonly List<UserAccount> _users = new()
        {
            new UserAccount { Username = "admin", Password = "admin123", Role = "Admin", DisplayName = "Quản trị viên" },
            new UserAccount { Username = "tenant1", Password = "tenant123", Role = "Tenant", DisplayName = "Người thuê 1", TenantId = "T001" }
        };

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            var user = _users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.DisplayName ?? user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            if (!string.IsNullOrEmpty(user.TenantId))
            {
                claims.Add(new Claim("TenantId", user.TenantId));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "PhongTro");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();
    }
}