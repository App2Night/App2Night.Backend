using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace App2NightAPI.Models.Authentification
{
    public class User : IdentityUser
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //[Key]
        //public Guid UserId { get; set; }
        //[Required]
        //public string Username { get; set; }
        //[Required]
        //public string Password { get; set; }
        public Location Location { get; set; }

        //TODO Wenn der User abgefragt wird, soll das JSON-Objekt im nachhinein bearbeitet werden und nur 
        //die IDs der Party übergeben werden, nicht das ganze Objekt "PartyHostedByUser"
        [JsonIgnore]
        public List<Party> PartyHostedByUser { get; set; }

    }
}
