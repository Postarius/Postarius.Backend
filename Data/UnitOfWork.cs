namespace Data
{
    public interface IUnitOfWork
    {
        void Commit();
    }
    
    public class UnitOfWork : IUnitOfWork
    {
        private PostariusContext DbContext { get; }

        public UnitOfWork(PostariusContext dbContext)
        {
            DbContext = dbContext;
        }

        public void Commit()
        {
            DbContext.SaveChanges();
        }
    }
}