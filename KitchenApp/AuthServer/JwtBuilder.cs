using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using KitchenLib;
using Microsoft.IdentityModel.Tokens;

namespace AuthServer
{
    public class JwtBuilder
    {
        private static string symSec =
            "";
        public static async Task<string> CreateJWTAsync(
            User user,
            string issuer,
            string authority,
            int hoursValid)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = await CreateClaimsIdentitiesAsync(user);

            // Create JWToken
            var token = tokenHandler.CreateJwtSecurityToken(issuer: issuer,
                audience: authority,
                subject: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(hoursValid),
                signingCredentials:
                    new SigningCredentials(
                        new SymmetricSecurityKey(
                            Encoding.Default.GetBytes(symSec)), 
                        SecurityAlgorithms.HmacSha256Signature));
            
            return tokenHandler.WriteToken(token);
        }

        public static async Task<string> ValidateJwtAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var validate = new TokenValidationParameters()
                {ValidateAudience = false, ValidateLifetime = true, ValidateIssuer = true, ValidIssuer = "KitchenAuth", IssuerSigningKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(symSec))};
            try
            {
                tokenHandler.ValidateToken(token, validate, out _);
            }
            catch
            {
                return null;
            }
            return tokenHandler.WriteToken(tokenHandler.CreateJwtSecurityToken(issuer: jwtToken.Issuer,
                audience: jwtToken.Audiences.GetEnumerator().Current, subject: new ClaimsIdentity(jwtToken.Claims), notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(1), signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.Default.GetBytes(symSec)),SecurityAlgorithms.HmacSha256Signature)));
        }
        
        //TODO Move this function
        public static async Task<string> UserJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var validate = new TokenValidationParameters()
                {ValidateAudience = false, ValidateLifetime = true, ValidateIssuer = true, ValidIssuer = "KitchenAuth", IssuerSigningKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(symSec))};
            try
            {
                tokenHandler.ValidateToken(token, validate, out _);
            }
            catch
            {
                return null;
            }

            return jwtToken.Actor;
        }
        
        public static Task<ClaimsIdentity> CreateClaimsIdentitiesAsync(User user)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Actor, user._email));
        //    var roles = Enumerable.Empty<Role>(); // Not a real list.
        //   foreach (var role in roles)
        //    { claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.RoleName)); }

            return Task.FromResult(claimsIdentity);
        }
    }
}