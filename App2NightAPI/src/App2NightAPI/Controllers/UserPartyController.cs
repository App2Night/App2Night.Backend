using App2NightAPI.Models;
using App2NightAPI.Models.Enum;
using App2NightAPI.Models.REST_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App2NightAPI.Controllers
{
    [Route("api/UserParty")]
    [Authorize]
    public class UserPartyController : CustomController
    {
        private DatabaseContext _dbContext;
        public UserPartyController(DatabaseContext dbContext, IUserService userService) : base(dbContext, userService)
        {
            _dbContext = dbContext;
        }


        // PUT /api/UserParty/commitmentState
        /// <summary>
        /// Set Commitmentstate
        /// </summary>
        /// <remarks> This function sets the users commitmenstate to the party.
        /// </remarks>
        /// <param name="id">Party Id</param>
        /// <param name="commitmentState">Value from EventCommitmentState-Enum.</param>
        /// <returns>Http Status Code 200 (Ok), or Http Status Code 400 (Bad Request), or Http Status Code 404 (Not Found) if Party doesn't exists.</returns>
        [ProducesResponseType(typeof(EventCommitmentState), 400)]
        [HttpPut]
        [Route("commitmentState/id={id}")]
        public ActionResult SetCommitmentState(Guid? id, [FromBody]UserPartyCommitment commitmentState)
        {
            try
            {
                //Check if PartyId is valid
                if (!Validator.IsGuidValid(id.ToString()))
                {
                    return BadRequest("Party ID is not valid.");
                }
                else
                {
                    //Select party
                    var party = _dbContext.PartyItems
                        .FirstOrDefault(p => p.PartyId == id);

                    if (party == null)
                    {
                        return NotFound("Party not found.");
                    }
                    else if (party.PartyDate < DateTime.Now)
                    {
                        return BadRequest("Can't commit to a party with party date in the past.");
                    }
                    else
                    {
                        UserParty userParty = _dbContext.UserPartyItems
                            .FirstOrDefault(up => up.PartyId == party.PartyId && up.UserId == User.UserId);

                        if (userParty == null)
                        {
                            userParty = new UserParty
                            {
                                PartyId = party.PartyId,
                                UserId = User.UserId,
                                EventCommitment = commitmentState.EventCommitment
                            };

                            _dbContext.UserPartyItems.Add(userParty);
                            _dbContext.SaveChanges();
                            return Ok();
                        }
                        else
                        {
                            if (userParty.EventCommitment == commitmentState.EventCommitment)
                            {
                                //Nothing changed
                                return Ok();
                            }
                            else
                            {
                                userParty.EventCommitment = commitmentState.EventCommitment;

                                _dbContext.UserPartyItems.Update(userParty);
                                _dbContext.SaveChanges();
                                return Ok();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        // PUT /api/UserParty/rating
        /// <summary>
        /// Set Rating
        /// </summary>
        /// <remarks> This function sets the users rating to the party.
        /// </remarks>
        /// <param name="id">Party Id</param>
        /// <param name="partyRating">Model with the different rating values. <b>Please set:<br/>0 for not rated,<br/>1 for Up-Vote and <br/>-1 for Down-Vote!</b></param>
        /// <returns>Http Status Code 200 (Ok), or Http Status Code 400 (Bad Request), or Http Status Code 404 (Not Found) if Party doesn't exists.</returns>
        [ProducesResponseType(typeof(UserPartyRating), 400)]
        [HttpPut]
        [Route("partyRating/id={id}")]
        public ActionResult SetRating(Guid? id, [FromBody]UserPartyRating partyRating)
        {
            try
            {
                //Check if PartyId is valid
                if (!Validator.IsGuidValid(id.ToString()))
                {
                    return BadRequest("Party ID is not valid.");
                }
                else
                {
                    //Select party
                    var party = _dbContext.PartyItems
                        .FirstOrDefault(p => p.PartyId == id);

                    if (party == null)
                    {
                        return NotFound("Party not found.");
                    }
                    else if (party.PartyDate < DateTime.Now)
                    {
                        return BadRequest("Can't rate a party with party date in the past.");
                    }
                    else if(!TryValidateModel(partyRating))
                    {
                        return BadRequest(new UserPartyRating());
                    }
                    else
                    {
                        UserParty userParty = _dbContext.UserPartyItems
                            .FirstOrDefault(up => up.PartyId == party.PartyId && up.UserId == User.UserId);

                        if (userParty == null)
                        {
                            return BadRequest("Can't rate a party without commitment.");
                        }
                        else if(userParty.EventCommitment == EventCommitmentState.Rejected)
                        {
                            return BadRequest("Can't rate the party. Users current commitment is rejected.");
                        }
                        else
                        {
                            String messageRatingRange = CheckRatingValuesRange(partyRating);
                            if (!String.IsNullOrEmpty(messageRatingRange))
                            {
                                //Something is wrong with the range of the rating values
                                return BadRequest(messageRatingRange);
                            }
                            else
                            {
                                Boolean hasChanged = MapRatingToModel(partyRating, ref userParty);
                                if(!hasChanged)
                                {
                                    //Nothing changed
                                    return Ok();
                                }
                                else
                                {
                                    _dbContext.UserPartyItems.Update(userParty);
                                    _dbContext.SaveChanges();
                                    return Ok();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        #region Help Functions
        private Boolean MapRatingToModel(UserPartyRating partyRating, ref UserParty userParty)
        {
            Boolean hasChanged = false;
            //Value mapping
            if (userParty.GeneralRating != partyRating.GeneralRating)
            {
                userParty.GeneralRating = partyRating.GeneralRating;
                hasChanged = true;
            }
            if (userParty.PriceRating != partyRating.PriceRating)
            {
                userParty.PriceRating = partyRating.PriceRating;
                hasChanged = true;
            }
            if (userParty.LocationRating != partyRating.LocationRating)
            {
                userParty.LocationRating = partyRating.LocationRating;
                hasChanged = true;
            }
            if (userParty.MoodRating != partyRating.MoodRating)
            {
                userParty.MoodRating = partyRating.MoodRating;
                hasChanged = true;
            }

            return hasChanged;
        }

        private String CheckRatingValuesRange(UserPartyRating partyRating)
        {
            //Check rating values
            if (!CheckRatingValue(partyRating.GeneralRating))
            {
                return "GeneralRating value is not beetween -1 and 1.";
            }
            else if (!CheckRatingValue(partyRating.PriceRating))
            {
                return "PriceRating value is not beetween -1 and 1.";
            }
            else if (!CheckRatingValue(partyRating.LocationRating))
            {
                return "LocationRating value is not beetween -1 and 1.";
            }
            else if (!CheckRatingValue(partyRating.MoodRating))
            {
                return "MoodRating value is not beetween -1 and 1.";
            }
            else
            {
                return String.Empty;
            }
        }

        private Boolean CheckRatingValue(int val)
        {
            if(val >= -1 && val <= 1)
            {
                return true;
            }
            return false;
        }
        #endregion

    }
}
