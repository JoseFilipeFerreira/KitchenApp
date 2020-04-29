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
        public User Info([FromHeader] string auth)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return null;
            }

            var u = UserStore.Get(user).Result;
            if (u == null)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return null;
            }

            u._passwd = "[REDACTED]";
            HttpContext.Response.Headers.Add("auth", auth);
            return u;
        }

        [HttpPost]
        public async Task<User> Edit([FromHeader] string auth, [FromForm] DateTime birthday = default,
            [FromForm] string name = null)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return null;
            }

            var u = UserStore.Get(user).Result;
            if (u == null)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
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

        [HttpDelete]
        public void Remove([FromHeader] string auth)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) != null)
            {
                UserStore.Remove(user);
                return;
            }

            HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
        }
    }
}