using App2NightAPI.Models.Authentification;
using App2NightAPI.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App2NightAPI.Models.REST_Models
{
    public class CreateParty
    {
        [Required]
        public string PartyName { get; set; }
        [Required]
        public DateTime PartyDate { get; set; }
        public DateTime CreationDate { get; set; }
        [Required]
        public MusicGenre MusicGenre { get; set; }
        [Required]
        public Location Location { get; set; }
        [Required]
        public PartyType PartyType { get; set; }
        [Required]
        public User Host { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
