using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BooksLibrary
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Register",
                url: "rejestracja/",
                defaults: new { controller = "Account", action = "Register" }
                );

            routes.MapRoute(
                name: "Login",
                url: "logowanie/{returnUrl}",
                defaults: new { controller = "Account", action = "Login", returnUrl = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "ConfirmAccount",
                url: "{controller}/{action}/{confirmationToken}",
                defaults: new { controller = "Account", action = "ConfirmAccount", confirmationTorken = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}