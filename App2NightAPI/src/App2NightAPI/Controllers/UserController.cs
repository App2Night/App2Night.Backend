using App2NightAPI.Models;
using App2NightAPI.Models.Authentification;
using App2NightAPI.Models.REST_Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

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
            //            JObject userJson = JObject.FromObject(user);
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

        [HttpPost]
        public ActionResult Register([FromBody]Login value)
        {
            //var user = new User
            //{
            //    Username = value.Username,
            //    Password = value.Password
            //};
            //_dbContext.UserItems.Add(user);
            //_dbContext.SaveChanges();
            //return Created("", user.UserId);
            return Ok();
        }
    }
}
