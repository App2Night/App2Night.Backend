using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace App2NightAPI
{
    public class Validator
    {
        public static bool IsGuidValid(string id)
        {
            Guid parsedId;
            if(Guid.TryParse(id.ToString(), out parsedId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
