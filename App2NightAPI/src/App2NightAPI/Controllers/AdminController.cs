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
    /// <summary>
    /// Administrator Controller for the Admin-Panel. All routes are only accessible with the administrator user.
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/admin")]
    [Authorize(Roles = "Administrator")]
    public class AdminController : CustomController
    {
        private DatabaseContext _dbContext;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">Database Context</param>
        /// <param name="userService">User Service for interaction with User Server</param>
        public AdminController(DatabaseContext dbContext, IUserService userService) : base(dbContext, userService)
        {
            _dbContext = dbContext;
        }

        // GET /api/admin/getpartys
        /// <summary>
        /// Modified GET-Party route.
        /// </summary>
        /// <returns>Functions returns all parties with location and host.</returns>
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
                    tempJobject = new AddPartyJSON(_dbContext, User).AddCustomJsonForAdmin(singleParty);
                    tempJobject.Remove("CommittedUser");
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

        // PUT /api/admin/modifyparty
        /// <summary>
        /// Modified PUT-Party route. Not only host is able to change the party.
        /// </summary>
        /// <param name="id">PartyId</param>
        /// <param name="value">CreateParty-Object</param>
        /// <returns>Http Status Code 200 (Ok), or Http Status Code 400 (Bad Request), or Http Status Code 404 (Not Found) if Party ID don't exists.</returns>
        [HttpPut("modifyParty")]
        public ActionResult Modify(Guid? id, [FromBody]CreateParty value)
        {
            try
            {
                //Check if Party Id is valid
                if (!Validator.IsGuidValid(id.ToString()))
                {
                    //Can't parse Party ID
                    return BadRequest("Party ID is not valid.");
                }
                else
                {
                    var party = _dbContext.PartyItems
                        .Include(p => p.Host)
                        .Include(p => p.Location)
                        .FirstOrDefault(p => p.PartyId == id);

                    if (party == null)
                    {
                        return NotFound("Party not found.");
                    }
                    else if (!TryValidateModel(party))
                    {
                        return BadRequest(new CreateParty());
                    }
                    else
                    {
                        //Party is valid.
                        new Helper(User).MapPartyToModel(value, ref party);

                        _dbContext.PartyItems.Update(party);
                        _dbContext.SaveChanges();
                        return Ok();
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        //DELETE /api/admin/deleteparty
        /// <summary>
        /// Modified DELETE-Party route. Not only the host can delete parties and parties in the past can be deleted.
        /// </summary>
        /// <param name="id">PartyId</param>
        /// <returns>Http Status Code 200 (Ok), or Http Status Code 400 (Bad Request), or Http Status Code 404 (Not Found) if Party ID don't exists.</returns>
        [HttpDelete("deleteParty")]
        public ActionResult Delete(Guid? id)
        {
            try
            {
                //Check if Party Id is valid
                if (!Validator.IsGuidValid(id.ToString()))
                {
                    //Can't parse Party ID
                    return BadRequest("Party ID is not valid.");
                }
                else
                {
                    var party = _dbContext.PartyItems
                        .Include(p => p.Host)
                        .Include(p => p.Location)
                        .FirstOrDefault(p => p.PartyId == id);

                    //Check if party exists
                    if (party == null)
                    {
                        return NotFound("Party not found.");
                    }
                    else
                    {
                        //Party is valid.
                        //Delete from Database
                        _dbContext.Entry(party).State = EntityState.Deleted;
                        _dbContext.Entry(party.Location).State = EntityState.Deleted;
                        _dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
            return Ok();
        }

        // GET /api/admin/getuser
        /// <summary>
        /// Functions returns the all registered users.
        /// </summary>
        /// <returns>Http Status Code 200 (Ok) and a list of users, or Http Status Code 404 (Not Found) if there are no users.</returns>
        [HttpGet("GetUser")]
        public ActionResult GetUser()
        {
            //Select all users
            var users = _dbContext.UserItems.ToList();

            if(users == null || users.Count == 0)
            {
                return NotFound("No users found.");
            }
            else
            {
                return Ok(users);
            }
        }

        /// <summary>
        /// Function retunrs all the UserParty-Items to a given User.
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns>Http Status Code 200 (Ok) and a list of UserParty-Items, or Http Status Code 404 (Not Found) if there are no Userparty-Items, 
        /// or Http Status Code 400 (Bad Request) if the UserId is not valid.</returns>
        [HttpGet("GetUserParties")]
        public ActionResult GetUserParties(Guid? UserId)
        {
            if(!Validator.IsGuidValid(UserId.ToString()))
            {
                return BadRequest("UserId is not valid.");
            }
            else
            {
                var userParties = _dbContext.UserPartyItems
                    .Select(up => up.UserId == UserId)
                    .ToList();

                if(userParties == null || userParties.Count == 0)
                {
                    return NotFound("No UserParty-Items found.");
                }
                else
                {
                    return Ok(userParties);
                }
            }
        }
    }
}
