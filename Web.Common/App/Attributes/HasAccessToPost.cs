using System;
using System.Threading.Tasks;
using Common;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.Common.App.Attributes
{
    public class HasAccessToPostFilter : IAsyncActionFilter
    {
        private IUserContextProvider UserContextProvider { get; }
        private IPostsRepository PostsRepository { get; }

        public HasAccessToPostFilter(IUserContextProvider userContextProvider,
            IPostsRepository postsRepository)
        {
            UserContextProvider = userContextProvider;
            PostsRepository = postsRepository;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.TryGetValue("id", out var id) &&
                !context.ActionArguments.TryGetValue("postId", out id))
            {
                throw new InvalidOperationException();
            }

            var postOwnerInfo = PostsRepository.GetById(id.AsInt(), p => new
            {
                Id = p.OwnerId
            }, false);

            if (postOwnerInfo is null)
            {
                context.Result = new NotFoundResult();
                return;
            }

            if (!UserContextProvider.Get().IsBackend || postOwnerInfo.Id != id.AsInt())
            {
                context.Result = new ForbidResult("You have no access to this post.");
                return;
            }

            await next();
        }
    }

    public class HasAccessToPostAttribute : Attribute, IFilterMetadata
    {
        
    }
}