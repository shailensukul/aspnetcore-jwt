using api.core;
using api.data.Models;
using api.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace api
{
    /// <summary>
    /// We use a combination of the secret and a user specific salt for signing the JWT token
    // This allows us to invalidate the user token when the password changes
    // or just change the salt and kill active user sessions
    /// </summary>
    public class DynamicKeyJwtValidationHandler : JwtSecurityTokenHandler, ISecurityTokenValidator
    {
        private IRepository<User> repo;
        private IConfiguration config;
        private readonly IHttpContextAccessor httpContextAccessor;

        public DynamicKeyJwtValidationHandler(IRepository<User> repository, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            this.repo = repository;
            this.config = config;
            this.httpContextAccessor = httpContextAccessor;
        }

        public SecurityKey GetKeyForClaimedId(string claimedId)
        {
            var secret = config.GetSection("JwtConfig").GetSection("secret").Value;
            var user = this.repo.Find(claimedId).ConfigureAwait(false).GetAwaiter().GetResult();
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret).Concat(user?.Salt ?? new byte[] { }).ToArray());
        }

        // [DebuggerStepThrough]
        public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            if (validationParameters == null)
                throw new ArgumentNullException(nameof(validationParameters));

            if (token.Length > MaximumTokenSizeInBytes)
                throw new ArgumentException(
                    $"IDX10209: token has length: '{token.Length}' which is larger than the MaximumTokenSizeInBytes: '{MaximumTokenSizeInBytes}'.");


            byte[] ecKey = new byte[256 / 8];
            Array.Copy(Encoding.ASCII.GetBytes(config.GetSection("JwtConfig").GetSection("encrypt").Value), ecKey, 256 / 8);
            //We can read the token before we've begun validating it.
            JwtSecurityToken incomingToken = ReadJwtToken(token);
            //Extract the external system ID from the token.
            //string userid = incomingToken
            //    .Claims
            //    .First(claim => claim.Type == "email")
            //    .Value;

            // if (this.httpContextAccessor.HttpContext.Request.Headers.TryGetValue("UserID", out var userid))
           if (this.httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("user_id", out var userid))
            {
                //Retrieve the corresponding Public Key from our data store
                SecurityKey publicKeyForExternalSystem = GetKeyForClaimedId(userid);

                //Set our parameters to use the public key we've looked up
                validationParameters.IssuerSigningKey = publicKeyForExternalSystem;
                validationParameters.TokenDecryptionKey = new SymmetricSecurityKey(ecKey);

                // And let the framework take it from here.
                return base.ValidateToken(token, validationParameters, out validatedToken);
            }

            throw new SecurityTokenDecryptionFailedException();
        }
    }
}
