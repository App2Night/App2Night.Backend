using App2NightAPI.Models.Enum;
using App2NightAPI.Models.Model_Abstraction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App2NightAPI.Models.REST_Models
{
    public class UserPartyRating
    {
        public int GeneralUpVotes { get; set; }
        public int GeneralDownVotes { get; set; }
        public int PriceUpVotes { get; set; }
        public int PriceDownVotes { get; set; }
        public int LocationUpVotes { get; set; }
        public int LocationDownVotes { get; set; }
        public int MoodUpVotes { get; set; }
        public int MoodDownVotes { get; set; }
    }
}
