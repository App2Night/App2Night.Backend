using App2NightAPI.Models;
using App2NightAPI.Models.Authentification;
using App2NightAPI.Models.REST_Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace App2NightAPI.Controllers
{
    [Route("api/Party")]
    public class PartyController : Controller
    {
        private DatabaseContext _dbContext;
        public PartyController(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET api/Party
        /// <summary>
        /// Get Partys
        /// </summary>
        /// <remarks>
        /// This function will return 15 partys from the database (at the moment!).
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<Party> Get()
        {
            return _dbContext.PartyItems.Take(15);
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
                var party = _mapPartyToModel(value); 

                bool validated = TryValidateModel(party);

                if (validated)
                {
                    _dbContext.PartyItems.Add(party);
                    _dbContext.UserItems.First<User>(p => p.UserId == Guid.Parse("1bd535c8-f90b-4a25-5b26-08d3f9b43b33")).PartyHostedByUser.Add(party);
                    _dbContext.SaveChanges();
                    return Created("", party.PartId);
                }
                else
                {
                    return BadRequest(new CreateParty());
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
        /// <returns>Http Status Code 200 (Ok), or Http Status Code 400 (Bad Request)</returns>
        /// <response code="200">Ok</response>
        /// <response code="400">Bad Request</response>
        [ProducesResponseType(typeof(CreateParty), 400)]
        [HttpPut("id={id}")]
        public ActionResult Modify(Guid? id, [FromBody]CreateParty value)
        {
            try
            {
                var party = _mapPartyToModel(value);
                party.PartId = Guid.Parse(id.ToString());
                party.Location.LocationId = _dbContext.PartyItems.Where(p => p.PartId == party.PartId).Select(p => new { p.Location.LocationId }).FirstOrDefault().LocationId;
                
                bool validated = TryValidateModel(party);

                if (validated)
                {
                    _dbContext.PartyItems.Update(party);
                    _dbContext.SaveChanges();
                    return Ok();
                }
                else
                {
                    return BadRequest(new CreateParty());
                }
            }
            catch(Exception)
            {
                return BadRequest();
            }
        }

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
                Host = _dbContext.UserItems.First<User>(p => p.UserId == Guid.Parse("1bd535c8-f90b-4a25-5b26-08d3f9b43b33")),
                Description = value.Description
            };
        }

        // DELETE /api/Party
        /// <summary>
        /// Deletes a Party
        /// </summary>
        /// <remarks>
        /// This function will delete a party from the database.
        /// </remarks>
        /// <param name="id">Party Id (Passed in the URL)</param>
        /// <returns>Http Status Code 200 (Ok), or Http Status Code 400 (Bad Request)</returns>
        /// <response code="200">Ok</response>
        /// <response code="400">Bad Request</response>
        [HttpDelete("id={id}")]
        public ActionResult Delete(Guid? id)
        {
            Guid partyId;
            try
            {
                if (Guid.TryParse(id.ToString(), out partyId))
                {
                    Party selectedParty = _dbContext.PartyItems.First<Party>(p => p.PartId == partyId);
                    if (selectedParty != null)
                    {
                        _dbContext.PartyItems.Remove(selectedParty);
                        _dbContext.SaveChanges();
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}