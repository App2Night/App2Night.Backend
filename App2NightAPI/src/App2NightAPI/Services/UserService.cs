using App2NightAPI.Models.Authentification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using App2NightAPI.Models;

namespace App2NightAPI
{
    public class UserService : IUserService
    {
        public Guid UserID { get; }
        public Guid Name { get; }

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
