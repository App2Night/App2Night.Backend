using App2NightAPI.Models;
using App2NightAPI.Models.Authentification;
using App2NightAPI.Models.REST_Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;

namespace App2NightAPI.Controllers
{
    [Route("api/Party")]
    [Authorize]
    public class PartyController : CustomController
    {
        private DatabaseContext _dbContext;
        public PartyController(DatabaseContext dbContext, IUserService userService) : base(dbContext, userService)
        {
            _dbContext = dbContext;
        }

        // GET api/Party
        /// <summary>
        /// Get Partys
        /// </summary>
        /// <remarks>
        /// This function will return 15 partys from the database (at the moment!) where the date is today or in futre.
        /// </remarks>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Get()
        {
            try
            {
                //Test
                List<JObject> jsonList = new List<JObject>();

                var partys = _dbContext.PartyItems
                    .Where(p => p.PartyDate >= DateTime.Today)
                    .Include(p => p.Location)
                    .Include(p => p.Host);

                if (partys == null)
                {
                    return NotFound("There are no partys in the past.");
                }
                else
                {
                    foreach (Party singleParty in partys)
                    {
                        if (singleParty.Location.Latitude != 0 && singleParty.Location.Longitude != 0)
                        {

                        }
                        else
                        {

                        }

                        jsonList.Add(_AddHostToJson(singleParty));
                    }

                    return Ok(jsonList);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // GET api/Party
        /// <summary>
        /// Get a Party by Id
        /// </summary>
        /// <remarks>
        /// This function will load a party by the given PartyId.
        /// </remarks>
        /// <param name="id">Party Id</param>
        /// <returns>Http Status Code 200 (Ok) and the Party Object, or Http Status Code 400 (Bad Request), or Http Status Code 404 (Not Found) if Party ID don't exists.</returns>
        /// <response code="200">Ok</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Not Found</response>
        [AllowAnonymous]
        [ProducesResponseType(typeof(Party), 200)]
        [HttpGet("id={id}")]
        public ActionResult GetPartyById(Guid? id)
        {
            try
            {
                List<JObject> jsonList = new List<JObject>();

                var singleParty = _dbContext.PartyItems
                    .Include(p => p.Location)
                    .Include(p => p.Host)
                    .First<Party>(p => p.PartyId == id);

                jsonList.Add(_AddHostToJson(singleParty));
                return Ok(jsonList);
            }
            catch (Exception)
            {
                return NotFound("Party not found.");
            }
        }

        // POST api/Party
        /// <summary>
        /// Creates a Party
        /// </summary>
        /// <remarks>
        /// This function will create a new party. At the moment the hoster of the party is hard coded to a specific user.
        /// </remarks>
        /// <param name="value">JSON Body</param>
        /// <returns>Http Status Code 201 (Created) and the newly created Party-Id, or Http Status Code 400 (Bad Request)</returns>
        /// <response code="201">Ok</response>
        /// <response code="400">Bad Request</response>
        [ProducesResponseType(typeof(CreateParty), 400)]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody]CreateParty value)
        {
            try
            {
                //Check if Party Date is today or in future
                if (value.PartyDate <= DateTime.Today)
                {
                    //Party Date is the past
                    return BadRequest("Party date can't be in the past.");
                }
                else if (!TryValidateModel(value))
                {
                    //Party Model is not valid!
                    return BadRequest(new CreateParty());
                }
                else
                {
                    //Party is valid.
                    var party = MapPartyToModel(value);
                    _dbContext.PartyItems.Add(party);
                    _dbContext.SaveChanges();
                    return Created("", party.PartyId);
                }
            }
            catch (Exception)
            {
                return BadRequest(new CreateParty());
            }
}

        // PUT /api/Party
        /// <summary>
        /// Updates a Party
        /// </summary>
        /// <remarks>
        /// This function will modify a given party.
        /// </remarks>
        /// <param name="id">Party Id (Passed in the URL)</param>
        /// <param name="value">JSON Body</param>
        /// <returns>Http Status Code 200 (Ok), or Http Status Code 400 (Bad Request), or Http Status Code 404 (Not Found) if Party ID don't exists.</returns>
        /// <response code="200">Ok</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Not Found</response>
        [ProducesResponseType(typeof(CreateParty), 400)]
        [HttpPut("id={id}")]
        public ActionResult Modify(Guid? id, [FromBody]CreateParty value)
        {
            try
            {
                //Check if Party Id is valid
                if (!Validator.IsGuidValid(id.ToString()))
                {
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
                    //Check if Party Host == Logged in User
                    //Only the Host of the Party can modify it!
                    else if (party.Host.UserId != User.UserId)
                    {
                        return Unauthorized();
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

        // DELETE /api/Party
        /// <summary>
        /// Deletes a Party
        /// </summary>
        /// <remarks>
        /// This function will delete a party and the related location from the database.
        /// </remarks>
        /// <param name="id">Party Id (Passed in the URL)</param>
        /// <returns>Http Status Code 200 (Ok), or Http Status Code 400 (Bad Request), or Http Status Code 404 (Not Found) if Party ID don't exists.</returns>
        /// <response code="200">Ok</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404"> Not Found</response>
        [HttpDelete("id={id}")]
        public ActionResult Delete(Guid? id)
        {
            Guid partyId;
            try
            {
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

                    if(party == null)
                    {
                        return NotFound("Party not found.");
                    }
                    else if(party.Host.UserId != User.UserId)
                    {
                        return Unauthorized();
                    }
                    else if(party.PartyDate < DateTime.Now)
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

        /// <summary>
        /// Validate a given location via Google
        /// </summary>
        /// <remarks>This validation will check the given location via google and return the google result as a location object.</remarks>
        /// <param name="location">Current Location</param>
        /// <returns> Http Status Code 400 (Bad Request) if given location is null, or Http Status Code 406 (Not Acceptable), or
        /// Http Status Code Ok (201) if location was found.</returns>
        [HttpPost("validate")]
        public ActionResult ValidateAdress([FromBody]Location location)
        {
            if(location == null)
            {
                return BadRequest(new Location());
            }
            else
            {
                Location loc = null;
                Task.WaitAll(Task.Run(async () => loc = await GeoCoding.GetLocationByAdress(location.HouseNumber, location.StreetName, location.CityName)));

                if (loc == null)
                {
                    return StatusCode(406); //Not Acceptable
                }
                else
                {
                    return Ok(loc);
                }
            }
        }

        [HttpGet("history")]
        public ActionResult GetHistoryAll()
        {
            List<JObject> jsonList = new List<JObject>();

            var partys = _dbContext.PartyItems
                    .Where(p => p.PartyDate < DateTime.Today)
                    .Include(p => p.Location)
                    .Include(p => p.Host);

            if (partys == null)
            {
                return NotFound("There are no partys in the past.");
            }
            else
            {
                foreach (Party singleParty in partys)
                {
                    jsonList.Add(_AddHostToJson(singleParty));
                }

                return Ok(jsonList);
            }
        }

        #region Help Functions
        private void MapPartyToModel(CreateParty value, ref Party party)
        {
            Location loc = null;
            Task.WaitAll(Task.Run(async () => loc = await GeoCoding.GetLocationByAdress(value.HouseNumber, value.StreetName, value.CityName)));
            if(loc == null)
            {
                throw new Exception("Location not found.");
            }

            party.PartyName = value.PartyName;
            party.PartyDate = value.PartyDate;
            party.MusicGenre = value.MusicGenre;
                party.Location = new Location()
                {
                    CityName = value.CityName,
                    HouseNumber = value.HouseNumber,
                    StreetName = value.StreetName,
                    CountryName = value.CountryName,
                    HouseNumberAdditional = value.HouseNumberAdditional,
                    Zipcode = value.Zipcode,
                    Latitude = loc.Latitude,
                    Longitude = loc.Longitude,
                };
            party.PartyType = value.PartyType;
            party.Description = value.Description;
        }

        private Party MapPartyToModel(CreateParty value)
        {
            var party = new Party();
            MapPartyToModel(value, ref party);
            party.Host = User;
            party.CreationDate = DateTime.Today;
            return party;
        }

        private JObject _AddHostToJson(Party singleParty)
        {
            var jobject = JObject.FromObject(singleParty);
            var host = new JObject() {
                        {
                            "HostId", singleParty.Host.UserId
                        },
                        {
                            "UserName", singleParty.Host.UserName
                        }
                    };
            jobject.Add("Host", host);
            return jobject;
        }
        #endregion
    }
}