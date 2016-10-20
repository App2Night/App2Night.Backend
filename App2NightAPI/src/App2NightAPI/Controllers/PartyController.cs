﻿using App2NightAPI.Models;
using App2NightAPI.Models.Authentification;
using App2NightAPI.Models.REST_Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        [HttpGet]
        public IEnumerable<Party> Get()
        {
            return _dbContext.PartyItems.Take(15);
        }

        // POST api/Party
        /// <summary>
        /// Post Party
        /// </summary>
        /// <remarks>
        /// This function will create a new party to a user
        /// </remarks>
        /// <param name="value">JSON Body</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Post([FromBody]CreateParty value)
        {
            try
            {
                var party = new Party
                {
                    Name = value.PartyName,
                    Date = DateTime.Today,
                    Host = _dbContext.UserItems.First<User>(p => p.UserId == Guid.Parse("31c0fe14-5964-439f-a877-08d3f68dcb0c"))
                };

                bool validated = TryValidateModel(party);

                if (validated)
                {
                    _dbContext.PartyItems.Add(party);
                    _dbContext.UserItems.First<User>(p => p.UserId == Guid.Parse("31c0fe14-5964-439f-a877-08d3f68dcb0c")).PartyHostedByUser.Add(party);
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
        /// Update Party
        /// </summary>
        /// <remarks>
        /// This is only a basic implementation and won't change the database!
        /// </remarks>
        /// <param name="id">Party ID</param>
        /// <param name="value">JSON Body</param>
        /// <returns></returns>
        [HttpPut("id={id}")]
        public ActionResult Modify(Guid? id, [FromBody]CreateParty value)
        {

            return Ok();
        }


        // DELETE /api/Party
        /// <summary>
        /// Delete Party
        /// </summary>
        /// <remarks>
        /// This is only a basic implementation and won't change the database!
        /// </remarks>
        /// <param name="id">Party ID</param>
        /// <returns></returns>
        [HttpDelete("id={id}")]
        public ActionResult Delete(Guid? id)
        {
            //Guid partyId;
            //try
            //{

            //   if(Guid.TryParse(id.ToString(), out partyId))
            //    {
            //        Party selectedParty = _dbContext.PartyItems.First<Party>(p => p.PartId == partyId);
            //        if(selectedParty != null)
            //        {
            //            _dbContext.PartyItems.Remove(selectedParty);
            //            _dbContext.SaveChanges();
            //        }
            //        else
            //        {
            //            //TODO ELSE
            //        }
            //    }
            //    else
            //    {
            //        return BadRequest();
            //    }

            //}
            //catch (Exception ex)
            //{
            //    return BadRequest();
            //}
            return Ok();
        }
    }
}