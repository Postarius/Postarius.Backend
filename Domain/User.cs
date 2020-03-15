using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Domain
{
    public class User : EntityBase
    {
        public string Login { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string AvatarUrl { get; set; }

        public List<Post> Posts { get; set; }
        
        // Self followments to others
        public List<Subscription> Followments { get; set; }
        
        // Other followments to self
        public List<Subscription> Followings { get; set; }
    }
}