using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AuthServer;
using KitchenLib;
using KitchenLib.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Neo4j.Driver;

namespace UserService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        //Todo Don't send passwd
        [HttpGet]
        public User Get()
        {
            string token, user;
            if (HttpContext.Request.Cookies.TryGetValue("token", out token) &&
                (user = JwtBuilder.UserJwtToken(token).Result) != null)
            {
                return new UserStore().Get(user).Result;
            }

            HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
            HttpContext.Response.Cookies.Delete("token");
            return null;
        }

        //TODO Get better way to update user
        [HttpPost]
        public async Task<User> Post()
        {
            string token, user;
            if (HttpContext.Request.Cookies.TryGetValue("token", out token) &&
                (user = JwtBuilder.UserJwtToken(token).Result) != null)
            {
                var us = new UserStore();
                var u = us.Get(user).Result;
                StringValues str;
                if (HttpContext.Request.Form.TryGetValue("birthdate", out str))
                {
                    u._birthdate = new LocalDateTime(Convert.ToDateTime(str.ToString()));
                }

                if (HttpContext.Request.Form.TryGetValue("name", out str))
                {
                    u._name = str.ToString();
                }

                await us.Add(u);

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