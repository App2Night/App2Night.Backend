using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using App2NightAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using App2NightAPI.Models.Authentification;

namespace App2NightAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ValuesController : CustomController
    {
        private DatabaseContext _dbContext;
        //private IUserService _userService;

        public ValuesController(DatabaseContext dbContext, IUserService userService) : base(dbContext, userService)
        {
            _dbContext = dbContext;
            //_userService = userService;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var test = GetUser();
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public Party Get(int id)
        {
            return _dbContext.PartyItems.ToList<Party>()[0];
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        [HttpPatch]
        public int Patch(int id)
        {
            return id;
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
