using System;
using System.Linq;
using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Services;
using Web.Common.App;
using Web.Common.Models.Posts;

namespace Web.Common.Controllers
{
    public class PostsControllerBase : ControllerBase
    {
        private IPostsRepository PostsRepository { get; }
        private IPostService PostService { get; }
        private IUnitOfWork UnitOfWork { get; }
        private IUserContextProvider UserContextProvider { get; }

        public PostsControllerBase(IPostsRepository postsRepository,
            IPostService postService,
            IUnitOfWork unitOfWork,
            IUserContextProvider userContextProvider)
        {
            PostsRepository = postsRepository;
            PostService = postService;
            UnitOfWork = unitOfWork;
            UserContextProvider = userContextProvider;
        }

        [HttpGet("Details/{id}")]
        public ActionResult Details(int id)
        {
            var post = PostsRepository.GetById(id, p => new DetailsModel
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                ImageUrls = p.ImageUrls,
                CreatedAt = p.Edited ? p.CreatedAt : (DateTime?)null,
                Edited = p.Edited,
                EditedAt = p.Edited ? p.UpdatedAt : (DateTime?)null
            });

            return Ok(post);
        }

        [HttpGet]
        public ActionResult Status(PostStatus? status)
        {
            var query = PostsRepository
                .GetPostsByUserId(UserContextProvider.Get().Id);
                
            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);
            
            query = query
                .OrderByDescending(p => p.UpdatedAt)
                .ThenByDescending(p => p.CreatedAt);

            var posts = query.ToArray();

            return Ok(new ListModel
            {
                Posts = posts.Select(p => new ListModel.PostModel
                {
                    Title = p.Title,
                    PrimaryImageUrl = p.ImageUrls.First(),
                    Description = p.Description,
                    Id = p.Id
                })
            });
        }
    }
}