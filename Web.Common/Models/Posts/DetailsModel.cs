using System;
using System.Collections.Generic;

namespace Web.Common.Models.Posts
{
    public class DetailsModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> ImageUrls { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool Edited { get; set; }
        public DateTime? EditedAt { get; set; }
    }
}