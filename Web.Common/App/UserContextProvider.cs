using System.Collections.Generic;
using System.Linq;
using Common;
using Data;
using Microsoft.AspNetCore.Http;

namespace Web.Common.App
{
    public interface IUserContextProvider
    {
        UserContext Get();
    }
    
    public class UserContextProvider : IUserContextProvider
    {
        private const string IdKey = "id";
        
        private readonly IReadOnlyDictionary<string, string> claims;
        
        private HttpContext Context { get; set; }
        private IUsersRepository UsersRepository { get; }
        private IContextProvider ContextProvider { get; }

        public UserContextProvider(IHttpContextAccessor httpContextAccessor,
            IUsersRepository usersRepository,
            IContextProvider contextProvider)
        {
            UsersRepository = usersRepository;
            ContextProvider = contextProvider;
            Context = httpContextAccessor.HttpContext;
            claims = Context.User.Claims.ToDictionary(c => c.Type, c => c.Value);
        }
        
        public UserContext Get()
        {
            var id = claims[IdKey].AsInt();
            return UsersRepository.GetById(id, u => new UserContext
            {
                Id = u.Id,
                Login = u.Login,
                IsBackend = ContextProvider.Context == ContextType.Admin
            });
        }
    }

    public class UserContext
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public bool IsBackend { get; set; }
    }
}