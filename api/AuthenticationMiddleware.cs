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

namespace api
{
    public static class AuthenticationExtension
    {
        public static IServiceCollection AddTokenAuthentication(this IServiceCollection services, IConfiguration config)
        {
            IRepository<User> repository = (IRepository<User>)services.BuildServiceProvider().GetService(typeof(IRepository<User>));
            IHttpContextAccessor contextAccessor = (IHttpContextAccessor)services.BuildServiceProvider().GetService(typeof(IHttpContextAccessor));


            var secret = config.GetSection("JwtConfig").GetSection("secret").Value;
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
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // ValidIssuer = "localhost",
                    RequireSignedTokens = true,
                    // ValidAudience = "localhost"// set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                };
                // Remove the default validator
                x.SecurityTokenValidators.Clear();
                x.SecurityTokenValidators.Add(new DynamicKeyJwtValidationHandler(repository, config, contextAccessor));
            });

            return services;
        }
    }
}
