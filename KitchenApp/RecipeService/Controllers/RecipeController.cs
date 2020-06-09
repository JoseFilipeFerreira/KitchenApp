using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using KitchenLib;
using KitchenLib.Database;
using Microsoft.AspNetCore.Mvc;

namespace RecipeService.Controllers
{
    [ApiController]
    [Route("[action]")]
    public class WeatherForecastController : ControllerBase
    {
        [HttpPost]
        public async Task<List<Recipe>> Search([FromHeader] string auth, [FromForm] string keys)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return null;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            var list = RecipeSearch.SearchRecipe(10, keys);
            foreach (var v in list)
            {
                await RecipeStore.Add(new MinimalRecipe(v));
            }

            return list;
        }
        
        [HttpPost]
        public async Task<List<Recipe>> Search([FromHeader] string auth, [FromForm] string keys, [FromForm] string inventory)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return null;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            var inv = InventoryStore.Get(inventory, user).Result;
            if (inv == null)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return null;
            }
            var r = new List<Product>(inv._products);
            var list = RecipeSearch.SearchRecipe(10, keys, r);
            foreach (var v in list)
            {
                await RecipeStore.Add(new MinimalRecipe(v));
            }

            return list;
        }

        [HttpPost]
        public async void Star([FromHeader] string auth, [FromForm] long id)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            await RecipeStore.Star(id, user);
        }
        
        [HttpPost]
        public async void Unstar([FromHeader] string auth, [FromForm] long id)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            await RecipeStore.Unstar(id, user);
        }

        [HttpGet]
        public async Task<List<MinimalRecipe>> Stared([FromHeader] string auth)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return null;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            return await RecipeStore.GetStared(user);
        }

        [HttpGet("{id}")]
        public List<Recipe> Get([FromHeader] string auth, [FromRoute] long id)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return null;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            return RecipeSearch.SearchRecipe(new List<long>{id});
        }
    }
}