using System;
using System.Net;
using System.Threading.Tasks;
using KitchenLib;
using KitchenLib.Database;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;

namespace UserService.Controllers
{
    [ApiController]
    [Route("[action]")]
    [EnableCors]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public User Info([FromHeader] string auth = null)
        {
            string user;
            if (auth != null &&
                (user = JwtBuilder.UserJwtToken(auth).Result) != null)
            {
                var u =  UserStore.Get(user).Result;
                if (u == null)
                {
                    HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                    HttpContext.Response.Headers.Remove("token");
                    return null;
                }
                u._passwd = null;
                HttpContext.Response.Headers.Add("auth", auth);
                return u;
            }

            HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
            HttpContext.Response.Headers.Remove("token");
            return null;
        }

        [HttpPost]
        public async Task<User> Edit([FromForm] DateTime birthday = default, [FromForm] string name = null, [FromHeader] string auth = null)
        {
            string user;
            if (auth != null &&
                (user = JwtBuilder.UserJwtToken(auth).Result) != null)
            {
                var u = UserStore.Get(user).Result;
                if (u == null)
                {
                    HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                    HttpContext.Response.Headers.Remove("token");
                    return null;
                }                
                if (birthday != default)
                {
                    u._birthdate = new LocalDateTime(birthday);
                }

                if (name != null)
                {
                    u._name = name;
                }

                await UserStore.Add(u);
                HttpContext.Response.Headers.Add("auth", auth);

                return u;
            }
            
            HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
            HttpContext.Response.Headers.Remove("token");
            return null;
        }

        [HttpDelete]
        public void Remove([FromHeader] string auth = null)
        {
            string user;
            if (auth != null &&
                (user = JwtBuilder.UserJwtToken(auth).Result) != null)
            {
                UserStore.Remove(user);
                HttpContext.Response.Headers.Remove("token");
                return;
            }
            HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
            HttpContext.Response.Headers.Remove("token");
        }
    }
}