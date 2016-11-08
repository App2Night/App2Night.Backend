using App2NightAPI.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App2NightAPI.Models.Model_Abstraction
{
    public interface IPartyBase
    {
        string PartyName { get; set; }
        DateTime PartyDate { get; set; }
        MusicGenre MusicGenre { get; set; }
        PartyType PartyType { get; set; }
        string Description { get; set; }
    }
}
