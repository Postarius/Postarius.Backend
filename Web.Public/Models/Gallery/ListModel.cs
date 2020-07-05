using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Web.Public.Models.Gallery
{
    public class ListModel
    {
        public IEnumerable<PreviewModel> Posts { get; set; }
        
        public class PreviewModel
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string PrimaryImageUrl { get; set; }
            public int Id { get; set; }
        }
    }
}