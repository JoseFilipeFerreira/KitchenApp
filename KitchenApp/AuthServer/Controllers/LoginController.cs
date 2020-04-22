using System;
using System.Net;
using System.Threading.Tasks;
using KitchenLib;
using KitchenLib.Database;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;

namespace AuthServer.Controllers
{
    [ApiController]
    [Route("[action]")]
    [EnableCors]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public async void Login([FromForm] string username, [FromForm] string passwd)
        {
            var u = UserStore.Get(username).Result;
            if (u != null && u.CheckPasswd(passwd))
            {
                var token = await JwtBuilder.CreateJWTAsync(u, "KitchenAuth", "KicthenAuth", 1);
                var cookieOpts = new CookieOptions {Expires = DateTimeOffset.Now.AddHours(1)};
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
        public async Task<User> Creds([FromForm] string email = null, [FromForm] string passwd = null)
        {
            string token, user;
            if (HttpContext.Request.Cookies.TryGetValue("token", out token) &&
                (user = JwtBuilder.UserJwtToken(token).Result) != null)
            {
                var u = UserStore.Get(user).Result;
                if (email != null)
                {
                    if (UserStore.Exists(email).Result)
                    {
                        HttpContext.Response.StatusCode = (int) HttpStatusCode.Conflict;
                        return null;
                    }

                    u._email = email;
                    token = await JwtBuilder.CreateJWTAsync(u, "KitchenAuth", "KicthenAuth", 1);
                    var cookieOpts = new CookieOptions {Secure = true, Expires = DateTimeOffset.Now.AddHours(1)};
                    HttpContext.Response.Cookies.Append("token", token, cookieOpts);
                }
            
                if (passwd != null)
                {
                    u._passwd = passwd;
                }
            
                await UserStore.Add(u);
                HttpContext.Response.StatusCode = (int) HttpStatusCode.OK;
                
                return u;
            }
                        
            HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
            HttpContext.Response.Cookies.Delete("token");
            return null;
        }

        [HttpPost]
        public async Task<User> SignUp([FromForm] string name, [FromForm] string passwd, [FromForm] string email,
            [FromForm] uint phone_number, [FromForm] DateTime birthdate)
        {
            if (await UserStore.Exists(email))
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Conflict;
                return null;
            }
            var user = new User(name, email, passwd, new LocalDateTime(birthdate), phone_number);
            await UserStore.Add(user);
            return user;
        }
    }
}