using Data;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Web.Common.App;
using Web.Common.Controllers;

namespace Web.Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PostsController : PostsControllerBase
    {
        private IPostsRepository PostsRepository { get; }
        private IPostService PostService { get; }
        private IUnitOfWork UnitOfWork { get; }
        private IUserContextProvider UserContextProvider { get; }

        public PostsController(IPostsRepository postsRepository,
            IPostService postService,
            IUnitOfWork unitOfWork,
            IUserContextProvider userContextProvider) :
            base(postsRepository, postService, unitOfWork, userContextProvider)
        {
            PostsRepository = postsRepository;
            PostService = postService;
            UnitOfWork = unitOfWork;
            UserContextProvider = userContextProvider;
        }

        [HttpPost("Approve/{id}")]
        public ActionResult Approve(int id)
        {
            PostService.ChangeStatus(id, PostStatus.Approved);
            UnitOfWork.Commit();

            return Ok();
        }
    }
}