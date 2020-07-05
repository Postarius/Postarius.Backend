using System.Linq;
using Domain;

namespace Data
{
    public interface IPostsRepository : IEntityRepository<Post>
    {
        IQueryable<Post> GetPostsByUserId(int userId);
        IQueryable<Post> GetByStatus(PostStatus status);
    }
    
    public class PostsRepository : EntityRepositoryBase<Post>, IPostsRepository
    {
        public PostsRepository(PostariusContext dbContext) : base(dbContext)
        {
            
        }

        public IQueryable<Post> GetPostsByUserId(int userId)
        {
            return GetMany(p => p.OwnerId == userId);
        }

        public IQueryable<Post> GetByStatus(PostStatus status)
        {
            return GetMany(p => p.Status == status);
        }
    }
}