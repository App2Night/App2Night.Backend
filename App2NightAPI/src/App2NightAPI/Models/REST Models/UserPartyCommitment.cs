using App2NightAPI.Models.Enum;
using App2NightAPI.Models.Model_Abstraction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App2NightAPI.Models.REST_Models
{
    public class UserPartyCommitment
    {
        [Required]
        public EventCommitmentState EventCommitment { get; set; }
    }
}
