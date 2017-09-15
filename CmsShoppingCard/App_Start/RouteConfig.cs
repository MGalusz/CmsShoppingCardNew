using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CmsShoppingCard
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Cart", "Cart/{action}/{id}", new { controller = "Cart", action = "Index", id = UrlParameter.Optional }, new[] { "CmsShoppingCard.Controllers" });

            routes.MapRoute("Shop", "Shop/{action}/{name}", new { controller = "Shop", action = "Index", name = UrlParameter.Optional }, new[] { "CmsShoppingCard.Controllers" });

            routes.MapRoute("SidebarPartial", "Pages/SidebarPartial", new { controller = "Pages", action = "SidebarPartial" }, new[] { "CmsShoppingCard.Controllers" });
            routes.MapRoute("PagesMenuPartial", "Pages/PagesMenuPartial", new { Controller = "Pages", action = "PagesMenuPartial" }, new[] { "CmsShoppingCard.Controllers" });
            routes.MapRoute("Pages", "{page}", new { Controller = "Pages", action = "Index" }, new[] { "CmsShoppingCard.Controllers" });
            routes.MapRoute("Default", "", new { Controller = "Pages", action = "Index" }, new[] { "CmsShoppingCard.Controllers" });

                        
            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}
