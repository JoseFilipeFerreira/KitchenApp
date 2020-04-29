using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using KitchenLib;
using KitchenLib.Database;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;

//TODO Implement a All like endpoint to the shared inventories
namespace ShoppinglistService.Controllers
{
    [ApiController]
    [Route("[action]")]
    public class ShoppinglistController : ControllerBase
    {
        [HttpGet("{uid}")]
        public async Task<Inventory<WantedProduct>> Info([FromHeader] string auth, [FromRoute] string uid)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return null;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            var u = await ShoppingListStore.Get(uid, user);
            if (u != null) return u;
            HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
            return null;
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

            var res = await ShoppingListStore.GetAll(user);
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

            var res = await ShoppingListStore.GetShared(user);
            return res;
        }
        
        [HttpPost]
        public async void AddProduct([FromHeader] string auth, [FromForm] string product, [FromForm] int quantity,
            [FromForm] string uid)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            if (!ProductStore.Exists(product).Result || !ShoppingListStore.Exists(uid, user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            await ShoppingListStore.Add_prod(user, uid, product, quantity);
        }

        [HttpPost]
        public async void EditProduct([FromHeader] string auth, [FromForm] string product,
            [FromForm] string uid, [FromForm] long quantity)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !await UserStore.Exists(user))
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            if (!ProductStore.Exists(product).Result || !ShoppingListStore.Exists(uid, user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            await ShoppingListStore.Restock(uid, product, quantity, user );
        }

        [HttpPost]
        public async void RemoveProduct([FromHeader] string auth, [FromForm] string uid, [FromForm] string prod)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !await UserStore.Exists(user))
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            if (!ProductStore.Exists(prod).Result || !ShoppingListStore.Exists(uid, user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            await ShoppingListStore.RemoveProd(uid, prod, user);
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
            if (await ShoppingListStore.ExistName(name, user))
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Conflict;
            }
            else
            {
                await ShoppingListStore.Add(new Inventory<WantedProduct>(name), user);
            }
        }

        [HttpDelete("{uid}")]
        public bool Remove([FromHeader] string auth, [FromRoute] string uid)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return false;
            }

            HttpContext.Response.Headers.Add("auth", auth);

            return ShoppingListStore.Remove(uid, user).Result;
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
            if (!ShoppingListStore.Exists(uid, user).Result || !UserStore.Exists(friend).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            await ShoppingListStore.Share(uid, user, friend);
        }

        [HttpPost]
        public async void Edit([FromHeader] string auth, [FromForm] string uid, [FromForm] string name)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            if (await ShoppingListStore.ExistName(name, user))
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Conflict;
                return;
            }

            await ShoppingListStore.EditName(user, uid, name);
        }
    }
}