// -----------------------------------------------------------------------
// <copyright file="AuthController.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace PersonaExample.Controllers
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web;
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
        /// Provides a page where anonymous users can log in.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>A result indicating the response to the user.</returns>
        public ActionResult Index(string returnUrl = null)
        {
            Uri uri = null;
            if (returnUrl != null)
            {
                Uri.TryCreate(returnUrl, UriKind.Relative, out uri);
            }

            if (uri == null && returnUrl != null)
            {
                return this.RedirectToAction("index", "auth");
            }

            ViewBag.ReturnUrl = uri ?? new Uri(this.Url.Content("~/"), UriKind.Relative);
            return this.View();
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
            var cookie = await this.auth.Login(assertion);
            if (cookie == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Bad Request");
            }

            this.Response.Cookies.Remove(PersonaAuth.CookieName);
            this.Response.AppendCookie(cookie);

            HttpCookie ignore;
            var identity = this.auth.Authenticate(this.Response.Cookies, out ignore);
            if (identity == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Internal Sever Error");
            }

            return this.Content(identity.Name);
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
                this.Response.Cookies.Remove(PersonaAuth.CookieName);
                this.Response.AppendCookie(cookie);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK, "OK");
        }

        /// <summary>
        /// Allows client-side scripts to refresh their CSRF token.
        /// </summary>
        /// <returns>A <see cref="PartialViewResult"/> object.</returns>
        public ActionResult Token()
        {
            return this.PartialView();
        }
    }
}
