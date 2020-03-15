using System.Collections.Generic;

namespace Domain
{
    public class Post : EntityBase
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public PostStatus Status { get; set; }
        public bool Edited { get; set; }

        public int OwnerId { get; set; }
        public User Owner { get; set; }

        public List<string> ImageUrls { get; set; }
    }

    public enum PostStatus
    {
        // Before admin watches post
        InProgress = 0,
        Finalized = 1,
        
        // After admin makes decision
        Approved = 10,
        Disapproved = 11
    }
}