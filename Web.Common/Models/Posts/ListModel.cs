using System.Collections.Generic;

namespace Web.Common.Models.Posts
{
    public class ListModel
    {
        public IEnumerable<PostModel> Posts { get; set; }
        
        public class PostModel
        {
            public string PrimaryImageUrl { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public int Id { get; set; }
        }
    }
}