using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Common;
using Domain;

namespace Data
{
    public interface IUsersRepository : IEntityRepository<User>
    {
        IQueryable<User> Search(ListSearchParams searchParams);
        IEnumerable<User> GetFollowersByUserId(int userId);
        IEnumerable<User> GetFollowedByUserId(int userId);
        User GetByEmail(string email, bool throwIfNotFound = false);
        User GetByLogin(string login, bool throwIfNotFound = false);
    }
    
    public class UsersRepository : EntityRepositoryBase<User>, IUsersRepository
    {
        public UsersRepository(PostariusContext dbContext) : base(dbContext)
        {
            
        }

        public IQueryable<User> Search(ListSearchParams searchParams)
        {
            return GetMany(searchParams.BuildExpression()).OrderBy(u => u.CreatedAt).Skip(searchParams.Skip).Take(searchParams.Take);
        }

        public IEnumerable<User> GetFollowersByUserId(int userId)
        {
            return GetById(userId, u => u.Followments.Select(f => f.Follower));
        }

        public IEnumerable<User> GetFollowedByUserId(int userId)
        {
            return GetById(userId, u => u.Followings.Select(f => f.Followed));
        }

        public User GetByEmail(string email, bool throwIfNotFound = false)
        {
            return throwIfNotFound ? First(u => u.Email == email) : FirstOrDefault(u => u.Email == email);
        }

        public User GetByLogin(string login, bool throwIfNotFound = false)
        {
            return throwIfNotFound ? First(u => u.Login == login) : FirstOrDefault(u => u.Login == login);
        }
    }

    public class ListSearchParams
    {
        public string DisplayName { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }

        public int Skip { get; set; }
        public int Take { get; set; }
        
        public Expression<Func<User, bool>> BuildExpression()
        {
            Expression<Func<User, bool>> expression = u => true;

            if (!Email.IsEmpty())
                expression = expression.And(u => u.Email.Contains(Email));
            
            if (!Login.IsEmpty())
                expression = expression.And(u => u.Login.Contains(Login));
            
            if (!DisplayName.IsEmpty())
                expression = expression.And(u => u.DisplayName.Contains(DisplayName));

            return expression;
        }
    }
}