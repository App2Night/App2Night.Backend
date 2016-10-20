using App2NightAPI.Models.Authentification;
using App2NightAPI.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App2NightAPI.Models
{
    public class UserParty
    {
        public User User { get; set; }
        public Party Party { get; set; }
        public EventCommitmentState EventCommitment { get; set; }
        public int PriceRating { get; set; }
        public int LocationRating { get; set; }
        public int MoodRating { get; set; }
    }
}
