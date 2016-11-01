using App2Night.Shared;
using IdentityServer4.Models;
using IdentityServer4.Services.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace UserServer
{
    public class Config
    {
        public static IEnumerable<Scope> GetScopes()
        {
            return new List<Scope>
            {
                StandardScopes.OfflineAccess,
                StandardScopes.OpenId,
                StandardScopes.Email,
                new Scope
                {
                    Name = "App2NightAPI",
                    Description = "My API",
                    Type = ScopeType.Resource,
                     IncludeAllClaimsForUser = true,
                     ScopeSecrets = new List<Secret>()
                     {
                        new Secret(new Secrets().APISecret.Sha256())
                     },
                }
                //StandardScopes.Profile
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {

                new Client
                {
                    ClientId = "nativeApp",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AccessTokenType = AccessTokenType.Reference,
                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret(new Secrets().ClientSecret.Sha256())
                    }, 
                    // scopes that client has access to
                    AllowedScopes = new List<string>()
                    {
                        "App2NightAPI",
                        StandardScopes.OfflineAccess.Name,
                        StandardScopes.OpenId.Name,
                        StandardScopes.Email.Name
                        //StandardScopes.Profile.Name
                    },

                }
            };
        }

        //public static List<InMemoryUser> GetUsers()
        //{
            //return new List<InMemoryUser>
            //{
            //    new InMemoryUser
            //    {
            //        Subject = "1",
            //        Username = "alice",
            //        Password = "password"
            //    },
            //    new InMemoryUser
            //    {
            //        Subject = "2",
            //        Username = "bob",
            //        Password = "password",
            //        Claims = new[]
            //        {
            //            new Claim("Name", "Bobii"),
            //            new Claim(ClaimTypes.Name, "Bobss"),
            //            new Claim(ClaimTypes.GivenName, "Testname")
            //        }
            //    }
            //};
        //}
    }
}
