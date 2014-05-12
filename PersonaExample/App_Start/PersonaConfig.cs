// -----------------------------------------------------------------------
// <copyright file="PersonaConfig.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

[assembly: System.Web.PreApplicationStartMethod(typeof(PersonaExample.PersonaConfig), "Configure")]

namespace PersonaExample
{
    using System.Web.Mvc;
    using Persona;

    /// <summary>
    /// Configures ASP.NET to use Persona authentication.
    /// </summary>
    public static class PersonaConfig
    {
        /// <summary>
        /// Performs the configuration.
        /// </summary>
        public static void Configure()
        {
            GlobalFilters.Filters.Add(new PersonaAuthenticationFilter());

            // #error Configure the following values then delete this line before proceeding.

            // The protocol, domain, and port of your application.  This should not be read from the request, since an attacker can tamper with the values in the request.
            // The best practice would be to read this value from a config file.
            PersonaAuth.Audience = "http://localhost:57186";

            // A flag indicating whether or not to use HTTPS-only cookies.  Change this to true if all of your authenticated requests use HTTPS exclusively.
            PersonaAuth.CookieSecure = false;
        }
    }
}
