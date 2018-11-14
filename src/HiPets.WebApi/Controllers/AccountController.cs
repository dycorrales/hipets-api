using HiPets.CrossCutting.Identity.Authorization;
using HiPets.CrossCutting.Identity.Models;
using HiPets.Domain.Entities;
using HiPets.Domain.Helpers;
using HiPets.Domain.Interfaces;
using HiPets.Domain.Notifications;
using HiPets.WebApi.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HiPets.WebApi.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        private readonly IMediatorBus _mediator;
        private readonly TokenDescriptor _tokenDescriptor;
        private readonly IAdopterRepository _adopterRepository;
        private readonly IUnitOfWork _unitOfWork;

        private static long ToUnixEpochDate(DateTime date) => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, TokenDescriptor tokenDescriptor, IUser user, IMediatorBus mediator, IAdopterRepository adopterRepository, INotificationHandler<DomainNotification> notifications, ILoggerFactory loggerFactory, IUnitOfWork unitOfWork) : base(mediator, notifications, loggerFactory, user)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _mediator = mediator;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _tokenDescriptor = tokenDescriptor;
            _adopterRepository = adopterRepository;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        [SwaggerOperation(Summary = "Login", Tags = new[] { "Security" })]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    NotifyInvalidModelError();
                    return RequestResponse(HttpStatusCode.BadRequest, isError: true, result: "Os dados fornecidos são inválidos");
                }

                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                {
                    return RequestResponse(HttpStatusCode.BadRequest, isError: true, result: "Os dados fornecidos são inválidos");
                }

                var userIdentity = _userManager.FindByEmailAsync(model.Email).Result;

                if(userIdentity == null)
                {
                    var id = Guid.NewGuid();

                    CreateAspNetUser(id, model.Email, model.Password);
                    
                    var name = model.Email.Split('@')[0];

                    _adopterRepository.Insert(new Adopter(id, name, "00000000000", model.Email));
                    var response = _unitOfWork.Commit();

                    if (response.Success)
                        userIdentity = _userManager.FindByEmailAsync(model.Email).Result;
                }

                if (userIdentity != null)
                {
                    var user = _adopterRepository.Find(adopter => adopter.Id == new Guid(userIdentity.Id)).FirstOrDefault();

                    var userActive = true;

                    if (user != null)
                        userActive = user.Status == Status.Active;

                    if (userActive)
                    {
                        var result = await _signInManager.CheckPasswordSignInAsync(userIdentity, model.Password, false);
                        object accessToken = null;

                        if (result.Succeeded)
                        {
                            var info = JsonConvert.SerializeObject($"Usuário {user.ContactInfo.Email} logado com sucesso!!");
                            _logger.LogInformation(info);

                            accessToken = await GenerateToken(userIdentity);

                            return RequestResponse(HttpStatusCode.OK, result: accessToken);
                        }
                    }
                }

                var message = $"Erro ao fazer login com usuário {userIdentity.Email}";

                var error = JsonConvert.SerializeObject($"Usuário {userIdentity.Email} não autorizado");
                _logger.LogError(error);

                return RequestResponse(HttpStatusCode.Unauthorized, isError: true);
            }
            catch (Exception ex)
            {
                var error = JsonConvert.SerializeObject(ex);
                _logger.LogError(error);

                return RequestResponse(HttpStatusCode.BadRequest, isError: true, result: "Ocorreu um erro ao fazer login");
            }
        }

        private void CreateAspNetUser(Guid id, string email, string password)
        {
            var username = email.Split('@')[0];

            var user = new ApplicationUser()
            {
                Id = id.ToString(),
                Email = email,
                UserName = username
            };

            var findedUser = _userManager.FindByEmailAsync(user.Email).Result;

            if (findedUser == null)
            {
                var result = _userManager.CreateAsync(user, password).Result;

                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, Role.ADOPTER.ToString()).Wait();
                }
            }
        }

        private async Task<object> GenerateToken(ApplicationUser userIdentity)
        {
            var userClaims = await _userManager.GetClaimsAsync(userIdentity);

            var roles = await _userManager.GetRolesAsync(userIdentity);

            userClaims.Add(new Claim(JwtRegisteredClaimNames.Sub, userIdentity.Id));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Email, userIdentity.Email));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

            foreach (var role in roles)
                userClaims.Add(new Claim(ClaimTypes.Role, role));

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(userClaims);

            var handler = new JwtSecurityTokenHandler();
            var signingConf = new SigningCredentialsConfiguration();

            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _tokenDescriptor.Issuer,
                Audience = _tokenDescriptor.Audience,
                SigningCredentials = signingConf.SigningCredentials,
                Subject = identityClaims,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(_tokenDescriptor.MinutesValid)
            });
                       
            var encodedJwt = handler.WriteToken(securityToken);

            var response = new
            {
                access_token = encodedJwt,
                userId = userIdentity.Id,
                isAdmin = roles.FirstOrDefault(r => r == "Admin") != null
            };

            return response;
        }
    }
}