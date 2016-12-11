using App2NightAPI.Models.Authentification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using App2NightAPI.Models;

namespace App2NightAPI
{
    /// <summary>
    /// This class provides the communication between the API and the User Server.
    /// </summary>
    public class UserService : IUserService
    {
        /// <summary>
        /// Returns the UserID of the current user.
        /// </summary>
        public Guid UserID { get; }
        /// <summary>
        /// Returns the Name of the current user.
        /// </summary>
        public Guid Name { get; }

        /// <summary>
        /// Transform the current user from the User Server and store relevant information in the database of the API.
        /// </summary>
        /// <param name="dbContext">Database Context</param>
        /// <param name="pUser">Current User as ClaimsPrincipal</param>
        /// <returns></returns>
        public User GetUser(DatabaseContext dbContext, ClaimsPrincipal pUser)
        {
            try
            {
                bool hasChanged = false;
                var userID = Guid.Parse(pUser.Claims.Where(o => o.Type == "sub").Select(p => new { p.Value }).FirstOrDefault().Value);
                var userName = pUser.Claims.Where(o => o.Type == "name").Select(p => new { p.Value }).FirstOrDefault().Value;
                var email = pUser.Claims.Where(o => o.Type == "email").Select(p => new { p.Value }).FirstOrDefault().Value;

                //Check if user already exists
                var user = dbContext.UserItems.FirstOrDefault(u => u.UserId == userID);

                if (user == null)
                {
                    //Create new user
                    user = new User()
                    {
                        UserId = userID,
                        UserName = userName,
                        Email = email,
                    };

                    dbContext.UserItems.Add(user);
                    dbContext.SaveChanges();
                }
                else
                {
                    //User exists already in the databsae
                    //Check if user changed
                    if (user.UserName != userName)
                    {
                        user.UserName = userName;
                        hasChanged = true;
                    }
                    if (user.Email != email)
                    {
                        user.Email = email;
                        hasChanged = true;
                    }

                    if (hasChanged)
                    {
                        dbContext.UserItems.Update(user);
                        dbContext.SaveChanges();
                    }
                }

                return user;
            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}
