using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using UserServer.Database;
using UserServer.Models;
using System.Web.Http;

namespace UserServer.Controllers
{
    [Route("api/User")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger _logger;
        private DatabaseContext _dbContext;

        public UserController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILoggerFactory loggerFactory,
            DatabaseContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<UserController>();

            _dbContext = dbContext;
        }

        //  POST /api/User
        /// <summary>
        /// Register/Create new user
        /// </summary>
        /// <remarks>This function will create a new user in the user database.</remarks>
        /// <param name="value">Login-Model</param>
        /// <returns>Http Status Code 201 (Created) and the newly created User ID, or Http Status Code 400 (Bad Request) if something went wrong.</returns>
        [ProducesResponseType(typeof(Login), 400)]
        [HttpPost]
        public async Task<ActionResult> Register([FromBody]Login value)
        {
            var user = new User { UserName = value.Username, Email = value.Email };

            var result = await _userManager.CreateAsync(user, value.Password);

            if (result.Succeeded)
            {
                var t = await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("email", user.Email));
                await _signInManager.SignInAsync(user, false);
                return Created("", user.Id);
            }
            else
            {
                return BadRequest(new Login());
            }
        }

        // DELETE /api/User
        /// <summary>
        /// Delete user
        /// </summary>
        /// <remarks>This function will delete the current user from the database. It's necessary to send a valid token!</remarks>
        /// <param name="id">User ID</param>
        /// <returns>Http Status Code 401 (Unauthorized) if the token isn't valid, or Http Status Code 404 (Not found) if the user won't exist in the database,
        /// or Http Status Code 400 (Bad Request) if user cannot be deleted, or Http Status Code 200 (Ok) if everything works well.</returns>
        [HttpDelete("id={id}")]
        //[HttpDelete]
        [Authorize]
        public async Task<ActionResult> Delete(Guid? id)
        {
            try
            {
                //Check if the id is valid
                Guid userID;
                if(!Guid.TryParse(id.ToString(), out userID))
                {
                    //User ID is not valid
                    return BadRequest("User ID is not valid.");
                }
                else
                {
                    var user = await _userManager.FindByIdAsync(userID.ToString());
                    if(user == null)
                    {
                        return NotFound("User not found.");
                    }
                    else
                    {
                        var result = await _userManager.DeleteAsync(user);

                        if(!result.Succeeded)
                        {
                            //TODO Can't delete user
                            return BadRequest();
                        } 
                        else
                        {
                            return Ok();
                        }
                    }
                }
            }
            catch(Exception)
            {
                return BadRequest();
            }
        }
    }
}
