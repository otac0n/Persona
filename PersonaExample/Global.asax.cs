// -----------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace PersonaExample
{
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

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
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
