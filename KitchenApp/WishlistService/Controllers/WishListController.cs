using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KitchenLib;
using KitchenLib.Database;

namespace WishlistService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WishListController : ControllerBase
    {
        [HttpGet]
        public Inventory<Product> Info(string wishlist, string user) {
            var u = UserStore.Get(user).Result;
            var wishL = WishlistStore.Get(wishlist, user).Result;
            //If user or wishlist does not exist it returns
            if(u == null||wishL == null) {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                HttpContext.Response.Headers.Remove("token");
                return null;//return new Inventory<Product>();
            }
            HttpContext.Response.Headers.Remove("token");
            return wishL;
        }
        //Ainda nao
        [HttpPost]
        public void AddProduct(string user, string wishlist, string product) {

        }

        [HttpPost]
        public void Add(string user, string name) {
            var u = UserStore.Get(user).Result;
            if (u == null) {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                HttpContext.Response.Headers.Remove("token");
                return;
            }
            _ = WishlistStore.Add(name,user);
            HttpContext.Response.Headers.Remove("token");
        }

        //Ainda nao ?
        [HttpPost]
        public void Edit(string user, string new_name) {

        }

        [HttpDelete]
        public void Remove(string user, string wishlist) {
            var u = UserStore.Get(user).Result;
            if (u == null) {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                HttpContext.Response.Headers.Remove("token");
                return;
            }
            //removes wishlist with name 'wishlist' form the user based on it's email
            _ = WishlistStore.Remove(wishlist, u._email);
            HttpContext.Response.Headers.Remove("token");
        }

        //Ainda nao
        [HttpPost]
        public void RemoveProduct(string user, string wishlist, string product) {

        }
    }
}
