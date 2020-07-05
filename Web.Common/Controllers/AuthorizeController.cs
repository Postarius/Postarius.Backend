using System;
using System.Security.Claims;
using Data;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;
using Services.Utils;
using Web.Common.App;
using Web.Common.Models.Authorize;

namespace Web.Common.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorizeController : ControllerBase
    {
        private IUsersRepository UsersRepository { get; }
        private IPasswordHashingService PasswordHashingService { get; }
        private ILogger<AuthorizeController> Logger { get; }
        private IJwtService JwtService { get; }
        private IContextProvider ContextProvider { get; }
        private IUserService UserService { get; }
        private IUnitOfWork UnitOfWork { get; }

        public AuthorizeController(IUsersRepository usersRepository,
            IPasswordHashingService passwordHashingService,
            ILogger<AuthorizeController> logger,
            IJwtService jwtService,
            IContextProvider contextProvider,
            IUserService userService,
            IUnitOfWork unitOfWork)
        {
            UsersRepository = usersRepository;
            PasswordHashingService = passwordHashingService;
            Logger = logger;
            JwtService = jwtService;
            ContextProvider = contextProvider;
            UserService = userService;
            UnitOfWork = unitOfWork;
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            var userInfo = UsersRepository.FirstOrDefault(u => u.Login == model.Login, u => new
            {
                u.Password,
                u.Login,
                u.Id,
                u.DisplayName
            });

            if (userInfo is null || PasswordHashingService.Hash(model.Password) != userInfo.Password)
                return Unauthorized("Login or password is incorrect.");

            var token = JwtService.GenerateToken(new[]
            {
                new Claim("login", userInfo.Login),
                new Claim("id", userInfo.Id.ToString()),
                new Claim("isAdministrator", (ContextProvider.Context == ContextType.Admin).ToString()), 
            }, DateTime.UtcNow.AddMinutes(30));

            Logger.LogInformation($"User got {token} token.");
            return Ok(new { Token = token, model.ReturnUrl, userInfo.Login, userInfo.DisplayName, userInfo.Id });
        }

        [HttpGet("Check"), Authorize]
        public ActionResult Check()
        {
            Logger.LogInformation("Successful check.");
            return Ok();
        }

        [HttpPost("CreateAdmin"), AllowAnonymous]
        public ActionResult CreateAdmin()
        {
            var admin = UsersRepository.SingleOrDefault(u => u.Login == "Admin");

            if (admin == null)
            {
                UserService.Save(new User
                {
                    Login = "Admin",
                    Email = "admin@gmail.com",
                    Password = "123"
                });
                
                UnitOfWork.Commit();
            }

            return Ok();
        }
    }
}