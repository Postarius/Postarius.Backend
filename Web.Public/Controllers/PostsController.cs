using System.Linq;
using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Services;
using Web.Common.App;
using Web.Common.Controllers;
using Web.Public.Models.Posts;
using Web.Common.App.Attributes;

namespace Web.Public.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpPost("Create")]
        public ActionResult Create(CreateModel model)
        {
            var post = new Post
            {
                Description = model.Description,
                Title = model.Title,
                ImageUrls = model.ImageUrls.ToList(),
                OwnerId = UserContextProvider.Get().Id,
                Status = PostStatus.InProgress
            };
            
            PostService.Save(post);
            UnitOfWork.Commit();

            return Created(Url.Action("Details", new {id = post.Id}), model);
        }

        [HttpPost("CreateAndFinalize")]
        public ActionResult CreateAndFinalize(CreateModel model)
        {
            var post = new Post
            {
                Description = model.Description,
                Title = model.Title,
                ImageUrls = model.ImageUrls.ToList(),
                OwnerId = UserContextProvider.Get().Id,
                Status = PostStatus.Finalized
            };
            
            PostService.Save(post);
            UnitOfWork.Commit();

            return Created(Url.Action("Details", new {id = post.Id}), model);
        }

        [HttpPatch("Finalize/{id}")]
        [HasAccessToPost]
        public ActionResult Finalize(int id)
        {
            PostService.ChangeStatus(id, PostStatus.Finalized);
            UnitOfWork.Commit();

            return Ok();
        }
    }
}