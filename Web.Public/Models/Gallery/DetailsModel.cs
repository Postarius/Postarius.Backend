using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Web.Public.Models.Gallery
{
    public class DetailsModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> ImageUrls { get; set; }
        public AuthorModel Author { get; set; }
        
        public class AuthorModel
        {
            public int Id { get; set; }
            public string DisplayName { get; set; }
            public int FollowerCount { get; set; }
        }
    }
}