using BooksLibrary.Hangfire;
using BooksLibrary.Infrastructure;
using Hangfire;
using Microsoft.Owin;
using Ninject;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[assembly: OwinStartup(typeof(BooksLibrary.Startup))]
namespace BooksLibrary
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("HangFireConntectionString");


            var kernel = NinjectControllerFactory.GetKernel();
            kernel.Bind<IHangfireService>().To<HangfireService>();
            GlobalConfiguration.Configuration.UseNinjectActivator(kernel);

            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}