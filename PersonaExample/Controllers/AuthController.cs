﻿// -----------------------------------------------------------------------
// <copyright file="AuthController.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace PersonaExample.Controllers
{
    using System.Configuration;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Persona;

    /// <summary>
    /// Provides authentication services.
    /// </summary>
    public class AuthController : AsyncController
    {
        private readonly PersonaAuth auth;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        public AuthController()
        {
            this.auth = new PersonaAuth();
        }

        /// <summary>
        /// Verifies an authentication assertion and logs the user in as an asynchronous operation using a task object.
        /// </summary>
        /// <param name="assertion">The assertion to verify.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(string assertion)
        {
            var cookie = await this.auth.Login(assertion, ConfigurationManager.AppSettings["PersonaAudience"]);
            if (cookie == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Bad Request");
            }

            this.Response.AppendCookie(cookie);
            return new HttpStatusCodeResult(HttpStatusCode.OK, "OK");
        }

        /// <summary>
        /// Logs the user out.
        /// </summary>
        /// <returns>A result indicating the response to the user.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            var cookie = this.auth.Logout(this.Request.Cookies);
            if (cookie != null)
            {
                this.Response.AppendCookie(cookie);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK, "OK");
        }
    }
}
