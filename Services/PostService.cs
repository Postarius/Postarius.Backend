using Data;
using Domain;

namespace Services
{
    public interface IPostService
    {
        void Save(Post post);
        void ChangeStatus(int id, PostStatus status);
    }
    
    public class PostService : IPostService
    {
        private IPostsRepository PostsRepository { get; }

        public PostService(IPostsRepository postsRepository)
        {
            PostsRepository = postsRepository;
        }
        
        public void Save(Post post)
        {
            PostsRepository.Save(post);
        }

        public void ChangeStatus(int id, PostStatus status)
        {
            var post = PostsRepository.GetById(id);
            post.Status = status;
            
            PostsRepository.Save(post);
        }
    }
}