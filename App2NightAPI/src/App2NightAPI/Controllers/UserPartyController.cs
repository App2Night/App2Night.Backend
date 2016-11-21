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
        /// <param name="id"></param>
        /// <param name="commitmentState"></param>
        /// <returns></returns>
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
                        .FirstOrDefault(p => p.PartyId == id);// && p.PartyDate <= DateTime.Today.AddDays(1));

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
        /// <param name="id"></param>
        /// <param name="commitmentState"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(EventCommitmentState), 400)]
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
                        .FirstOrDefault(p => p.PartyId == id);// && p.PartyDate <= DateTime.Today.AddDays(1));

                    if (party == null)
                    {
                        return NotFound("Party not found.");
                    }
                    else if (party.PartyDate < DateTime.Now)
                    {
                        return BadRequest("Can't rate a party with party date in the past.");
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
                                //TODO
                                //Check if Up and Down Votes pro Attribute is set...

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
            if (userParty.GeneralUpVotes != partyRating.GeneralUpVotes)
            {
                userParty.GeneralUpVotes = partyRating.GeneralUpVotes;
                hasChanged = true;
            }
            if (userParty.GeneralDownVotes != partyRating.GeneralDownVotes)
            {
                userParty.GeneralDownVotes = partyRating.GeneralDownVotes;
                hasChanged = true;
            }
            if (userParty.PriceUpVotes != partyRating.PriceUpVotes)
            {
                userParty.PriceUpVotes = partyRating.PriceUpVotes;
                hasChanged = true;
            }
            if (userParty.PriceDownVotes != partyRating.PriceDownVotes)
            {
                userParty.PriceDownVotes = partyRating.PriceDownVotes;
                hasChanged = true;
            }
            if (userParty.LocationUpVotes != partyRating.LocationUpVotes)
            {
                userParty.LocationUpVotes = partyRating.LocationUpVotes;
                hasChanged = true;
            }
            if (userParty.LocationDownVotes != partyRating.LocationDownVotes)
            {
                userParty.LocationDownVotes = partyRating.LocationDownVotes;
                hasChanged = true;
            }
            if (userParty.MoodUpVotes != partyRating.MoodUpVotes)
            {
                userParty.MoodUpVotes = partyRating.MoodUpVotes;
                hasChanged = true;
            }
            if (userParty.MoodDownVotes != partyRating.MoodDownVotes)
            {
                userParty.MoodDownVotes = partyRating.MoodDownVotes;
                hasChanged = true;
            }

            return hasChanged;
        }

        private String CheckRatingValuesRange(UserPartyRating partyRating)
        {
            //Check rating values
            if (!CheckRatingValue(partyRating.GeneralUpVotes))
            {
                return "GeneralUpVotes value is not beetween 0 and 1.";
            }
            else if (!CheckRatingValue(partyRating.GeneralDownVotes))
            {
                return "GeneralDownVotes value is not beetween 0 and 1.";
            }
            else if (!CheckRatingValue(partyRating.PriceUpVotes))
            {
                return "PriceUpVotes value is not beetween 0 and 1.";
            }
            else if (!CheckRatingValue(partyRating.PriceDownVotes))
            {
                return "PriceDownVotes value is not beetween 0 and 1.";
            }
            else if (!CheckRatingValue(partyRating.LocationUpVotes))
            {
                return "LocationUpVotes value is not beetween 0 and 1.";
            }
            else if (!CheckRatingValue(partyRating.LocationDownVotes))
            {
                return "LocationDownVotes value is not beetween 0 and 1.";
            }
            else if (!CheckRatingValue(partyRating.MoodUpVotes))
            {
                return "MoodUpVotes value is not beetween 0 and 1.";
            }
            else if (!CheckRatingValue(partyRating.MoodDownVotes))
            {
                return "MoodDownVotes value is not beetween 0 and 1.";
            }
            else
            {
                return String.Empty;
            }
        }

        private Boolean CheckRatingValue(int val)
        {
            if(val >= 0 && val <= 1)
            {
                return true;
            }
            return false;
        }
        #endregion

    }
}
