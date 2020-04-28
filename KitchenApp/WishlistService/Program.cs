using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using KitchenLib;
using KitchenLib.Database;

namespace WishlistService
{
    public class Program
    {
        public static void Main(string[] args) {
            //new Program().NovoTmp();
            CreateHostBuilder(args).Build().Run();
        }
        public async void NovoTmp() {
            User u = new User("pedro", "abc@gmail.com", "123", new Neo4j.Driver.LocalDateTime(DateTime.Now), 123456789);
            //await UserStore.Add(u);
            
            Inventory<Product> inventory = new Inventory<Product>();
            //await WishlistStore.Add("pedroWish",u._name);
            //await WishlistStore.Remove("pedroWish", u._email);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
