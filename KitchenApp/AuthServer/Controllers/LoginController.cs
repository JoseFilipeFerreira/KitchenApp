using System;
using System.Net;
using System.Threading.Tasks;
using KitchenLib;
using KitchenLib.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace AuthServer.Controllers
{
    [ApiController]
    [Route("[action]")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public async void Login()
        {
            var uid = HttpContext.Request.Form["username"].ToString();
            var passwd = HttpContext.Request.Form["passwd"].ToString();
            var u = new UserStore().Get(uid).Result;
            if (u != null && u.CheckPasswd(passwd))
            {
                var token = await JwtBuilder.CreateJWTAsync(u, "KitchenAuth", "KicthenAuth", 1);
                var cookieOpts = new CookieOptions {Secure = true, Expires = DateTimeOffset.Now.AddHours(1)};
                HttpContext.Response.Cookies.Append("token", token, cookieOpts);
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Accepted;
            }
            else
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
            }
        }

        [HttpGet]
        public async void Auth()
        {
            string token;
            if (!HttpContext.Request.Cookies.TryGetValue("token", out token))
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return;
            }
            token = await JwtBuilder.ValidateJwtAsync(token);
            if (token == null)
            {
                HttpContext.Response.Cookies.Delete("token");
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return;
            }
            var cookieOpts = new CookieOptions {Secure = true, Expires = DateTimeOffset.Now.AddHours(1)};
            HttpContext.Response.Cookies.Append("token", token, cookieOpts);
            HttpContext.Response.StatusCode = (int) HttpStatusCode.Accepted;
        }

        [HttpPost]
        public async Task<User> Creds()
        {
            string token, user;
            if (HttpContext.Request.Cookies.TryGetValue("token", out token) &&
                (user = JwtBuilder.UserJwtToken(token).Result) != null)
            {
                var us = new UserStore();
                var u = us.Get(user).Result;
                StringValues str;
                if (HttpContext.Request.Form.TryGetValue("email", out str))
                {
                    if (us.Exists(str).Result)
                    {
                        HttpContext.Response.StatusCode = (int) HttpStatusCode.Conflict;
                        return null;
                    }
                    u._email = str.ToString();
                    token = await JwtBuilder.CreateJWTAsync(u, "KitchenAuth", "KicthenAuth", 1);
                    var cookieOpts = new CookieOptions {Secure = true, Expires = DateTimeOffset.Now.AddHours(1)};
                    HttpContext.Response.Cookies.Append("token", token, cookieOpts);
                }
            
                if (HttpContext.Request.Form.TryGetValue("passwd", out str))
                {
                    u._passwd = str.ToString();
                }
            
                await us.Add(u);
                HttpContext.Response.StatusCode = (int) HttpStatusCode.OK;
                
                return u;
            }
                        
            HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
            HttpContext.Response.Cookies.Delete("token");
            return null;
        }
    }
}