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
            HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
            return null;
        }

        [HttpPost]
        public async Task<Product> Add([FromHeader] string auth, [FromHeader] string name, [FromHeader] string category,
            [FromHeader] uint quantity, [FromHeader] string units, [FromHeader] float price)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return null;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            var p = new Product(name, category, quantity, units, price);
            await ProductStore.Add(p);
            return p;
        }

        [HttpPost]
        public async Task<Product> Edit([FromHeader] string auth, [FromHeader] string uid,
            [FromHeader] string name = null,
            [FromHeader] string category = null,
            [FromHeader] uint? quantity = null, [FromHeader] string units = null, [FromHeader] float? price = null)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return null;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            return null;
        }

        [HttpDelete]
        [Route("{prod}")]
        public async void Remove([FromHeader] string auth, [FromRoute] string prod)
        {
            string user;
            if ((user = JwtBuilder.UserJwtToken(auth).Result) == null || !UserStore.Exists(user).Result)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                return;
            }

            HttpContext.Response.Headers.Add("auth", auth);
            await ProductStore.Remove("prod");
        }

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