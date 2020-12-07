using api.core;
using api.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using api.core.Security;
using Microsoft.AspNetCore.Authorization;
using api.core.Models;
using api.data.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using api.data.Commands;
using System.ComponentModel.DataAnnotations;

namespace api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IConfiguration config;
        private IRepository<User> userRepo;

        public AccountController(IConfiguration config, IRepository<User> repository)
        {
            this.config = config;
            this.userRepo = repository;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            try
            {
                user.Salt = user.Password.GetSalt(32);
                user.Roles = new List<string> { RoleDefinitions.Customer }.ToArray();
                this.userRepo.Insert(user);
                return new ContentResult() { StatusCode = StatusCodes.Status200OK };
            }
            catch (Exception ex)
            {
                return new ContentResult() { StatusCode = StatusCodes.Status500InternalServerError, Content = ex.ToString() };
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserCredentials credentials)
        {
            try
            {
                var expiry = DateTime.UtcNow.AddMinutes(double.Parse(config.GetSection("JwtConfig").GetSection("expirationInMinutes").Value));
                var user = await this.userRepo.Find(credentials.UserId);
                if (user == null || user?.Password != credentials.Password)
                {
                    return new ContentResult() { Content = "", StatusCode = StatusCodes.Status401Unauthorized };
                }

                var jwt = new JwtService(config);
                var token = jwt.GenerateSecurityToken(credentials.UserId, user.Salt, user.Roles);
                this.setCookie("access_token", token, expiry);
                this.setCookie("user_id", credentials.UserId, expiry);

                return new ContentResult() { Content = token, StatusCode = StatusCodes.Status200OK };
            }
            catch (Exception ex)
            {
                return new ContentResult() { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [Authorize]
        [HttpPost("refreshtoken")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                if (Request.Headers.TryGetValue("UserID", out var userid))
                {
                    var user = await this.userRepo.Find(userid);
                    if (user == null)
                    {
                        return new ContentResult() { Content = "", StatusCode = StatusCodes.Status401Unauthorized };
                    }

                    var jwt = new JwtService(config);
                    var token = jwt.GenerateSecurityToken(userid, user.Salt, user.Roles);
                    var expiry = DateTime.UtcNow.AddMinutes(double.Parse(config.GetSection("JwtConfig").GetSection("expirationInMinutes").Value));
                    this.setCookie("access_token", token, expiry);
                    this.setCookie("user_id", userid, expiry);

                    return new ContentResult() { Content = token, StatusCode = StatusCodes.Status200OK };
                }
                return new ContentResult() { StatusCode = StatusCodes.Status401Unauthorized };
            }
            catch (Exception ex)
            {
                return new ContentResult() { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [Authorize(Roles = RoleDefinitions.Administrator)]
        [HttpPost("addroles")]
        public async Task<IActionResult> AddRoles(AddRolesCommand command)
        {
            try
            {
                if (command.Roles != null)
                {
                    // validate
                    foreach (var role in command.Roles)
                    {
                        if (!RoleDefinitions.All.Contains(role))
                        {
                            throw new ValidationException($"Role {role} does not exist!");
                        }
                    }

                    bool roleChanged = false;
                    var user = await this.userRepo.Find(command.UserId);
                    List<string> userRoles = new List<string>(user.Roles);

                    foreach (var role in command.Roles)
                    {
                        if (!userRoles.Contains(role))
                        {
                            roleChanged = true;
                            userRoles.Add(role);
                        }
                    }

                    if (roleChanged)
                    {
                        user.Roles = userRoles.ToArray();
                        await this.userRepo.Update(user);
                    }
                }
                return new ContentResult() { StatusCode = StatusCodes.Status200OK };
            }
            catch (ValidationException vEx)
            {
                return new ContentResult() { StatusCode = StatusCodes.Status422UnprocessableEntity, Content = vEx.Message };
            }
            catch (Exception ex)
            {
                return new ContentResult() { StatusCode = StatusCodes.Status400BadRequest, Content = ex.ToString() };
            }
        }

        [Authorize(Roles = RoleDefinitions.Administrator)]
        [HttpPost("removeroles")]
        public async Task<IActionResult> RemoveRoles(RemoveRolesCommand command)
        {
            try
            {
                if (command.Roles != null)
                {
                    // validate
                    foreach (var role in command.Roles)
                    {
                        if (!RoleDefinitions.All.Contains(role))
                        {
                            throw new ValidationException($"Role {role} does not exist!");
                        }
                    }

                    bool roleChanged = false;
                    var user = await this.userRepo.Find(command.UserId);
                    List<string> userRoles = new List<string>(user.Roles);

                    foreach (var role in command.Roles)
                    {
                        if (userRoles.Contains(role))
                        {
                            roleChanged = true;
                            userRoles.Remove(role);
                        }
                    }

                    if (roleChanged)
                    {
                        user.Roles = userRoles.ToArray();
                        await this.userRepo.Update(user);
                    }
                }
                return new ContentResult() { StatusCode = StatusCodes.Status200OK };
            }
            catch (ValidationException vEx)
            {
                return new ContentResult() { StatusCode = StatusCodes.Status422UnprocessableEntity, Content = vEx.Message };
            }
            catch (Exception ex)
            {
                return new ContentResult() { StatusCode = StatusCodes.Status400BadRequest, Content = ex.ToString() };
            }
        }

        [HttpGet("test")]
        [AuthorizeRoles(RoleDefinitions.Administrator, RoleDefinitions.Customer)]
        public IActionResult Test()
        {
            return new ContentResult() { StatusCode = StatusCodes.Status200OK };
        }

        [Authorize(Roles = RoleDefinitions.Administrator)]
        [HttpPost("changeUserSalt")]
        public async Task<IActionResult> ChangeUserSalt(UserCredentials credentials)
        {
            try
            {
                var user = await this.userRepo.Find(credentials.UserId);
                user.Salt = user.Password.GetSalt(32);
                await this.userRepo.Update(user);

                return new ContentResult() { StatusCode = StatusCodes.Status200OK };
            }
            catch (Exception ex)
            {
                return new ContentResult() { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        private void setCookie(string name, string value, DateTimeOffset expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires,
                Secure = true
            };
            Response.Cookies.Append(name, value, cookieOptions);
        }

        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
