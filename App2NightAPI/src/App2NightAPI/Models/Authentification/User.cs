using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace App2NightAPI.Models.Authentification
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Location Location { get; set; }

        public List<Party> PartyHostedByUser { get; set; }

    }
}
