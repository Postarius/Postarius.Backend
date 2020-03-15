using Domain;

namespace Data
{
    public interface IMediaRepository : IEntityRepository<Media>
    {
        
    }
    
    public class MediaRepository : EntityRepositoryBase<Media>, IMediaRepository
    {
        public MediaRepository(PostariusContext dbContext) : base(dbContext)
        {
        }
    }
}