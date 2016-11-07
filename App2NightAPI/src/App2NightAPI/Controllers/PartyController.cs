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

                foreach(Party singleParty in partys)
                {
                    if(singleParty.Location.Latitude != 0 && singleParty.Location.Longitude != 0)
                    {

                    }
                    else
                    {

                    }

                    jsonList.Add(_AddHostToJson(singleParty));
                }

                return Ok(jsonList);
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
                    return BadRequest(new CreateParty());
                }
                else
                {
                    var party = _mapPartyToModel(value);

                    if (!TryValidateModel(party))
                    {
                        //Party Model is not valid!
                        return BadRequest(new CreateParty());
                    }
                    else
                    {
                        Location loc = await GeoCoding.GetLocationByAdress(party.Location.HouseNumber, party.Location.StreetName, party.Location.CityName);
                        if (loc == null)
                        {
                            return BadRequest("Cannot find Location.");
                        }
                        else
                        {
                            party.Location = loc;
                            _dbContext.PartyItems.Add(party);
                            _dbContext.SaveChanges();
                            return Created("", party.PartyId);
                        }
                    }
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
                    //Check if Party Date is toady or in future
                    if (value.PartyDate <= DateTime.Today)
                    {
                        //Party Date is not today or in future
                        return BadRequest("Party have to be in the future.");
                    }
                    else
                    {
                        var party = _mapPartyToModel(value);
                        party.PartyId = Guid.Parse(id.ToString());

                        //Check if Party Element exists in the databse.
                        int count = _dbContext.PartyItems.Count(p => p.PartyId == id);
                        if (count != 1)
                        {
                            //Party contains not in the databse.
                            return NotFound("Party not found.");
                        }
                        else
                        {
                            var usr = GetUser();
                            party.Location.LocationId = _dbContext.PartyItems
                                .Where(p => p.PartyId == party.PartyId)
                                .Select(p => new { p.Location.LocationId })
                                .FirstOrDefault().LocationId;

                            if (!TryValidateModel(party))
                            {
                                return BadRequest(new CreateParty());
                            }
                            //TODO Check if Party Host == Logged in User
                            //Only the Host of the Party can modify it!
                            //else if (party.Host.UserId != usr.UserId)
                            //{

                            //}
                            else
                            {
                                _dbContext.PartyItems.Update(party);
                                _dbContext.SaveChanges();
                                return Ok();
                            }
                        }
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
                    return BadRequest();
                }
                else
                {
                    int count = _dbContext.PartyItems.Count(p => p.PartyId == partyId);
                    if (count != 1)
                    {
                        return NotFound("Party not found.");
                    }
                    else
                    {
                        var usr = GetUser();
                        Party selectedParty = _dbContext.PartyItems
                                 .Include(p => p.Location)
                                 .Include(p => p.Host)
                                 .First<Party>(p => p.PartyId == partyId);

                        if (selectedParty == null)
                        {
                            //Party is in the past
                            //Can't delete
                            return BadRequest("Can't delete a party with date in the past.");
                        }
                        //TODO Check if Party Host == Logged in User
                        //Only the Host of the Party can delete it!
                        //else if (selectedParty.Host.UserId != usr.UserId)
                        //{
                        //    return Unauthorized();
                        //}
                        //Check if Party Datum is in future
                        else if (selectedParty.PartyDate < DateTime.Now)
                        {
                            return BadRequest();
                        }
                        else
                        {
                            //Delete from Database
                            _dbContext.Entry(selectedParty).State = EntityState.Deleted;
                            _dbContext.Entry(selectedParty.Location).State = EntityState.Deleted;
                            _dbContext.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
            return Ok();
        }

        #region Help Functions

        private Party _mapPartyToModel(CreateParty value)
        {
            return new Party
            {
                PartyName = value.PartyName,
                PartyDate = value.PartyDate,
                CreationDate = DateTime.Today,
                MusicGenre = value.MusicGenre,
                Location = value.Location,
                PartyType = value.PartyType,
                //Host = _dbContext.UserItems.First<User>(p => p.UserId == Guid.Parse("1bd535c8-f90b-4a25-5b26-08d3f9b43b33")),
                Host = _dbContext.UserItems.First<User>(p => p.UserId == GetUser().UserId),
                Description = value.Description
            };
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

        private double? _getDistance(double lat1, double lon1, double lat2, double lon2)
        {
            try
            {
                //Test
                lat1 = 48.2086369;
                lon1 = 8.7548875;
                lat2 = 48.4456403;
                lon2 = 8.6942879;

                var R = 6371; //Radius Earth
                var deltaLat = deg2rad(lat2 - lat1);
                var deltaLon = deg2rad(lon2 - lon1);
                var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
                var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                var d = R * c; //Distance in km
                return d;
            }
            catch(Exception)
            {
                return null;
            }
        }

        private double deg2rad(double deg)
        {
            return deg * (Math.PI / 100);
        }
        #endregion
    }
}