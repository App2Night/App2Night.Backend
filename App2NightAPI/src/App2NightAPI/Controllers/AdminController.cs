using App2NightAPI.Models;
using App2NightAPI.Models.Enum;
using App2NightAPI.Models.REST_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App2NightAPI.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/admin")]
    [Authorize(Roles = "Administrator")]
    public class AdminController : CustomController
    {
        private DatabaseContext _dbContext;
        public AdminController(DatabaseContext dbContext, IUserService userService) : base(dbContext, userService)
        {
            _dbContext = dbContext;
        }

        [HttpGet("GetPartys")]
        public ActionResult GetPartys()
        {
            List<JObject> jsonList = new List<JObject>();

            //Select all parties for admin client
            var partys = _dbContext.PartyItems
                .Include(p => p.Location)
                .Include(p => p.Host).ToList();

            if (partys == null)
            {
                return NotFound("There are no partys.");
            }
            else
            {
                foreach (Party singleParty in partys)
                {
                    //Location + Host not as array
                    JObject tempJobject = new JObject();
                    tempJobject = new AddPartyJSON(_dbContext, User).AddCustomJson(singleParty);
                    tempJobject.Remove("Location");
                    tempJobject.Remove("Host");
                    string locationString = singleParty.Location.StreetName + " " + singleParty.Location.HouseNumber + ", " + singleParty.Location.Zipcode + " " + singleParty.Location.CityName + ", " + singleParty.Location.CountryName;
                    tempJobject.Add("Location", locationString);
                    string hostString = singleParty.Host.UserName + " " + singleParty.Host.Email;
                    tempJobject.Add("Host", hostString);

                    jsonList.Add(tempJobject);
                }
            }
                return Ok(jsonList);   
        }

        [HttpGet("GetUser")]
        public ActionResult GetUser()
        {
            //Select all users
            var users = _dbContext.UserItems.ToList();

            if(users == null || users.Count == 0)
            {
                return BadRequest("No users found.");
            }
            else
            {
                return Ok(users);
            }
        }
    }
}
