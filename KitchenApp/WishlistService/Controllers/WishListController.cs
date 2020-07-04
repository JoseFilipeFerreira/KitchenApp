using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KitchenLib;
using KitchenLib.Database;

namespace WishlistService.Controllers
{
    [ApiController]
    [Route("[action]")]
    public class WishListController : ControllerBase
    {
        [HttpGet("{uid}")]
        public Inventory<Product> Info([FromRoute] string uid, [FromHeader] string auth)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return null;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            var wishL = WishlistStore.Get(uid, user).Result;
            if (wishL != null) return wishL;

            HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
            return null; //return new Inventory<Product>();
        }

        [HttpPost]
        public async void AddProduct([FromHeader] string auth, [FromForm] string uid, [FromForm] string product)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            var wishL = WishlistStore.Get(uid, user).Result;
            if (wishL == null)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            await WishlistStore.Add_prod(uid, product, user);
        }

        [HttpPost]
        public async void Add([FromHeader] string auth, [FromForm] string name)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            if (await WishlistStore.ExistName(name, user))
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Conflict;
            }
            else
            {
                await WishlistStore.Add(new Inventory<Product>(name), user);
            }
        }

        [HttpPost]
        public async void Edit([FromHeader] string auth, [FromForm] string name, [FromForm] string uid)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return;
            }
            
            HttpContext.Response.Headers.Add("auth", auth);
            if (await WishlistStore.ExistName(name, user))
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Conflict;
            }
            else
            {
                await WishlistStore.EditName(user, uid, name);
            }
        }

        [HttpPost("{uid}")]
        public bool Remove([FromHeader] string auth, [FromRoute] string uid)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return false;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            //removes wishlist with name 'wishlist' form the user based on it's email
            return WishlistStore.Remove(uid, user).Result;
        }

        [HttpPost]
        public async void RemoveProduct([FromHeader] string auth, [FromForm] string uid, [FromForm] string product)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            var wishL = WishlistStore.Get(uid, user).Result;
            if (wishL == null)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            await WishlistStore.RemoveProd(uid, product, user);
        }
        
        [HttpPost]
        public async void Share([FromHeader] string auth, [FromForm] string uid, [FromForm] string friend)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            if (!WishlistStore.Exists(uid, user).Result || !UserStore.Exists(friend).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            await WishlistStore.Share(uid, user, friend);
        }
        
        [HttpGet]
        public async Task<IDictionary<string, string>> All([FromHeader] string auth)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return null;
            }

            HttpContext.Response.Headers.Add("auth", auth);

            var res = await WishlistStore.GetAll(user);
            return res;
        }
        
        [HttpGet]
        public async Task<IDictionary<string, string>> Shared([FromHeader] string auth)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return null;
            }

            HttpContext.Response.Headers.Add("auth", auth);

            var res = await WishlistStore.GetShared(user);
            return res;
        }
    }
}