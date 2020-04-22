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
        //Todo Don't send passwd
        [HttpGet]
        public User Info()
        {
            string token, user;
            if (HttpContext.Request.Cookies.TryGetValue("token", out token) &&
                (user = JwtBuilder.UserJwtToken(token).Result) != null)
            {
                var u =  UserStore.Get(user).Result;
                u._passwd = null;
                return u;
            }

            HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
            HttpContext.Response.Cookies.Delete("token");
            return null;
        }

        [HttpPost]
        public async Task<User> Edit([FromForm] DateTime birthday = default, [FromForm] string name = null)
        {
            string token, user;
            if (HttpContext.Request.Cookies.TryGetValue("token", out token) &&
                (user = JwtBuilder.UserJwtToken(token).Result) != null)
            {
                var us = new UserStore();
                var u = UserStore.Get(user).Result;
                if (birthday != default)
                {
                    u._birthdate = new LocalDateTime(birthday);
                }

                if (name != null)
                {
                    u._name = name;
                }

                await UserStore.Add(u);

                return u;
            }
            
            HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
            HttpContext.Response.Cookies.Delete("token");
            return null;
        }

        [HttpDelete]
        public void Remove()
        {
            string token, user;
            if (HttpContext.Request.Cookies.TryGetValue("token", out token) &&
                (user = JwtBuilder.UserJwtToken(token).Result) != null)
            {
                new UserStore().Remove(user);
                HttpContext.Response.Cookies.Delete("token");
                return;
            }
            HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
            HttpContext.Response.Cookies.Delete("token");
        }
    }
}