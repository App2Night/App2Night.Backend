using App2NightAPI.Models;
using App2NightAPI.Models.Authentification;
using App2NightAPI.Models.REST_Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App2NightAPI.Controllers
{
    [Route("api/User")]
    public class UserController : Controller 
    {
        private DatabaseContext _dbContext;
        public UserController(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET api/User
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "user..."};
        }

        [HttpPost]
        public ActionResult Register([FromBody]Login value)
        {
            var user = new User
            {
                Username = value.Username,
                Password = value.Password
            };
            _dbContext.UserItems.Add(user);
            _dbContext.SaveChanges();
            return Created("", user.UserId);
        }
    }
}
