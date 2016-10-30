using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App2NightAPI.Models.Authentification;
using App2NightAPI.Models.Enum;
using App2NightAPI.Models.REST_Models;
using Newtonsoft.Json;

namespace App2NightAPI.Models
{
    public class Party : CreateParty
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid PartId { get; set; }
        [JsonIgnore]
        public DateTime CreationDate { get; set; }
        public int Price { get; set; }
        [JsonIgnore]
        public User Host { get; set; }
        //public DateTime CreationDate { get; set; }
        //[Required]
        //public User Host { get; set; }
        //public MusicGenre MusicGenre { get; set; }
        //[Required]
        //public Location Location { get; set; }
        //[Required]
        //public string Description { get; set; }
        //[Required]
        //public PartyType PartyType { get; set; }
    }
}
