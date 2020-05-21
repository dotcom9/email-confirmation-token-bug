using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReproduceIssue.Models;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace ReproduceIssue.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private Guid _userId = new Guid("8d768d7d-24d5-45b0-ae77-356a01c94b02");

        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetLocalAsync()
        {
            var user = await _userManager.FindByIdAsync(_userId.ToString());
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await ConfirmEmail(user, token);
            return View("Index");
        }

        public async Task<IActionResult> GetApiAsync()
        {
            var url = $"http://localhost:62990/api/link/{_userId}";
            var client = new HttpClient();
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Token received: {0}", token);
                var user = await _userManager.FindByIdAsync(_userId.ToString());
                await ConfirmEmail(user, token);
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, message);
            }
            return View("Index");
        }

        private async Task ConfirmEmail(IdentityUser user, string token)
        {
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
