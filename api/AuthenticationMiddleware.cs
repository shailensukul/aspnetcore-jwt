using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using api.core;
using api.Data;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography.X509Certificates;
using api.data.Models;
using System.Security.Claims;

namespace api
{
    public static class AuthenticationExtension
    {
        public static IServiceCollection AddTokenAuthentication(this IServiceCollection services, IConfiguration config)
        {
            IRepository<User> repository = (IRepository<User>)services.BuildServiceProvider().GetService(typeof(IRepository<User>));
            IHttpContextAccessor contextAccessor = (IHttpContextAccessor)services.BuildServiceProvider().GetService(typeof(IHttpContextAccessor));
            //contextAccessor?.HttpContext?.Request.Headers.TryGetValue("UserID", out var userId);
            //var user = repository.Find(userId).ConfigureAwait(false).GetAwaiter().GetResult();

            var secret = config.GetSection("JwtConfig").GetSection("secret").Value;
            //var key = Encoding.ASCII.GetBytes(secret).Concat(user?.Salt ?? new byte[] { }).ToArray();
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;

                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    //IssuerSigningKey = new SymmetricSecurityKey(key),
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // ValidIssuer = "localhost",
                    RequireSignedTokens = true,
                    //TokenDecryptionKey = new X509SecurityKey(new X509Certificate2("key_private.pfx", "idsrv3test")),
                    // ValidAudience = "localhost"// set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                };
                // Remove the default validator
                x.SecurityTokenValidators.Clear();
                x.SecurityTokenValidators.Add(new DynamicKeyJwtValidationHandler(repository, config, contextAccessor));
                //Override the JWT Events
                x.Events = new JwtBearerEvents()
                {
                    OnTokenValidated = ctx =>
                    {
                        // Once the token has been validated, we get a list of the user roles from the database and convert them into Role claims
                        if (ctx.Request.Cookies.TryGetValue("user_id", out var userid))
                        {
                            var user = repository.Find(userid).ConfigureAwait(false).GetAwaiter().GetResult();
                            var rolesClaims = user.Roles.Select(r => new Claim(ClaimTypes.Role, r));
                            ctx.Principal.AddIdentity(new ClaimsIdentity(rolesClaims));
                        }
                        return Task.FromResult(0);
                    }
                };
            });

            return services;
        }
    }
}
