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
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Provides authentication services.
    /// </summary>
    public class AuthController : AsyncController
    {
        internal static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Verifies an authentication assertion and logs the user in as an asynchronous operation using a task object.
        /// </summary>
        /// <param name="assertion">The assertion to verify.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(string assertion)
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "assertion", assertion },
                { "audience", this.Request.Url.Scheme + "://" + this.Request.Url.Host + ":" + this.Request.Url.Port },
            });

            using (var client = new HttpClient())
            {
                using (var response = await client.PostAsync("https://verifier.login.persona.org/verify", content))
                {
                    response.EnsureSuccessStatusCode();
                    var resultString = await response.Content.ReadAsStringAsync();
                    var result = JObject.Parse(resultString);
                    if ((string)result["status"] == "okay")
                    {
                        var token = Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(resultString), FormsAuthentication.FormsCookieName));

                        this.Response.AppendCookie(new HttpCookie(FormsAuthentication.FormsCookieName)
                        {
                            HttpOnly = true,
                            Secure = this.Request.Url.Scheme == "https",
                            Path = FormsAuthentication.FormsCookiePath,
                            Expires = Epoch.AddMilliseconds((double)result["expires"]),
                            Value = token,
                        });

                        return new HttpStatusCodeResult(HttpStatusCode.OK, "OK");
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Bad Request");
                    }
                }
            }
        }

        /// <summary>
        /// Logs the user out.
        /// </summary>
        /// <returns>A result indicating the response to the user.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            if (this.Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                this.Response.AppendCookie(new HttpCookie(FormsAuthentication.FormsCookieName)
                {
                    Path = FormsAuthentication.FormsCookiePath,
                    Expires = Epoch,
                });
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK, "OK");
        }
    }
}
