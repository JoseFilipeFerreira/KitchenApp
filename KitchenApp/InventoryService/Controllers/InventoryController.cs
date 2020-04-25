using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using KitchenLib;
using KitchenLib.Database;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;

namespace InventoryService.Controllers
{
    [ApiController]
    [Route("[action]")]
    public class InventoryController : ControllerBase
    {
        [HttpGet("{uid}")]
        public async Task<Inventory<OwnedProduct>> Info([FromHeader] string auth, [FromRoute] string uid)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) != null)
            {
                var u = await InventoryStore.Get(uid, user);
                if (u == null)
                {
                    HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                    return null;
                }

                HttpContext.Response.Headers.Add("auth", auth);
                return u;
            }

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

            var res = await InventoryStore.GetAll(user);
            return res;
        }

        [HttpPost]
        public async void AddProduct([FromHeader] string auth, [FromForm] string product, [FromForm] int quantity,
            [FromForm] string uid, [FromForm] DateTime expire)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) != null && UserStore.Exists(user).Result)
            {
                if (!ProductStore.Exists(product).Result || !InventoryStore.Exists(uid, user).Result)
                {
                    HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                    HttpContext.Response.Headers.Add("auth", auth);
                    return;
                }

                HttpContext.Response.Headers.Add("auth", auth);
                await InventoryStore.Add_prod(uid, product, quantity, expire, user);
                return;
            }

            HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
        }

        [HttpPost]
        public async void EditProduct([FromHeader] string auth, [FromForm] string product,
            [FromForm] string uid, [FromForm] DateTime? expire = null, [FromForm] long? quantity = null)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) != null && await UserStore.Exists(user))
            {
                if (!ProductStore.Exists(product).Result || !InventoryStore.Exists(uid, user).Result)
                {
                    HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                    HttpContext.Request.Headers.Add("auth", auth);
                    return;
                }

                var date = expire != null ? new LocalDateTime((DateTime) expire) : null;
                await InventoryStore.Restock(uid, product, user, quantity, date);
                HttpContext.Response.Headers.Add("auth", auth);
                return;
            }

            HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
        }

        [HttpPost]
        public async void RemoveProduct([FromHeader] string auth, [FromForm] string inv, [FromForm] string prod)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) != null && await UserStore.Exists(user))
            {
                if (!ProductStore.Exists(prod).Result || !InventoryStore.Exists(inv, user).Result)
                {
                    HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                    HttpContext.Request.Headers.Add("auth", auth);
                    return;
                }

                HttpContext.Response.Headers.Add("auth", auth);
                await InventoryStore.RemoveProd(inv, prod, user);
                return;
            }

            HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
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
            if (await InventoryStore.ExistName(name, user))
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Conflict;
            }
            else
            {
                var inv = new Inventory<OwnedProduct>(name);
                await InventoryStore.Add(inv, user);
            }
        }

        [HttpPost]
        public bool Remove([FromHeader] string auth, [FromForm] string uid)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return false;
            }

            HttpContext.Response.Headers.Add("auth", auth);

            return InventoryStore.Remove(uid, user).Result;
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

            if (!InventoryStore.Exists(uid, user).Result || !UserStore.Exists(friend).Result)
            {
                Console.WriteLine(UserStore.Exists(friend).Result);
                HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            HttpContext.Response.Headers.Add("auth", auth);

            await InventoryStore.Share(uid, user, friend);
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
            if (await InventoryStore.ExistName(name, user))
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Conflict;
            }
            else
            {
                await InventoryStore.EditName(user, uid, name);
            }
        }
    }
}