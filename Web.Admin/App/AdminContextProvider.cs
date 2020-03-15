using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Web.Common.App;

namespace Web.Admin.App
{
    public class AdminContextProvider : IContextProvider
    {
        private AppSettings AppSettings { get; set; }
        public ContextType Context => ContextType.Admin;
        public IReadOnlyCollection<string> IssuerNames => AppSettings.IssuerNames.ToArray();
        
        public AdminContextProvider(IOptions<AppSettings> appSettings)
        {
            AppSettings = appSettings.Value;
        }
    }
}