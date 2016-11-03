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

        public User GetUser(DatabaseContext dbContext, ClaimsPrincipal user)
        {
            User usr = new User();
            usr.UserId = Guid.Parse(user.Claims.Where(o => o.Type == "sub").Select(p => new { p.Value }).FirstOrDefault().Value);
            usr.UserName = user.Claims.Where(o => o.Type == "name").Select(p => new { p.Value }).FirstOrDefault().Value;
            usr.Email = user.Claims.Where(o => o.Type == "email").Select(p => new { p.Value }).FirstOrDefault().Value;

            //Check if user already exists
            int usrCount = dbContext.UserItems.Where(u => u.UserId == usr.UserId).Count();
            if(usrCount == 0)
            {
                //User is not in the databse
                //Create User
                dbContext.UserItems.Add(usr);
                dbContext.SaveChanges();
            }
            else
            {
                //User exists already in the databsae
                //Check if user changed
                if(!dbContext.UserItems.Contains(usr))
                {
                    //User exists in the database but has changed
                    dbContext.UserItems.Update(usr);
                    dbContext.SaveChanges();   
                }
                //else user exists and has not changed
                //Select the partys where the current user is the host
                usr.PartyHostedByUser = dbContext.PartyItems.Where(p => p.Host == usr).ToList();
            }

            return usr;
        }
    }
}
