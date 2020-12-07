using System;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using api.core;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace api
{
    public class JwtService : IJwtService
    {
        private readonly string _secret;
        private readonly string _ecKey;
        private readonly string _expDate;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtService(IConfiguration config)
        {
            _ecKey = config.GetSection("JwtConfig").GetSection("encrypt").Value;
            _secret = config.GetSection("JwtConfig").GetSection("secret").Value;
            _expDate = config.GetSection("JwtConfig").GetSection("expirationInMinutes").Value;
            _issuer = config.GetSection("JwtConfig").GetSection("issuer").Value;
            _audience = config.GetSection("JwtConfig").GetSection("audience").Value;

        }

        public string GenerateSecurityToken(string email, byte[] salt, string[] roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_secret).Concat(salt).ToArray();
            byte[] ecKey = new byte[256 / 8];
            Array.Copy(Encoding.ASCII.GetBytes(_ecKey), ecKey, 256 / 8);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _issuer,
                Audience = _audience,
                Subject = new ClaimsIdentity(
                    new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, email)
                    }
                    .Concat(roles.Select(r => new Claim(ClaimTypes.Role, r))).ToArray()),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_expDate)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                EncryptingCredentials = new EncryptingCredentials(
        new SymmetricSecurityKey(
            ecKey),
            SecurityAlgorithms.Aes256KW,
            SecurityAlgorithms.Aes256CbcHmacSha512)
            };

            //var token = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }

        public bool ValidateToken(string token, byte[] salt)
        {
            var key = Encoding.ASCII.GetBytes(_secret).Concat(salt).ToArray();
            var validationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken = null;
            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            }
            catch (SecurityTokenException)
            {
                return false;
            }
            catch (Exception e)
            {
                Console.Write(e.ToString()); //something else happened
                throw;
            }
            //... manual validations return false if anything untoward is discovered
            return validatedToken != null;
        }
    }
}