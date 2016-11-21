using App2NightAPI.Models.Authentification;
using App2NightAPI.Models.Enum;
using App2NightAPI.Models.Model_Abstraction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace App2NightAPI.Models.REST_Models
{
    public class CreateParty : IPartyBase 
    {
        [Required]
        public string PartyName { get; set; }
        [Required]
        public DateTime PartyDate { get; set; }
        [Required]
        public MusicGenre MusicGenre { get; set; }
        [Required]
        public string CountryName { get; set; }
        [Required]
        public string CityName { get; set; }
        [Required]
        public string StreetName { get; set; }
        [Required]
        public string HouseNumber { get; set; }
        [Required]
        public string Zipcode { get; set; }
        [Required]
        public PartyType PartyType { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
