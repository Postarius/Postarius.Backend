using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;
using Web.Common.App;
using Web.Common.Controllers;

namespace Web.Public.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : UsersControllerBase
    {
        public UsersController(IUsersRepository usersRepository,
            IUserService userService,
            ILogger<UsersControllerBase>logger,
            IUnitOfWork unitOfWork,
            IUserContextProvider userContextProvider) :
            base(usersRepository, userService, logger, unitOfWork, userContextProvider)
        {
        }
    }
}