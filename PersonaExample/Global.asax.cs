// -----------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace PersonaExample
{
    using System.Configuration;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Persona;

    /// <summary>
    /// The PersonaExample application.
    /// </summary>
    public class MvcApplication : HttpApplication
    {
        /// <summary>
        /// Starts the application.
        /// </summary>
        protected virtual void Application_Start()
        {
            GlobalFilters.Filters.Add(new PersonaAuthenticationFilter(ConfigurationManager.AppSettings["PersonaAudience"]));
            AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
