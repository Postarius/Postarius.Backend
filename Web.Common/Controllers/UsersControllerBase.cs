using System;
using System.Linq;
using Common;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;
using Web.Common.App;
using Web.Common.Models.Users;

namespace Web.Common.Controllers
{
    public class UsersControllerBase : ControllerBase
    {
        private IUsersRepository UsersRepository { get; }
        private IUserService UserService { get; }
        private ILogger<UsersControllerBase> Logger { get; }
        private IUnitOfWork UnitOfWork { get; }
        private IUserContextProvider UserContextProvider { get; }

        public UsersControllerBase(IUsersRepository usersRepository,
            IUserService userService,
            ILogger<UsersControllerBase> logger,
            IUnitOfWork unitOfWork,
            IUserContextProvider userContextProvider)
        {
            UsersRepository = usersRepository;
            UserService = userService;
            Logger = logger;
            UnitOfWork = unitOfWork;
            UserContextProvider = userContextProvider;
        }
        
        [HttpGet("List")]
        public ActionResult List(int skip = 0, int take = 10, string displayName = null, string login = null,
            string email = null)
        {
            var searchParams = new ListSearchParams
            {
                Skip = skip,
                Take = take,
                DisplayName = displayName,
                Login = login,
                Email = email
            };

            var userInfos = UsersRepository.Search(searchParams).Select(u => new UserInfo
            {
                Email = u.Email,
                DisplayName = u.DisplayName,
                Id = u.Id,
                AvatarUrl = u.AvatarUrl
            });

            var totalEntries = UsersRepository.Count(searchParams.BuildExpression());
            var pageNumber = skip / take;
            var totalPageCount = Math.Ceiling((decimal) totalEntries / take).AsInt();

            var model = new ListModel
            {
                UserInfos = userInfos,
                SearchParams = searchParams,
                TotalEntries = totalEntries,
                TotalPageCount = totalPageCount,
                PageNumber = pageNumber
            };

            return Ok(model);
        }

        [HttpGet("Profile/{id}")]
        public ActionResult Profile(int id)
        {
            var userInfo = UsersRepository.GetById(id, ProfileModel.GetProfileDataExpression, false);

            if (userInfo is null)
                return NotFound($"User with {id} id not found");

            return Ok(userInfo);
        }
        
        [HttpGet("MyProfile")]
        public ActionResult MyProfile()
        {
            var userId = UserContextProvider.Get().Id;

            var userInfo = UsersRepository.GetById(userId, ProfileModel.GetProfileDataExpression);

            return Ok(userInfo);
        }

        [HttpGet("Followers")]
        public ActionResult Followers()
        {
            var userId = UserContextProvider.Get().Id;
            var followers = UsersRepository.GetFollowersByUserId(userId);

            return Ok(new FollowersModel
            {
                Followers = followers.Select(f => new UserInfo
                {
                    Id = f.Id,
                    Email = f.Email,
                    DisplayName = f.DisplayName,
                    AvatarUrl = f.AvatarUrl
                })
            });
        }

        [HttpGet("AlreadyFollowing")]
        public ActionResult AlreadyFollowing([FromQuery] int userId)
        {
            var id = UserContextProvider.Get().Id;
            return Ok(UsersRepository.GetFollowedByUserId(id).Select(f => f.Id).Contains(userId));
        }

        [HttpPost("Follow")]
        public ActionResult Follow(FollowModel model)
        {
            var userId = UserContextProvider.Get().Id;
            UserService.CreateSubscription(userId, model.UserToFollowId);
            UnitOfWork.Commit();

            return Ok();
        }
    }
}