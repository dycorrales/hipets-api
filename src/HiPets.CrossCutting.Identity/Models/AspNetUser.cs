using System;
using System.Collections.Generic;
using System.Security.Claims;
using HiPets.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace HiPets.CrossCutting.Identity.Models
{
    public class AspNetUser : IUser
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public AspNetUser(UserManager<ApplicationUser> userManager, IHttpContextAccessor accessor)
        {
            _accessor = accessor;
            _userManager = userManager;
        }

        public string Name => _accessor.HttpContext.User.Identity.Name;

        public IEnumerable<Claim> GetClaimsIdentity() => _accessor.HttpContext.User.Claims;

        public bool IsAuthenticated() => _accessor.HttpContext.User.Identity.IsAuthenticated;

        public Guid GetUserId() => IsAuthenticated()
            ? Guid.Parse(_accessor.HttpContext.User.GetUserId())
            : Guid.NewGuid();

        public bool IsAdminUser
        {
            get
            {
                var user = _userManager.GetUserAsync(_accessor.HttpContext.User).Result;

                if (user == null)
                    return false;

                var roles = _userManager.GetRolesAsync(user).Result;

                return roles.Contains("Admin");
            }
        }

        public int? GetRole()
        {
            var user = _userManager.GetUserAsync(_accessor.HttpContext.User).Result;
            var role = _userManager.GetRolesAsync(user).Result[0];

            if (role.Equals(Roles.ADMIN))
            {
                return (int)Role.ADMIN;
            }
            if (role.Equals(Roles.ADOPTER))
            {
                return (int)Role.ADOPTER;
            }
            return null;
        }
    }
}
