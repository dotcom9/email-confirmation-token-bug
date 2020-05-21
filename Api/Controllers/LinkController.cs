using Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinkController : ControllerBase
    {
        private ILogger<LinkController> Logger { get; }
        private UserManager<IdentityUser> UserManager { get; }
        private ApplicationDbContext DbContext { get; }
        public LinkController(ILogger<LinkController> logger, UserManager<IdentityUser> userManager, ApplicationDbContext dbContext)
        {
            Logger = logger;
            UserManager = userManager;
            DbContext = dbContext;
        }

        [HttpGet, Route("{userId}")]
        public async Task<string> GetLink(string userId)
        {
            var user = await DbContext.Users.FindAsync(userId);
            var token = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            Logger.LogInformation("Token sent: {0}", token);
            return token;
        }
    }
}