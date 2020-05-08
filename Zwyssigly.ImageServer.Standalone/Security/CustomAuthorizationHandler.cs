using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using Zwyssigly.ImageServer.Security;

namespace Zwyssigly.ImageServer.Standalone.Security
{
    public class CustomAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly ISecurityConfigurationStorage _configurationStorage;

        public CustomAuthorizationHandler(ISecurityConfigurationStorage configurationStorage)
        {
            _configurationStorage = configurationStorage;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User.Claims.Any(c => c.Type == PermissionRequirement.ClaimType && Permission.FromString(c.Value).UnwrapOrThrow() == requirement.Permission))
                context.Succeed(requirement);

            context.Fail();

            return Task.FromResult(0);
        }
    }

    public class PermissionRequirement : IAuthorizationRequirement
    {
        public const string ClaimType = "permission";

        public Permission Permission { get; }

        public PermissionRequirement(Permission permission)
        {
            Permission = permission;
        }
    }
}
