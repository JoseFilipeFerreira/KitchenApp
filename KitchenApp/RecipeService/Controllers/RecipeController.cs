using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using KitchenLib;
using KitchenLib.Database;
using Microsoft.AspNetCore.Mvc;

namespace RecipeService.Controllers
{
    [ApiController]
    [Route("[action]")]
    public class RecipeController : ControllerBase
    {
        private string API_KEY = ConfigurationManager.AppSettings.Get("SpoonacularKey");
        
        [HttpPost]
        public async Task<List<MinimalRecipe>> Search([FromHeader] string auth, [FromForm] string keys, [FromForm] string inventory)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return null;
            }

            List<MinimalRecipe> list;

            HttpContext.Response.Headers.Add("auth", auth);
            if (inventory != null)
            {
                var inv = InventoryStore.Get(inventory, user).Result;
                if (inv == null)
                {
                    HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                    return null;
                }

                var r = new List<Product>(inv._products);
                list = RecipeSearch.SearchMinimalRecipe(API_KEY, 10, r);
            }

            else
            {
                list = RecipeSearch.SearchMinimalRecipe(API_KEY, 10, keys);
            }

            foreach (var v in list)
            {
                await RecipeStore.Add(v);
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
            return RecipeSearch.SearchRecipe(API_KEY, new List<long>{id});
        }
    }
}