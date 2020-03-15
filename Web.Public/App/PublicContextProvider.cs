using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Web.Common.App;

namespace Web.Public.App
{
    public class PublicContextProvider : IContextProvider
    {
        private AppSettings AppSettings { get; set; }
        public ContextType Context => ContextType.Public;
        public IReadOnlyCollection<string> IssuerNames => AppSettings.IssuerNames.ToArray();
        
        public PublicContextProvider(IOptions<AppSettings> appSettings)
        {
            AppSettings = appSettings.Value;
        }
    }
}