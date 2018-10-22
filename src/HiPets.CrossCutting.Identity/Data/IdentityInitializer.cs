using HiPets.CrossCutting.Identity.Models;
using HiPets.CrossCutting.Identity.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;

namespace HiPets.CrossCutting.Identity.Data
{
    public class IdentityInitializer
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public IdentityInitializer(UserManager<ApplicationUser> userManager, ApplicationContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        
        public void Initialize()
        {
            _context.Database.Migrate();
            _context.Database.EnsureCreated();

            CreateRole(Roles.ADMIN);
            CreateRole(Roles.ADOPTER);

            var adminUser = new ApplicationUser()
            {
                UserName = "Admin",
                Email = "admin@teste.com"
            };

            var adopterUser = new ApplicationUser()
            {
                UserName = "Joao",
                Email = "js@gmail.com"
            };

            CreateUser(adminUser, "Admin@2018", Roles.ADMIN);
            AddRoleAsync(adminUser, Roles.ADMIN);

            CreateUser(adopterUser, "Joao@2018", Roles.ADOPTER);          
            AddRoleAsync(adopterUser, Roles.ADOPTER);
        }

        private void CreateRole(string role)
        {
            if (!_roleManager.RoleExistsAsync(role).Result)
            {
                var resultado = _roleManager.CreateAsync(
                    new IdentityRole(role)).Result;
                if (!resultado.Succeeded)
                {
                    throw new Exception(
                        $"Error in creation of {role}.");
                }
            }
        }

        private void AddRoleAsync(ApplicationUser user, string role)
        {
            if (user != null && role != null)
            {
                user = _userManager.FindByNameAsync(user.UserName).Result;

                var roles = _userManager.GetRolesAsync(user).Result;

                if (roles == null || roles.Count == 0)
                    _userManager.AddToRoleAsync(user, role).Wait();

            }
        }

        private void CreateUser(ApplicationUser user, string password, string role)
        {
            if (_userManager.FindByNameAsync(user.UserName).Result == null)
            {
                var result = _userManager.CreateAsync(user, password).Result;

                if (result.Succeeded && role != null)
                {
                    _userManager.AddToRoleAsync(user, role).Wait();
                }
            }
        }
    }
}
