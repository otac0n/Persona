// -----------------------------------------------------------------------
// <copyright file="RouteConfig.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace PersonaExample
{
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// Contains route registrations.
    /// </summary>
    public static class RouteConfig
    {
        /// <summary>
        /// Registers the application's routes against the specified <see cref="RouteCollection"/>.
        /// </summary>
        /// <param name="routes">The route collection to update.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}
