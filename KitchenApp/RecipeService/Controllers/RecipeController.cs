using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using KitchenLib;
using KitchenLib.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
            var list = RecipeSearch.SearchRecipe(50, keys);
            foreach (var v in list)
            {
                await RecipeStore.Add(new MinimalRecipe(v));
            }

            return list;
        }

        [HttpPost]
        public async void Star([FromHeader] string auth, [FromForm] string id)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            await RecipeStore.star(id, user);
        }
        
        [HttpPost]
        public async void Unstar([FromHeader] string auth, [FromForm] string id)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            await RecipeStore.unstar(id, user);
        }
    }
}