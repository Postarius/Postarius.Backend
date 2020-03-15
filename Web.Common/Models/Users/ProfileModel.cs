using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Domain;

namespace Web.Common.Models.Users
{
    public class ProfileModel
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }

        public IEnumerable<UserInfo> Followers { get; set; }
        public IEnumerable<UserInfo> Followed { get; set; }

        public static Expression<Func<User, ProfileModel>> GetProfileDataExpression =
            u => new ProfileModel
            {
                Id = u.Id,
                Login = u.Login,
                Email = u.Email,
                DisplayName = u.DisplayName,
                AvatarUrl = u.AvatarUrl,
                Followers = u.Followments.Select(f => new UserInfo
                {
                    DisplayName = f.Follower.DisplayName,
                    Id = f.Follower.Id,
                    Email = f.Follower.Email,
                    AvatarUrl = f.Follower.AvatarUrl
                }),
                Followed = u.Followings.Select(f => new UserInfo
                {
                    DisplayName = f.Followed.DisplayName,
                    Id = f.Followed.Id,
                    Email = f.Followed.Email,
                    AvatarUrl = f.Followed.AvatarUrl
                })
            };
    }
}