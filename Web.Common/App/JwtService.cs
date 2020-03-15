using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Common;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Web.Common.App
{
    public interface IJwtService
    {
        bool ValidateToken(string token, out UserData userData);
        string GenerateToken(IEnumerable<Claim> claims, DateTime? expires = null);
    }
    public class JwtService : IJwtService
    {
        private IContextProvider ContextProvider { get; }
        private AppSettings AppSettings { get; }

        public JwtService(IContextProvider contextProvider,
            IOptions<AppSettings> appSettings)
        {
            ContextProvider = contextProvider;
            AppSettings = appSettings.Value;
        }
        
        public bool ValidateToken(string token, out UserData userData)
        {
            userData = null;
            
            var validationParameters = new TokenValidationParameters
            {
                ValidateLifetime = false,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(AppSettings.SigninSecretKey.GetBytes())
            };

            if (ContextProvider.Context == ContextType.Admin)
            {
                validationParameters.ValidIssuers = ContextProvider.IssuerNames;
                validationParameters.ValidAudiences = AppSettings.FrontendUrls;
            }

            if (!ValidateToken(token, validationParameters, out var claims))
                return false;
            
            var tokenExpiresAtUtc = DateTime.Parse(claims.FindFirstValue("expiresAtUtc"), styles: DateTimeStyles.AdjustToUniversal);
            if (tokenExpiresAtUtc < DateTime.UtcNow)
                return false;

            userData = claims.FindFirstValue("data").FromJson<UserData>();

            return true;
        }

        public string GenerateToken(IEnumerable<Claim> claims, DateTime? expires = null)
        {
            if (claims == null)
                throw new ArgumentNullException(nameof(claims));
            
            var credentials = new SigningCredentials(new SymmetricSecurityKey(AppSettings.SigninSecretKey.GetBytes()), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials,
                claims: claims,
                notBefore: DateTime.UtcNow,
                audience: AppSettings.FrontendUrls.First(),
                issuer: ContextProvider.IssuerNames.First());

            try
            {
                new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        private bool ValidateToken(string token, TokenValidationParameters validationParameters, out ClaimsPrincipal claimsPrincipal)
        {
            claimsPrincipal = null;
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                claimsPrincipal = jwtSecurityTokenHandler.ValidateToken(token, validationParameters, out _);
                return true;
            }
            catch (SecurityTokenException)
            {
                return false;
            }
        }
    }

    public class UserData
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public bool IsAdminUser { get; set; }
    }
}