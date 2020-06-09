using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using KitchenLib;
using KitchenLib.Database;
using Microsoft.AspNetCore.Mvc;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("[action]")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("{prod}")]
        public Product Get([FromHeader] string auth, [FromRoute] string prod)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return null;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            var product = ProductStore.Get(prod).Result;
            if (product != null) return product;
            HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
            return null;
        }

        [HttpPost]
        public async Task<Product> Add([FromHeader] string auth, [FromForm] string name, [FromForm] string category,
            [FromForm] long quantity, [FromForm] string units, [FromForm] double price)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return null;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            //TODO Check products with same name
            var p = new Product(name, category, quantity, units, price);
            await ProductStore.Add(p);
            return p;
        }
        
/*
        [HttpPost]
        public async Task<Product> Edit([FromHeader] string auth, [FromForm] string uid,
            [FromForm] string name = null,
            [FromForm] string category = null,
            [FromForm] uint? quantity = null, [FromForm] string units = null, [FromForm] float? price = null)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return null;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            return await ProductStore.Edit(uid, name, category, quantity, units, price);
        }

        [HttpPost]
        [Route("{prod}")]
        public async Task<bool> Remove([FromHeader] string auth, [FromRoute] string prod)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return false;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            return await ProductStore.Remove("prod");
        }
*/

        
        [HttpPost]
        public async Task<List<Product>> Search([FromHeader] string auth, [FromForm] string regex)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return null;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            return await ProductStore.Search(regex);
        }
        
        [HttpPost]
        public async Task<List<Product>> Search([FromHeader] string auth, [FromForm] string regex, [FromForm] string category)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return null;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            return await ProductStore.Search(regex, category);
        }
        
        [HttpGet]
        public async Task<List<Product>> GetAll([FromHeader] string auth)
        {
             string user;
             if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
             {
                 HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                 return null;
             }
 
             HttpContext.Response.Headers.Add("auth", auth);
             return await ProductStore.GetAll();
             
        }
    }
}