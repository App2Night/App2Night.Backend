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
        public ActionResult GetPartys(Boolean loadAll = false)
        {
            List<JObject> jsonList = new List<JObject>();
            List<Party> partys;

            if (!loadAll)
            {
                //Select parties in future
                partys = _dbContext.PartyItems
                    .Where(p => p.PartyDate.Date >= DateTime.Today)
                    .Include(p => p.Location)
                    .Include(p => p.Host)
                    .ToList();
            }
            else
            {
                //Select all parties
                 partys = _dbContext.PartyItems
                    .Include(p => p.Location)
                    .Include(p => p.Host)
                    .ToList();
            }

            if (partys == null)
            {
                return NotFound("There are no partys.");
            }
            else
            {
                partys = partys.OrderByDescending(p => p.PartyDate).ToList();
                foreach (Party singleParty in partys)
                {
                    //Location + Host not as array
                    JObject tempJobject = new JObject();
                    tempJobject = new AddPartyJSON(_dbContext, User).AddCustomJsonForAdmin(singleParty);
                    tempJobject.Remove("MusicGenre");
                    tempJobject.Add("MusicGenre", singleParty.MusicGenre.ToString());
                    tempJobject.Remove("PartyType");
                    tempJobject.Add("PartyType", singleParty.PartyType.ToString());
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
        /// <param name="id"></param>
        /// <returns>Http Status Code 200 (Ok) and a list of UserParty-Items, or Http Status Code 404 (Not Found) if there are no Userparty-Items, 
        /// or Http Status Code 400 (Bad Request) if the UserId is not valid.</returns>
        [HttpGet("GetUserParties")]
        public ActionResult GetUserParties(Guid? id)
        {
            if(!Validator.IsGuidValid(id.ToString()))
            {
                return BadRequest("UserId is not valid.");
            }
            else
            {
                var userParties = _dbContext.UserPartyItems
                    .Include(p => p.Party)
                        .ThenInclude(p => p.Host)
                    .Include(p => p.Party)
                        .ThenInclude(p => p.Location)
                    .Include(u => u.User)
                    .Where(up => up.UserId == id)
                    .ToList();

                if(userParties == null || userParties.Count == 0)
                {
                    return NotFound("No UserParty-Items found.");
                }
                else
                {
                    List<JObject> jarray = new List<JObject>();

                    foreach(UserParty singleUP in userParties)
                    {
                        JObject partyJObject = new JObject();
                        partyJObject.Add("UserId", singleUP.UserId);
                        string user = singleUP.User.UserName + " " + singleUP.User.Email;
                        partyJObject.Add("User", user);
                        partyJObject.Add("PartyId", singleUP.PartyId);
                        string partyName = singleUP.Party.PartyName;
                        partyJObject.Add("PartyName", partyName);
                        string partyDate = singleUP.Party.PartyDate.ToString("dd.MM.yyyy hh:mm:ss");
                        partyJObject.Add("PartyDate", partyDate);
                        string locationString = singleUP.Party.Location.StreetName + " " + singleUP.Party.Location.HouseNumber + ", " + singleUP.Party.Location.Zipcode + " " + singleUP.Party.Location.CityName + ", " + singleUP.Party.Location.CountryName;
                        partyJObject.Add("Location", locationString);
                        string hostString = singleUP.Party.Host.UserName + " " + singleUP.Party.Host.Email;
                        partyJObject.Add("Host", hostString);
                        partyJObject.Add("EventCommitment", singleUP.EventCommitment.ToString());
                        partyJObject.Add("GeneralRating", singleUP.GeneralRating);
                        partyJObject.Add("PriceRating", singleUP.PriceRating);
                        partyJObject.Add("LocationRating", singleUP.LocationRating);
                        partyJObject.Add("MoodRating", singleUP.LocationRating);
                        jarray.Add(partyJObject);
                    }
                    return Ok(jarray);
                }
            }
        }
    }
}
