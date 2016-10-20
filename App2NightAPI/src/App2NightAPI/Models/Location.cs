using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App2NightAPI.Models
{
    public class Location
    {
        [Key]
        public int LocationId { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public string StreetName { get; set; }
        public int HouseNumber { get; set; }
        public string HouseNumberAdditional { get; set; }
        public int Zipcode { get; set; }
        public long Latitude { get; set; }
        public long Longitude { get; set; }
    }
}
