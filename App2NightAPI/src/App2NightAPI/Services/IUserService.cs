using App2NightAPI.Models;
using App2NightAPI.Models.Authentification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace App2NightAPI
{
    public interface IUserService
    {
        Guid UserID { get; }
        Guid Name { get; }

        User GetUser(DatabaseContext dbContext, ClaimsPrincipal user);
    }
}
