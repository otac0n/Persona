// -----------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace PersonaExample
{
    using System;
    using System.Security.Cryptography;
    using System.Security.Principal;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

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
            var cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null)
            {
                return;
            }

            var email = (string)null;
            try
            {
                var token = cookie.Value;
                var resultString = Encoding.UTF8.GetString(MachineKey.Unprotect(Convert.FromBase64String(token), FormsAuthentication.FormsCookieName));
                var result = JObject.Parse(resultString);
                var expires = Controllers.AuthController.Epoch.AddMilliseconds((double)result["expires"]);
                if (expires > DateTime.UtcNow)
                {
                    email = (string)result["email"];
                }
            }
            catch (ArgumentException)
            {
            }
            catch (CryptographicException)
            {
            }
            catch (FormatException)
            {
            }
            catch (JsonReaderException)
            {
            }

            if (email == null)
            {
                return;
            }

            var identity = new GenericIdentity(email);

            Thread.CurrentPrincipal = HttpContext.Current.User = new GenericPrincipal(identity, new string[0]);
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
