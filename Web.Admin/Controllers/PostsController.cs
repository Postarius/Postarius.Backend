using System;
using System.Linq;
using Data;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Web.Common.App;
using Web.Common.Controllers;
using Web.Common.Models.Posts;

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

        [HttpGet("Finalized")]
        public ActionResult FinalizedPosts()
        {
            var postsQuery = PostsRepository.GetByStatus(PostStatus.Finalized);

            return Ok(new ListModel
            {
                Posts = postsQuery.Select(p => new ListModel.PostModel
                {
                    Description = p.Description,
                    Id = p.Id,
                    Title = p.Title,
                    PrimaryImageUrl = p.ImageUrls.First()
                })
            });
        }

        [HttpPost("Approve/{id}")]
        public ActionResult Approve(int id)
        {
            if (!PostsRepository.Exists(id))
                return NotFound(id);
            
            PostService.ChangeStatus(id, PostStatus.Approved);
            UnitOfWork.Commit();

            return Ok();
        }

        [HttpPost("Disapprove/{id}")]
        public ActionResult Disapprove(int id)
        {
            if (!PostsRepository.Exists(id))
                return NotFound(id);
            
            PostService.ChangeStatus(id, PostStatus.Disapproved);
            UnitOfWork.Commit();

            return Ok();
        }
    }
}