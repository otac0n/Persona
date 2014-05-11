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
    using System.Security.Principal;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Persona;

    /// <summary>
    /// The PersonaExample application.
    /// </summary>
    public class MvcApplication : HttpApplication
    {
        /// <summary>
        /// Authenticates the request.
        /// </summary>
        protected void Application_AuthenticateRequest()
        {
            HttpCookie newCookie;
            var identity = new PersonaAuth().Authenticate(HttpContext.Current.Request.Cookies, ConfigurationManager.AppSettings["PersonaAudience"], out newCookie);

            if (identity != null)
            {
                var roles = new string[0];  // TODO: Get the roles for the given identity.
                Thread.CurrentPrincipal = HttpContext.Current.User = new GenericPrincipal(identity, roles);
            }

            if (newCookie != null)
            {
                HttpContext.Current.Response.AppendCookie(newCookie);
            }
        }

        /// <summary>
        /// Starts the application.
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
