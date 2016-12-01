﻿using App2NightAPI.Models;
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
using App2NightAPI.Models.Enum;

namespace App2NightAPI.Controllers
{
    [Route("api/Party")]
    [Authorize]
    public class PartyController : CustomController
    {
        private readonly int maxResults = 50;
        private DatabaseContext _dbContext;
        public PartyController(DatabaseContext dbContext, IUserService userService) : base(dbContext, userService)
        {
            _dbContext = dbContext;
        }

        //// GET api/Party
        ///// <summary>
        ///// Get Partys
        ///// </summary>
        ///// <remarks>
        ///// This function will return partys from the database where the date is today or in futre with the related rating.
        ///// </remarks>
        ///// <returns></returns>
        //[AllowAnonymous]
        //[HttpGet]
        //public ActionResult Get()
        //{
        //    try
        //    {
        //        //Test
        //        List<JObject> jsonList = new List<JObject>();

        //        var partys = _dbContext.PartyItems
        //            .Where(p => p.PartyDate >= DateTime.Today)
        //            .Include(p => p.Location)
        //            .Include(p => p.Host).ToList();

        //        if (partys == null)
        //        {
        //            return NotFound("There are no partys in the future.");
        //        }
        //        else
        //        {
        //            foreach (Party singleParty in partys)
        //            {
        //                if (singleParty.Location.Latitude == 0 || singleParty.Location.Longitude == 0)
        //                {
        //                    //Party without geocoordinates
        //                }
        //                else
        //                {

        //                }

        //                jsonList.Add(AddCustomJson(singleParty));
        //                //TODO
        //                //jsonList.Add(AddHostToJson(singleParty));
        //            }

