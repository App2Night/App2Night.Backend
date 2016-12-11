using App2NightAPI.Models;
using App2NightAPI.Models.Authentification;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App2NightAPI.Controllers
{
    /// <summary>
    /// This Controller is the main controller. All other controllers inherit from this controller. This controller provides the connection to the User Server via UserService.
    /// </summary>
    public class CustomController : Controller 
    {
        private DatabaseContext _dbContext;
        private IUserService _userService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">Database Context</param>
        /// <param name="userService">User Service for interaction with User Server</param>
        public CustomController(DatabaseContext dbContext, IUserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;
        }

        /// <summary>
        /// Returns the current user from the User Server via UserSerivce.
        /// </summary>
        protected new User User => _userService.GetUser(_dbContext, base.User);
    }
}
