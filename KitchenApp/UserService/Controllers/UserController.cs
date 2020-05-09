using System;
using System.Collections.Generic;
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
    [EnableCors]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [Route("[action]")]
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
        [Route("[action]")]
        public async Task<User> Edit([FromHeader] string auth, [FromForm] DateTime birthday = default,
            [FromForm] string name = null, [FromForm] long? phone_number = null)
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

            if (phone_number != null)
            {
                u._phone_number = (long) phone_number;
            }

            await UserStore.Add(u);
            HttpContext.Response.Headers.Add("auth", auth);

            return u;
        }

        [HttpDelete]
        [Route("[action]")]
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