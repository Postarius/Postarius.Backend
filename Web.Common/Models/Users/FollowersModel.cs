using System.Collections.Generic;

namespace Web.Common.Models.Users
{
    public class FollowersModel
    {
        public IEnumerable<UserInfo> Followers { get; set; }
    }
}