using System;
using System.Linq;
using Data;
using Domain;
using Services.Utils;

namespace Services
{
    public interface IUserService
    {
        void Save(User user);
        void CreateSubscription(int initiatorId, int followedId);
    }
    
    public class UserService : IUserService
    {
        private IUsersRepository UsersRepository { get; }
        private IPasswordHashingService PasswordHashingService { get; }
        private ISubscriptionRepository SubscriptionRepository { get; }

        public UserService(IUsersRepository usersRepository,
            IPasswordHashingService passwordHashingService,
            ISubscriptionRepository subscriptionRepository)
        {
            UsersRepository = usersRepository;
            PasswordHashingService = passwordHashingService;
            SubscriptionRepository = subscriptionRepository;
        }


        public void Save(User user)
        {
            user.Password = PasswordHashingService.Hash(user.Password);

            UsersRepository.Save(user);
        }

        public void CreateSubscription(int initiatorId, int followedId)
        {
            var initiatorFollowings = UsersRepository.GetFollowedByUserId(initiatorId);
            if (initiatorFollowings.Select(f => f.Id).Contains(followedId))
                throw new Exception("Already friends.");
            
            var subscription = new Subscription
            {
                FollowedId = followedId,
                FollowerId = initiatorId
            };
            
            SubscriptionRepository.Save(subscription);
        }
    }
}