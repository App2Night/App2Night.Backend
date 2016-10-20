using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App2NightAPI.Models.REST_Models
{
    public class CreateParty
    {
        public string PartyName { get; set; }

        public DateTime PartyDate { get; set; }

        public DateTime CreationDate { get; set; }

        // TODO Musik-Enums

        // TODO Teilnahme

        // TODO HostID
    }
}
