using System;
using System.Linq;
using Data;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;
using Web.Admin.Models.Users;
using Web.Common.App;
using Web.Common.Controllers;

namespace Web.Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : UsersControllerBase
    {
        private IUsersRepository UsersRepository { get; }
        private IUserService UserService { get; }
        private IUnitOfWork UnitOfWork { get; }
        private ILogger<UsersController> Logger { get; }
        private IUserContextProvider UserContextProvider { get; }

        public UsersController(IUsersRepository usersRepository,
            IUserService userService,
            IUnitOfWork unitOfWork,
            ILogger<UsersController> logger,
            IUserContextProvider userContextProvider) :
            base(usersRepository, userService, logger, unitOfWork, userContextProvider)
        {
            UsersRepository = usersRepository;
            UserService = userService;
            UnitOfWork = unitOfWork;
            Logger = logger;
            UserContextProvider = userContextProvider;
        }

        [HttpPost("Profile/Edit")]
        public ActionResult Edit(EditModel model)
        {
            var user = UsersRepository.GetById(model.Id, false);

            if (user is null)
                return NotFound($"User with {model.Id} could not be found.");

            user.DisplayName = model.DisplayName;
            user.AvatarUrl = model.AvatarUrl;

            UserService.Save(user);
            UnitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("Create")]
        public ActionResult CreateUser(CreateModel model)
        {
            if (UsersRepository.Any(u => u.Email == model.Email || u.Login == model.Login))
                return BadRequest("User with this email or login already exists");
            
            var user = new User
            {
                Login = model.Login,
                Email = model.Email,
                AvatarUrl = model.AvatarUrl,
                DisplayName = model.DisplayName,
                Password = model.Password
            };
            
            UserService.Save(user);
            UnitOfWork.Commit();

            return Ok();
        }
    }
}