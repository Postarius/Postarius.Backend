using System.Collections.Generic;
using Data;

namespace Web.Common.Models.Users
{
    public class ListModel
    {
        public IEnumerable<UserInfo> UserInfos { get; set; }
        public ListSearchParams SearchParams { get; set; }
        public int TotalPageCount { get; set; }
        public int TotalEntries { get; set; }
        public int PageNumber { get; set; }
    }
}