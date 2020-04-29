using System;
using System.Net;
using System.Threading.Tasks;
using KitchenLib;
using KitchenLib.Database;
using Microsoft.AspNetCore.Cors;
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
                HttpContext.Response.Headers.Add("auth", token);
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Accepted;
            }
            else
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                HttpContext.Response.Headers.Remove("auth");
            }
        }

        [HttpGet]
        public async void Auth([FromHeader] string auth = null)
        {
            if (auth == null)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return;
            }
            auth = await JwtBuilder.ValidateJwtAsync(auth);
            if (auth == null)
            {
                HttpContext.Response.Headers.Remove("auth");
                
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return;
            }
            HttpContext.Response.Headers.Add("auth", auth);
            HttpContext.Response.StatusCode = (int) HttpStatusCode.Accepted;
        }
        
        

        [HttpPost]
        public async Task<User> Creds([FromForm] string email = null, [FromForm] string passwd = null, [FromHeader] string token = null)
        {
            string user;
            if (token != null &&
                (user = JwtBuilder.UserJwtToken(token).Result) != null)
            {
                var u = UserStore.Get(user).Result;
                if (u == null)
                {
                    HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                    HttpContext.Response.Headers.Remove("auth");
                    return null;
                }
                if (email != null)
                {
                    if (UserStore.Exists(email).Result)
                    {
                        HttpContext.Response.StatusCode = (int) HttpStatusCode.Conflict;
                        return null;
                    }

                    u._email = email;
                    token = await JwtBuilder.CreateJWTAsync(u, "KitchenAuth", "KicthenAuth", 1);
                }
            
                if (passwd != null)
                {
                    u._passwd = passwd;
                }
            
                await UserStore.Add(u);
                
                HttpContext.Response.Headers.Add("auth", token);
                HttpContext.Response.StatusCode = (int) HttpStatusCode.OK;
                
                return u;
            }
                        
            HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
            HttpContext.Response.Headers.Remove("auth");
            return null;
        }

        [HttpPost]
        public async Task<User> SignUp([FromForm] string name, [FromForm] string passwd, [FromForm] string email,
            [FromForm] long phone_number, [FromForm] DateTime birthdate)
        {
            if (await UserStore.Exists(email))
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Conflict;
                return null;
            }
            var user = new User(name, email, passwd, new LocalDateTime(birthdate), phone_number);
            await UserStore.Add(user);
            user._passwd = null;
            return user;
        }
    }
}