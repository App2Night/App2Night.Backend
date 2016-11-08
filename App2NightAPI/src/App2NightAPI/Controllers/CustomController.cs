using App2NightAPI.Models;
using App2NightAPI.Models.Authentification;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App2NightAPI.Controllers
{
    public class CustomController : Controller 
    {
        private DatabaseContext _dbContext;
        private IUserService _userService;

        public CustomController(DatabaseContext dbContext, IUserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;
        }

        //[ApiExplorerSettings(IgnoreApi = true)]
        protected new User User => _userService.GetUser(_dbContext, base.User);
    }
}
