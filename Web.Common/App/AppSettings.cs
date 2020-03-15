using System.Collections.Generic;

namespace Web.Common.App
{
    public class AppSettings
    {
        public string SigninSecretKey { get; set; }
        public IEnumerable<string> FrontendUrls { get; set; }
        public IEnumerable<string> IssuerNames { get; set; }
    }
}