using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Zwyssigly.Functional;
using Zwyssigly.ImageServer.Security;

namespace Zwyssigly.ImageServer.Standalone.Security
{
    public class CustomAuthenticationOptions : AuthenticationSchemeOptions { }
    public class CustomAuthenticationHandler : AuthenticationHandler<CustomAuthenticationOptions>
    {
        private readonly ISecurityConfigurationStorage _storage;

        public CustomAuthenticationHandler(
            ISecurityConfigurationStorage storage,
            IOptionsMonitor<CustomAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _storage = storage;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var values))
                return GetResult(a => a.Type == AccountType.Anonymous);


            if (!AuthenticationHeaderValue.TryParse(values, out var header))
                return Task.FromResult(AuthenticateResult.Fail("Can not parse authorization header"));

            switch (header.Scheme)
            {
                case "Basic":
                    {
                        var userAndPass = Encoding.UTF8.GetString(Convert.FromBase64String(header.Parameter)).Split(":", 2);
                        if (userAndPass.Length != 2)
                            return Task.FromResult(AuthenticateResult.Fail("Can not parse authorization header"));
                        return GetResult(a => a.Type == AccountType.Basic && a.Name.ToString() == userAndPass[0] && a.Password.Match(v => v == userAndPass[1], () => false));
                    }
                default:
                    return Task.FromResult(AuthenticateResult.Fail($"Unknown authorization scheme '{header.Scheme}'"));
            }
        }

        private async Task<AuthenticateResult> GetResult(Func<AccountConfiguration, bool> predicate)
        {
            var accountsResult = await _storage.GetGlobal()
                .AndThenAsync(async global =>
                {
                    var accounts = new List<AccountConfiguration>();

                    accounts.AddRange(global.Accounts.Where(predicate));

                    if (Request.RouteValues.TryGetValue("galleryName", out var routeValue))
                    {
                        var localResult = await Name.FromString(routeValue.ToString())
                            .AndThenAsync(name => _storage.Get(name));

                        localResult.IfSuccess(config => accounts.AddRange(config.Accounts.Where(predicate)));
                    }

                    return Result.Success<List<AccountConfiguration>, Error>(accounts);
                });

            return accountsResult.Match(
                accounts =>
                {
                    if (accounts.Count == 0)
                        return AuthenticateResult.NoResult();

                    var claims = new List<Claim> { new Claim(ClaimTypes.Name, accounts.First().Name.ToString()) };
                    claims.AddRange(accounts.SelectMany(a => a.Permissions).Distinct().Select(p => new Claim(PermissionRequirement.ClaimType, p.ToString())));

                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);
                    return AuthenticateResult.Success(ticket);
                },
                error => AuthenticateResult.Fail(error.Message.UnwrapOr(() => error.Code.ToString())));
        }
    }
}
