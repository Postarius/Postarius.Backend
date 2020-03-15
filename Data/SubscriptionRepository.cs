using System.Linq;
using Domain;

namespace Data
{
    public interface ISubscriptionRepository : IEntityRepository<Subscription>
    {
        
    }
    public class SubscriptionRepository : EntityRepositoryBase<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(PostariusContext dbContext) : base(dbContext)
        {
        }
    }
}