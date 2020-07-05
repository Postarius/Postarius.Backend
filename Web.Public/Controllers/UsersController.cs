using System;
using Common;
using Data;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;
using Web.Common.App;
using Web.Common.Controllers;
using Web.Public.Models.Users;

namespace Web.Public.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : UsersControllerBase
    {
        private IUsersRepository UsersRepository { get; }
        private IUserService UserService { get; }
        private ILogger<UsersControllerBase> Logger { get; }
        private IUnitOfWork UnitOfWork { get; }
        private IUserContextProvider UserContextProvider { get; }

        public UsersController(IUsersRepository usersRepository,
            IUserService userService,
            ILogger<UsersControllerBase> logger,
            IUnitOfWork unitOfWork,
            IUserContextProvider userContextProvider) :
            base(usersRepository, userService, logger, unitOfWork, userContextProvider)
        {
            UsersRepository = usersRepository;
            UserService = userService;
            Logger = logger;
            UnitOfWork = unitOfWork;
            UserContextProvider = userContextProvider;
        }

        [HttpPost(nameof(Register)), AllowAnonymous]
        public IActionResult Register(RegisterModel model)
        {
            var existingUser = UsersRepository.GetByEmail(model.Email);

            if (existingUser != null)
                return BadRequest($"User with {model.Email} email already exists.");

            existingUser = UsersRepository.GetByLogin(model.Login);

            if (existingUser != null)
                return BadRequest($"User with {model.Login} login already exists.");
            
            var user = new User
            {
                Login = model.Login,
                DisplayName = model.DisplayName,
                Email = model.Email,
                AvatarUrl = Consts.DefaultAvatarUrl,
                Password = model.Password
            };
            
            UserService.Save(user);
            UnitOfWork.Commit();

            return NoContent();
        }
    }
}