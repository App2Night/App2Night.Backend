using App2NightAPI.Models;
using App2NightAPI.Models.Authentification;
using App2NightAPI.Models.REST_Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace App2NightAPI.Controllers
{
    [Route("api/User")]
    public class UserController : Controller 
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger _logger;
        private DatabaseContext _dbContext;

        public UserController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILoggerFactory loggerFactory,
            DatabaseContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<UserController>();

            _dbContext = dbContext;
        }

        // GET api/User
        [HttpGet]
        public IEnumerable<string> Get()
        {
            //try
            //{
            //    Guid userId;
            //    if (Guid.TryParse(id.ToString(), out userId))
            //    {
            //        var user = _dbContext.UserItems
            //            .Include(l => l.Location)
            //            .Include(u => u.PartyHostedByUser)
            //            .First<User>(u => u.UserId == id);

            //        if (user != null)
            //        {
            //JObject userJson = JObject.FromObject(user);
            //            foreach (var p in user.PartyHostedByUser)
            //            {
            //                userJson.Add("partId", p.PartId);
            //            }
            //        }
            //        else
            //        {
            //            //return BadRequest();
            //        }
            //    }
            //    else
            //    {
            //        //return BadRequest();
            //    }
            //}
            //catch (Exception)
            //{
            //    //return BadRequest();
            //}
            return new string[] { "user..."};
        }

        //  POST /api/User
        [HttpPost]
        public async Task<ActionResult> Register([FromBody]Login value)
        {
            var user = new User { UserName = value.Username, Email = value.Email };
            var result = await _userManager.CreateAsync(user, value.Password);

            return Ok();
        }
    }
}
