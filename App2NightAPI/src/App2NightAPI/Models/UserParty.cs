using App2NightAPI.Models.Authentification;
using App2NightAPI.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App2NightAPI.Models
{
    public class UserParty
    {
        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; }
        [Required]
        public Guid PartyId { get; set; }
        public Party Party { get; set; }
        public EventCommitmentState EventCommitment { get; set; }
        public int GeneralRating { get; set; }
        public int PriceRating { get; set; }
        public int LocationRating { get; set; }
        public int MoodRating { get; set; }
        //public int GeneralUpVotes { get; set; }
        //public int GeneralDownVotes { get; set; }
        //public int PriceUpVotes { get; set; }
        //public int PriceDownVotes { get; set; }
        //public int LocationUpVotes { get; set; }
        //public int LocationDownVotes { get; set; }
        //public int MoodUpVotes { get; set; }
        //public int MoodDownVotes { get; set; }
    }
}
