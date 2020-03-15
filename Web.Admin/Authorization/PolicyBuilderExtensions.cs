using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Web.Admin.Authorization
{
    public static class PolicyBuilderExtensions
    {
        public static AuthorizationPolicyBuilder RequireAdminRights(this AuthorizationPolicyBuilder self)
        {
            self.Requirements.Add(new ClaimsAuthorizationRequirement("isAdministrator", new []{true.ToString()}));
            return self;
        }
    }
}