using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Web.Admin.Models.Users
{
    public class CreateModel
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string AvatarUrl { get; set; }
    }
}