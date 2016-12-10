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
                    //Check if Party Date is toady or in future
                    else if (value.PartyDate <= DateTime.Today)
                    {
                        //Party Date is not today or in future
                        return BadRequest("Party has to be in the future.");
                    }
                    else if (!TryValidateModel(party))
                    {
                        return BadRequest(new CreateParty());
                    }
                    else
                    {
                        //Party is valid.
                        MapPartyToModel(value, ref party);

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

        [HttpDelete("deleteParty")]
        public ActionResult Delete(Guid? id)
        {
            Guid partyId;
            try
            {
                //Check if Party Id is valid
                if (!Guid.TryParse(id.ToString(), out partyId))
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
                    //Check if the Date of the party is valid.
                    else if (party.PartyDate < DateTime.Now)
                    {
                        return BadRequest("Can't delete. Party date is in the past.");
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

        //TODO Seperate class for help functions!!!
        private void MapPartyToModel(CreateParty value, ref Party party)
        {
            Location loc = null;
            Task.WaitAll(Task.Run(async () => loc = await GeoCoding.GetLocationByAdress(value.HouseNumber, value.StreetName, value.CityName)));
            if (loc == null)
            {
                throw new Exception("Location not found.");
            }

            party.PartyName = value.PartyName;
            party.PartyDate = value.PartyDate.Millisecond == 0 ? value.PartyDate.AddMilliseconds(01.123) : value.PartyDate;
            party.MusicGenre = value.MusicGenre;
            party.Location = new Location()
            {
                CityName = value.CityName,
                HouseNumber = value.HouseNumber,
                StreetName = value.StreetName,
                CountryName = value.CountryName,
                Zipcode = value.Zipcode,
                Latitude = loc.Latitude,
                Longitude = loc.Longitude,
            };
            party.PartyType = value.PartyType;
            party.Description = value.Description;
            party.Price = value.Price;
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
