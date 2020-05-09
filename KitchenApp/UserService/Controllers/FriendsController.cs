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
            if ((user = JwtBuilder.UserJwtToken(auth).Result) != null) return UserStore.GetFriends(user).Result;
            HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
            return null;

        }

        [HttpGet]
        public IDictionary<string, string> Pending([FromHeader] string auth)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) != null) return UserStore.GetPendingFriends(user).Result;
            HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
            return null;
        }

        [HttpPost]
        public void Add([FromHeader] string auth, [FromForm] string friend)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(friend).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            UserStore.AddFriend(user, friend);
        }

        [HttpDelete]
        public void Remove([FromHeader] string auth, [FromForm] string friend)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(friend).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            UserStore.FriendshipRuined(user, friend);
        }

        [HttpPost]
        public void Accept([FromHeader] string auth, [FromForm] string friend)
        {
             string user;
             if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(friend).Result)
             {
                 HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                 return;
             }
 
             UserStore.AcceptFriend(user, friend);           
        }
    }
}