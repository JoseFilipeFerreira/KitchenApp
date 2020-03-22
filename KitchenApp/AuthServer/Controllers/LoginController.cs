using System;
using System.Net;
using KitchenLib.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Controllers
{
    [ApiController]
    [Route("[action]")]
    public class LoginController : ControllerBase
    {
        [HttpGet]
        public async void Login()
        {
            var uid = HttpContext.Request.Form["username"].ToString();
            var passwd = HttpContext.Request.Form["passwd"].ToString();
            var u = new UserStore().Get(uid).Result;
            var token = await JwtBuilder.CreateJWTAsync(u, "KitchenAuth", "KicthenAuth", 1);
            var cookieOpts = new CookieOptions {Secure = true, Expires = DateTimeOffset.Now.AddHours(1)};
            if (u != null && u.CheckPasswd(passwd))
            {
                HttpContext.Response.Cookies.Append("token", token, cookieOpts);
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
                HttpContext.Response.StatusCode = (int) HttpStatusCode.NotAcceptable;
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
    }
}