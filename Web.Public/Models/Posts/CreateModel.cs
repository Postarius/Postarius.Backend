using System.Collections.Generic;

namespace Web.Public.Models.Posts
{
    public class CreateModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> ImageUrls { get; set; }
    }
}