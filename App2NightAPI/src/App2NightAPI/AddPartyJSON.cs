using App2NightAPI.Models;
using App2NightAPI.Models.Authentification;
using App2NightAPI.Models.Enum;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App2NightAPI
{
    public class AddPartyJSON
    {
        private DatabaseContext _dbContext;
        private User _user;
        public AddPartyJSON(DatabaseContext dbContext, User user)
        {
            _dbContext = dbContext;
            _user = user;
        }
        public JObject AddCustomJson(Party singleParty)
        {
            var jobject = JObject.FromObject(singleParty);
            AddHostToJson(ref jobject, singleParty.Host.UserId, singleParty.Host.UserName);
            AddHostedByUserToJson(ref jobject, CheckPartyHostedByUser(singleParty));
            AddRatingToParty(ref jobject, singleParty.PartyId);
            AddCommitments(ref jobject, singleParty.PartyId);
            AddUserCommitmentState(ref jobject, singleParty.PartyId);
            return jobject;
        }

        public JObject AddCustomJsonForAdmin(Party singleParty)
        {
            var jobject = JObject.FromObject(singleParty);
            AddRatingToParty(ref jobject, singleParty.PartyId, true);
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
                return party.Host.UserId == _user.UserId ? true : false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void AddRatingToParty(ref JObject party, Guid partyId, Boolean JustGeneralVoting = false)
        {
            int generalUp = 0;
            int generalDown = 0;
            int priceUp = 0;
            int priceDown = 0;
            int locationUp = 0;
            int locationDown = 0;
            int moodUp = 0;
            int moodDown = 0;

            if (!JustGeneralVoting)
            {
                var userRatings = _dbContext.UserPartyItems
                    .Where(up => up.PartyId == partyId)
                    .ToList();

                if (userRatings != null)
                {
                    foreach (UserParty up in userRatings)
                    {
                        if (up.GeneralRating == 1)
                        {
                            generalUp++;
                        }
                        else if (up.GeneralRating == -1)
                        {
                            generalDown++;
                        }
                        if (up.PriceRating == 1)
                        {
                            priceUp++;
                        }
                        else if (up.PriceRating == -1)
                        {
                            priceDown++;
                        }
                        if (up.LocationRating == 1)
                        {
                            locationUp++;
                        }
                        else if (up.LocationRating == -1)
                        {
                            locationDown++;
                        }
                        if (up.MoodRating == 1)
                        {
                            moodUp++;
                        }
                        else if (up.MoodRating == -1)
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
            else if(JustGeneralVoting)
            {
                var userRatings = _dbContext.UserPartyItems
                    .Where(p => p.PartyId == partyId)
                    .ToList();

                if(userRatings != null)
                {
                    foreach(UserParty up in userRatings)
                    {
                        if (up.GeneralRating == 1)
                        {
                            generalUp++;
                        }
                        else if (up.GeneralRating == -1)
                        {
                            generalDown++;
                        }
                    }
                }
                party.Add("GeneralUpVoting", generalUp);
                party.Add("GeneralDownVoting", generalDown);
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

            if (commitetUser != null && commitetUser.Count > 0)
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

        private void AddUserCommitmentState(ref JObject party, Guid partyId)
        {
            try
            {
                var userCommitment = _dbContext.UserPartyItems
                    .FirstOrDefault(p => p.PartyId == partyId && p.UserId == _user.UserId);

                if (userCommitment != null)
                {
                    party.Add("UserCommitmentState", GetValueFromEventCommitmentstate(userCommitment.EventCommitment));
                }
                else
                {
                    //Set default value
                    party.Add("UserCommitmentState", GetValueFromEventCommitmentstate(userCommitment.EventCommitment));
                }
            }
            catch (Exception)
            {
                //Set default commitmentstate by definition
                party.Add("UserCommitmentState", GetValueFromEventCommitmentstate(EventCommitmentState.Rejected));
            }
        }

        private int GetValueFromEventCommitmentstate(EventCommitmentState state)
        {
            return (int)Enum.Parse(state.GetType(), state.ToString());
            //return iState.ToString();
        }

        public JObject AddValidStateToLocation(Location location, Boolean isValid)
        {
            if (location == null)
            {
                return null;
            }
            else
            {
                JObject partyJObject = JObject.FromObject(location);
                partyJObject.Add("IsValid", isValid);
                return partyJObject;
            }
        }
    }
}
