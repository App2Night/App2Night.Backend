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
using UserServer.Services;

namespace UserServer.Controllers
{
    [Route("api/User")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private DatabaseContext _dbContext;

        public UserController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IEmailSender emailSender,
            ILoggerFactory loggerFactory,
            DatabaseContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
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
            if (user.Email.Contains("@"))
            {
                var result = await _userManager.CreateAsync(user, value.Password);

                if (result.Succeeded)
                {
                    var t = await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("email", user.Email));
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "User", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

                    //Use BodyBuilder for message with HTML Code
                    string message = "Hello " + user.UserName + "<br/><br/>" + $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a> <br/><br/> Best greets from App2Night-Team";
                    await _emailSender.SendEmailAsync(value.Email, "Confirm your App2Night-Account", message);
                    
                    return Created("","E-Mail sent to user email adress.");
                }
                else
                {
                    //TDOD Error handling if username is a duplicate
                    //TODO Error handling if email is a duplicate
                    return BadRequest(new Login());
                }
            }
            else
            {
                return BadRequest("Email is not valid.");
            }
        }

        // GET : /User/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if(userId == null || code == null)
            {
                return BadRequest("UserId or Code is null");
            }
            else
            {
                var user = await _userManager.FindByIdAsync(userId);
                if(user == null)
                {
                    return BadRequest("Can't find user by given Id");
                }
                else
                {
                    //Get the confirmed token
                    //var confirmToke = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //Confirm email by user and token
                    var userConfirmationResult = await _userManager.ConfirmEmailAsync(user, code);
                    if(!userConfirmationResult.Succeeded)
                    {
                        return BadRequest("Can't confirm user token to email");
                    }
                    else
                    {
                        return Ok();
                    }
                }
            }
        }

        //// GET: /Account/ConfirmEmail
        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        return View("Error");
        //    }
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null)
        //    {
        //        return View("Error");
        //    }
        //    var result = await _userManager.ConfirmEmailAsync(user, code);
        //    return View(result.Succeeded ? "ConfirmEmail" : "Error");
        //}

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
