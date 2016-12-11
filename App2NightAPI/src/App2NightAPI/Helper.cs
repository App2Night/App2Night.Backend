using App2NightAPI.Models;
using App2NightAPI.Models.Authentification;
using App2NightAPI.Models.REST_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App2NightAPI
{
    /// <summary>
    /// This class will provide help functions which are needed in different classes.
    /// </summary>
    public class Helper
    {
        private User _user;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="user">Current User</param>
        public Helper(User user)
        {
            _user = user;
        }

        /// <summary>
        /// Function will check the location and map a CreatePartyModel-Object to a exsiting PartyModel-Object
        /// </summary>
        /// <param name="value">CreateParty-Object</param>
        /// <param name="party">Party-Object</param>
        public void MapPartyToModel(CreateParty value, ref Party party)
        {
            Location loc = null;
            Task.WaitAll(Task.Run(async () => loc = await GeoCoding.GetLocationByAdress(value.HouseNumber, value.StreetName, value.CityName)));
            if (loc == null)
            {
                throw new Exception("Location not found.");
            }

            party.PartyName = value.PartyName;
            party.PartyDate = value.PartyDate.Millisecond == 0 ? value.PartyDate.AddMilliseconds(01.123) : value.PartyDate;
            party.MusicGenre = value.MusicGenre;
            party.Location = new Location()
            {
                CityName = value.CityName,
                HouseNumber = value.HouseNumber,
                StreetName = value.StreetName,
                CountryName = value.CountryName,
                Zipcode = value.Zipcode,
                Latitude = loc.Latitude,
                Longitude = loc.Longitude,
            };
            party.PartyType = value.PartyType;
            party.Description = value.Description;
            party.Price = value.Price;
        }

        /// <summary>
        /// Function will check the location and map the CreateParty-Object to a new Party-Object
        /// and add the current user as host of the party.
        /// </summary>
        /// <param name="value">CreateParty-Object</param>
        /// <returns></returns>
        public Party MapPartyToModel(CreateParty value)
        {
            var party = new Party();
            MapPartyToModel(value, ref party);
            party.Host = _user;
            party.CreationDate = DateTime.Today;
            return party;
        }
    }
}