        //            return Ok(jsonList);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        // GET api/Party
        /// <summary>
        /// Get Partys by user location.
        /// </summary>
        /// <remarks>
        /// This function will return partys from the database where the date is today or in the future with the related rating and is near to the user location.
        /// <br/><b>Please pass the URL-Parameter the following way: ...?lat=49...&amp;lon=9....&amp;radius=2...</b>
        /// <br/> Radius: Please pass a value between 0 and 200.
        /// </remarks>
        /// <param name="lat">User latitude</param>
        /// <param name="lon">User longitude</param>
        /// <param name="radius">Radius</param>
        /// <returns>Http Status Code 200 (Ok) and the Party List, or Http Status Code 400 (Bad Request), or Http Status Code 404 (Not Found) if no party in the future exists.</returns>
        /// <response code="200">Ok</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Not Found</response>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult GetWithUserLocation(double lat, double lon, float radius)
        {
            try
            {
                if (radius < 0 || radius > 200)
                {
                    //Radius is out of range
                    return BadRequest("Radius is out of range. Please pass a value between 0 and 200.");
                }
                else
                {
                    //Test
                    List<JObject> jsonList = new List<JObject>();

                    var partys = _dbContext.PartyItems
                        .Where(p => p.PartyDate >= DateTime.Today)
                        .Include(p => p.Location)
                        .Include(p => p.Host).ToList();

                    if (partys == null)
                    {
                        return NotFound("There are no partys in the future.");
                    }
                    else
                    {
                        //NOTE: Outcommented code is to get the nearest results. At the moment not used.

                        //Initialize list do save the party Id and the distance to the user
                        //List<Tuple<Guid, double?>> partyDistanceList = new List<Tuple<Guid, double?>>();
                        foreach (Party singleParty in partys)
                        {
                            double? distance = GeoCoding.GetDistance(lat, lon, singleParty.Location.Latitude, singleParty.Location.Longitude);

                            if (distance != null)
                            {
                                if (distance <= radius)
                                {
                                    //Save party Id and distance
                                    //partyDistanceList.Add(new Tuple<Guid, double?> ( singleParty.PartyId, distance ));

                                    //Party is near to the user location
                                    jsonList.Add(AddCustomJson(singleParty));
                                }
                            }
                        }

                        //if(partyDistanceList.Count <= 0)
                        //{
                        //    return BadRequest("No party in your nearness.");
                        //}
                        //else
                        //{
                        //    //Sort list by distance and take the first "maxResults"-entries
                        //    IEnumerable<Tuple<Guid, double?>> sortedList = partyDistanceList.OrderBy(l => l.Item2).ToList().Take(maxResults);
                        //    foreach(var singleEntry in sortedList)
                        //    {
                        //        var party = partys.FirstOrDefault(p => p.PartyId == singleEntry.Item1);
                        //        if(party != null)
                        //        {
                        //            //Party is near to the user location
                        //            jsonList.Add(AddCustomJson(party));
                        //        }
                        //    }
                        //}
                        return Ok(jsonList);
                    }
                }
            }
            catch (Exception)
            {
                return BadRequest("Paramter in URL: ...?lat=49...&lon=9....&radius=2...");
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

                //jsonList.Add(AddHostToJson(singleParty));
                jsonList.Add(AddCustomJson(singleParty));
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
        public ActionResult Post([FromBody]CreateParty value)
        {
            try
            {
                //Check if Json is valid.
                if (!TryValidateModel(value))
                {
                    //Party Model is not valid!
                    return BadRequest(new CreateParty());
                }
                //Check if Party Date is today or in future
                else if (value.PartyDate <= DateTime.Today)
                {
                    //Party Date is the past
                    return BadRequest("Party date can't be in the past.");
                } 
                else
                {
                    //Party is valid and is added to database.
                    var party = MapPartyToModel(value);
                    _dbContext.PartyItems.Add(party);
                    _dbContext.SaveChanges();
                    return Created("", party.PartyId);
                }
            }
            catch (Exception ex)
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
        [HttpPut]
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
        [HttpDelete]
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
                    if(party == null)
                    {
                        return NotFound("Party not found.");
                    }

                    //Check if User is Host of the party
                    else if(party.Host.UserId != User.UserId)
                    {
                        return Unauthorized();
                    }

                    //Check if the Date of the party is valid.
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
        /// <response code="200">Ok</response>
        /// <response code="406">Not Acceptable</response>
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

        /// <summary>
        /// Select User History
        /// </summary>
        /// <remarks>This function selects all parties in the past where the User has EvenCommtitmentState = Accepted.</remarks>
        /// <returns> Http Status Code 404 (Not found) if there are no parties for the user in future, or Http Status Code 200 (Ok)</returns>
        /// <response code="200">Ok</response>
        /// <response code="404">Not Found</response>
        [HttpGet("history")]
        public ActionResult GetHistoryAll()
        {
            List<JObject> jsonList = new List<JObject>();

            var userPartys = _dbContext.UserPartyItems
                .Where(up => up.UserId == User.UserId
                    && up.EventCommitment == Models.Enum.EventCommitmentState.Accepted)
                .ToList();

            if (userPartys != null && userPartys.Count == 0)
            {
                return NotFound("There are no partys for the current user in the past.");
            }
            else
            {
                foreach(var singleUserParty in userPartys)
                {
                    var party = _dbContext.PartyItems
                        .Include(p => p.Location)
                        .Include(p => p.Host)
                        .FirstOrDefault(p => p.PartyId == singleUserParty.PartyId
                                    && p.PartyDate.Date < DateTime.Now.Date);

                    if (party != null)
                    {
                        jsonList.Add(AddCustomJson(party));
                    }
                }
                return (Ok(jsonList));
            }
        }

        /// <summary>
        /// Select User Partys
        /// </summary>
        /// <remarks>This function selects all parties in the future where the User has EvenCommtitmentState = Accepted or Noted.</remarks>
        /// <returns> Http Status Code 404 (Not found) if there are no parties for the user in future, or Http Status Code 200 (Ok)</returns>
        /// <response code="200">Ok</response>
        /// <response code="404">Not Found</response>
        [HttpGet("myParties")]
        public ActionResult GetMyParties()
        {
            List<JObject> jsonList = new List<JObject>();

            var userPartys = _dbContext.UserPartyItems
                .Where(up => up.UserId == User.UserId
                    && (up.EventCommitment == Models.Enum.EventCommitmentState.Accepted
                    || up.EventCommitment == Models.Enum.EventCommitmentState.Noted))
                .ToList();

            if(userPartys != null && userPartys.Count == 0)
            {
                return NotFound("There are no partys for the current user.");
            }
            else
            {
                foreach(var singleUserParty in userPartys)
                {
                    var party = _dbContext.PartyItems
                        .Include(p => p.Location)
                        .Include(p => p.Host)
                        .FirstOrDefault(p => p.PartyId == singleUserParty.PartyId
                                    && p.PartyDate.Date >= DateTime.Now.Date);

                    if(party != null)
                    {
                        jsonList.Add(AddCustomJson(party));
                    }
                }
                return (Ok(jsonList));
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
        }

        private Party MapPartyToModel(CreateParty value)
        {
            var party = new Party();
            MapPartyToModel(value, ref party);
            party.Host = User;
            party.CreationDate = DateTime.Today;
            return party;
        }

        private JObject AddCustomJson(Party singleParty)
        {
            var jobject = JObject.FromObject(singleParty);
            AddHostToJson(ref jobject, singleParty.Host.UserId, singleParty.Host.UserName);
            AddHostedByUserToJson(ref jobject, CheckPartyHostedByUser(singleParty));
            AddRatingToParty(ref jobject, singleParty.PartyId);
            AddCommitments(ref jobject, singleParty.PartyId);
            AddUserCommitmentState(ref jobject, singleParty.PartyId);
            return jobject;
        }

        private void AddHostToJson(ref JObject jobject, Guid UserId, String UserName)
        {
            //var jobject = JObject.FromObject(singleParty);
            var host = new JObject() {
                        {
                            "HostId", /*singleParty.Host.*/UserId
                        },
                        {
                            "UserName", /*singleParty.Host.*/UserName
                        }
                    };
            jobject.Add("Host", host);
            //return jobject;
        }

        private void AddHostedByUserToJson(ref JObject party, Boolean IsHosted)
        {
            party.Add("HostedByUser", IsHosted);
            //return party;
        }

        private Boolean CheckPartyHostedByUser(Party party)
        {
            //Check if party is hosted by current user
            //No logged in user wants to see some partys
            try
            {
                return party.Host.UserId == User.UserId ? true : false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void AddRatingToParty(ref JObject party, Guid partyId)
        {
            var userRatings = _dbContext.UserPartyItems
                .Where(up => up.PartyId == partyId)
                .ToList();

            if(userRatings != null)
            {
                int generalUp = 0;
                int generalDown = 0;
                int priceUp = 0;
                int priceDown = 0;
                int locationUp = 0;
                int locationDown = 0;
                int moodUp = 0;
                int moodDown = 0;

                foreach (UserParty up in userRatings)
                {
                    if(up.GeneralRating == 1)
                    {
                        generalUp++;
                    }
                    else if(up.GeneralRating == -1)
                    {
                        generalDown++;
                    }
                    if(up.PriceRating == 1)
                    {
                        priceUp++;
                    }
                    else if(up.PriceRating == -1)
                    {
                        priceDown++;
                    }
                    if(up.LocationRating == 1)
                    {
                        locationUp++;
                    }
                    else if(up.LocationRating == -1)
                    {
                        locationDown++;
                    }
                    if(up.MoodRating == 1)
                    {
                        moodUp++;
                    }
                    else if(up.MoodRating == -1)
                    {
                        moodDown++;
                    }
                }

                //Add calculated values to json
                party.Add("GeneralUpVoting", generalUp);
                party.Add("GeneralDownVoting", generalDown);
                party.Add("PriceUpVotring", priceUp);
                party.Add("PriceDownVoting", priceDown);
                party.Add("LocationUpVoting", locationUp);
                party.Add("LocationDownVoting", locationDown);
                party.Add("MoodUpVoting", moodUp);
                party.Add("MoodDownVoting", moodDown);
            }
        }

        private void AddCommitments(ref JObject party, Guid partyId)
        {
            JArray userArray = new JArray();
            //Select all commitet user to the given party id
            var commitetUser = _dbContext.UserPartyItems
                .Where(up => up.PartyId == partyId && up.EventCommitment == Models.Enum.EventCommitmentState.Accepted)
                .Select(u => new { u.User })
                .ToList();

            if(commitetUser != null && commitetUser.Count > 0)
            {
                //Add all selected users to an array with UserId and UserName
                foreach (var up in commitetUser)
                {
                    var user = new JObject
                    {
                        {
                            "UserId", up.User.UserId
                        },
                        {
                            "UserName", up.User.UserName
                        }
                    };
                    userArray.Add(user);
                }
            }
            //Add the user array to the current party-JSON
            party.Add("CommittedUser", userArray);
        }

        private void AddUserCommitmentState(ref JObject party,  Guid partyId)
        {
            try
            {
                var userCommitment = _dbContext.UserPartyItems
                    .FirstOrDefault(p => p.PartyId == partyId && p.UserId == User.UserId);

                if (userCommitment != null)
                {
                    party.Add("UserCommitmentState", GetValueFromEventCommitmentstate(userCommitment.EventCommitment).ToString());
                }
                else
                {
                    //Set default value
                    party.Add("UserCommitmentState", GetValueFromEventCommitmentstate(userCommitment.EventCommitment).ToString());
                }
            }
            catch(Exception)
            {
                //Set default commitmentstate by definition
                party.Add("UserCommitmentState", GetValueFromEventCommitmentstate(EventCommitmentState.Rejected).ToString());
            }
        }

        private String GetValueFromEventCommitmentstate(EventCommitmentState state)
        {
            int iState = (int)Enum.Parse(state.GetType(), state.ToString());
            return iState.ToString();
        }
        #endregion
    }
}