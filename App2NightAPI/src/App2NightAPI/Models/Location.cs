using App2NightAPI.Models.Enum;
using App2NightAPI.Models.REST_Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace App2NightAPI.Models
{
    public class Location
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [JsonIgnore]
        public int LocationId { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public string StreetName { get; set; }
        public string HouseNumber { get; set; }
        public string HouseNumberAdditional { get; set; }
        public string Zipcode { get; set; }
        public long Latitude { get; set; }
        public long Longitude { get; set; }
        
    }
}
