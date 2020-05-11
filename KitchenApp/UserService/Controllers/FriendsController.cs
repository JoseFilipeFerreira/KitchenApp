using System.Collections.Generic;
using System.Net;
using KitchenLib;
using KitchenLib.Database;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace UserService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [EnableCors]
    public class FriendsController : ControllerBase
    {
        [HttpGet]
        public IDictionary<string, string> Get([FromHeader] string auth)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return null;
            }

            HttpContext.Response.Headers.Add("auth", auth);

            return UserStore.GetFriends(user).Result;
        }

        [HttpGet]
        public IDictionary<string, string> Pending([FromHeader] string auth)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return null;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            return UserStore.GetPendingFriends(user).Result;
        }

        [HttpPost]
        public async void Add([FromHeader] string auth, [FromForm] string friend)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(friend).Result ||
                !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            HttpContext.Response.Headers.Add("auth", auth);

            await UserStore.AddFriend(user, friend);
        }

        [HttpDelete]
        public async void Remove([FromHeader] string auth, [FromForm] string friend)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(friend).Result ||
                !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            HttpContext.Response.Headers.Add("auth", auth);

            await UserStore.FriendshipRuined(user, friend);
        }

        [HttpPost]
        public async void Accept([FromHeader] string auth, [FromForm] string friend)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(friend).Result ||
                !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            HttpContext.Response.Headers.Add("auth", auth);

            await UserStore.AcceptFriend(user, friend);
        }
    }
}