using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentOutSourceAPI.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ContentOutSourceAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {

            CreateHostBuilder(args).Build().Run();
            //var usercontroller = new UsersController();
           // Console.WriteLine(usercontroller.FindMacthingWriterByPostKeywords(1, "cuong"));
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
