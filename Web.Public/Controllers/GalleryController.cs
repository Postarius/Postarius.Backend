using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Web.Public.Models.Gallery;

namespace Web.Public.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GalleryController : ControllerBase
    {
        private IPostsRepository PostsRepository { get; }

        public GalleryController(IPostsRepository postsRepository)
        {
            PostsRepository = postsRepository;
        }

        [HttpGet("List")]
        public ActionResult List()
        {
            var posts = PostsRepository.GetByStatus(PostStatus.Approved);

            return Ok(new ListModel
            {
                Posts = posts.Select(p => new ListModel.PreviewModel
                {
                    Description = p.Description,
                    Id = p.Id,
                    Title = p.Title,
                    PrimaryImageUrl = p.ImageUrls.First()
                })
            });
        }

        [HttpGet("Details/{id}")]
        public ActionResult Details(int id)
        {
            var model = PostsRepository.GetById(id, p => new DetailsModel
            {
                Author = new DetailsModel.AuthorModel
                {
                    Id = p.OwnerId,
                    DisplayName = p.Owner.DisplayName,
                    FollowerCount = p.Owner.Followments.Count
                },
                Description = p.Description,
                Title = p.Title,
                ImageUrls = p.ImageUrls
            });

            return Ok(model);
        }
    }
}