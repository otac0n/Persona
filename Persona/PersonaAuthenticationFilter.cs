// -----------------------------------------------------------------------
// <copyright file="PersonaAuthenticationFilter.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Persona
{
    using System;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    /// <summary>
    /// Provides authentication via a global filter.
    /// </summary>
    public class PersonaAuthenticationFilter : IAuthorizationFilter
    {
        private readonly string audience;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonaAuthenticationFilter"/> class.
        /// </summary>
        public PersonaAuthenticationFilter(string audience)
        {
            this.audience = audience;
        }

        /// <summary>
        /// Called when authorization is required.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            HttpCookie newCookie;
            var identity = new PersonaAuth().Authenticate(filterContext.HttpContext.Request.Cookies, audience, out newCookie);

            if (identity != null)
            {
                var roles = Roles.Enabled ? Roles.GetRolesForUser(identity.Name) : new string[0];
                filterContext.HttpContext.User = new GenericPrincipal(identity, roles);
            }

            if (newCookie != null)
            {
                filterContext.HttpContext.Response.AppendCookie(newCookie);
            }
        }
    }
}
