using App2NightAPI.Models.Authentification;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace App2NightAPI
{
    public class RessourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;

        public RessourceOwnerPasswordValidator(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var result = await _signInManager.PasswordSignInAsync(context.UserName, context.Password, isPersistent:true, lockoutOnFailure: false);
            if(result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(context.UserName);
                var claims = user.Claims;
                context.Result = new GrantValidationResult(subject: user.Id.ToString(), authenticationMethod: "default", claims: null);
            }
            else
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid credentials.");
            }                
        }
    }
}
