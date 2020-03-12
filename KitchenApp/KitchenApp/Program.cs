using System;
using System.Threading.Tasks;
using KitchenApp.Model;
using KitchenApp.Model.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace KitchenApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var aaa = new UserStore();
            await aaa.Add(new User("mna", "aaa", "ree", DateTime.Now));
            await aaa.Get("aaa");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}