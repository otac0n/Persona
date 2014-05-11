// -----------------------------------------------------------------------
// <copyright file="PersonaAuth.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Persona
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Security.Principal;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Security;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Provides BrowserID authentication services using the Mozilla Persona implementation.
    /// </summary>
    public class PersonaAuth
    {
        static PersonaAuth()
        {
            CookieDomain = "";
            CookiePath = "/";
            CookieName = "AuthToken";
            CookieSecure = false;
            Timeout = TimeSpan.FromHours(1);
        }

        /// <summary>
        /// Gets or sets the value of the domain of the authentication token cookie.
        /// </summary>
        public static string CookieDomain { get; set; }

        /// <summary>
        /// Gets or sets the name of the cookie used to store the authentication token.
        /// </summary>
        public static string CookieName { get; set; }

        /// <summary>
        /// Gets or sets the value of the path of the authentication token cookie.
        /// </summary>
        public static string CookiePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the authentication token cookie is only sent over HTTPS.
        /// </summary>
        public static bool CookieSecure { get; set; }

        /// <summary>
        /// Gets or sets the timeout of the authentication token cookie.
        /// </summary>
        public static TimeSpan Timeout { get; set; }

        /// <summary>
        /// Authenticates an HTTP request's cookies.
        /// </summary>
        /// <param name="cookies">The cookie collection containing the authentication token to validate.</param>
        /// <param name="audience">The protocol, domain name, and port of your site. For example, "https://example.com:443".</param>
        /// <param name="newCookie">An updated cookie.</param>
        /// <returns>The <see cref="IIdentity"/> of the authenticated user, or null if authentication failed.</returns>
        public virtual IIdentity Authenticate(HttpCookieCollection cookies, string audience, out HttpCookie newCookie)
        {
            if (cookies == null)
            {
                throw new ArgumentNullException("cookies");
            }
            else if (audience == null)
            {
                throw new ArgumentNullException("audience");
            }

            newCookie = null;

            var cookie = cookies[CookieName];
            if (cookie != null)
            {
                try
                {
                    var result = VerifyResult.Parse(Unprotect(cookie.Value));
                    var remaining = (result.Issued + Timeout) - DateTimeOffset.UtcNow;
                    if (result.Status == "okay" && result.Audience == audience && remaining > TimeSpan.Zero)
                    {
                        if (CookieSecure && remaining.TotalMilliseconds < Timeout.TotalMilliseconds / 4)
                        {
                            result.Issued = DateTimeOffset.UtcNow;
                            newCookie = MakeCookie(result);
                        }

                        return new GenericIdentity(result.Email);
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

                newCookie = ExpireCookie();
            }

            return null;
        }

        /// <summary>
        /// Verifies the specified assertion and creates an authentication token as an asynchronous operation using a task object.
        /// </summary>
        /// <param name="assertion">The assertion supplied by the user.</param>
        /// <param name="audience">The protocol, domain name, and port of your site. For example, "https://example.com:443".</param>
        /// <returns>The task representing the asynchronous operation.</returns>
        public virtual async Task<HttpCookie> Login(string assertion, string audience)
        {
            if (audience == null)
            {
                throw new ArgumentNullException("audience");
            }
            else if (string.IsNullOrEmpty(assertion))
            {
                return null;
            }

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "assertion", assertion },
                { "audience", audience },
            });

            using (var client = new HttpClient())
            {
                using (var response = await client.PostAsync("https://verifier.login.persona.org/verify", content))
                {
                    response.EnsureSuccessStatusCode();
                    var result = VerifyResult.Parse(await response.Content.ReadAsStringAsync());
                    if (result.Status == "okay" && result.Audience == audience && result.Expires > DateTimeOffset.UtcNow)
                    {
                        result.Issued = DateTimeOffset.UtcNow;
                        return MakeCookie(result);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Creates a cookie that will log the user out if an authentication token is present in the request cookies.
        /// </summary>
        /// <param name="cookies">The cookie collection of the request.</param>
        /// <returns>A cookie, if the request contains an authentication token; null, otherwise.</returns>
        public virtual HttpCookie Logout(HttpCookieCollection cookies)
        {
            if (cookies == null)
            {
                throw new ArgumentNullException("cookies");
            }
            else if (cookies[CookieName] == null)
            {
                return null;
            }

            return ExpireCookie();
        }

        private static HttpCookie ExpireCookie()
        {
            return new HttpCookie(CookieName)
            {
                Expires = DateTime.UtcNow.AddYears(-1),
                Domain = CookieDomain,
                Path = CookiePath,
                HttpOnly = true,
                Secure = false,
            };
        }

        private static HttpCookie MakeCookie(VerifyResult result)
        {
            return new HttpCookie(CookieName, Protect(result.ToString()))
            {
                Domain = CookieDomain,
                Path = CookiePath,
                HttpOnly = true,
                Secure = CookieSecure,
            };
        }

        private static string Protect(string value)
        {
            return Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(value), CookieName));
        }

        private static string Unprotect(string value)
        {
            return Encoding.UTF8.GetString(MachineKey.Unprotect(Convert.FromBase64String(value), CookieName));
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "This is instantiated by the JSON parser.")]
        private class VerifyResult
        {
            public string Audience { get; set; }

            public string Email { get; set; }

            [JsonConverter(typeof(UnixEpochDateTimeConverter))]
            public DateTimeOffset Expires { get; set; }

            [JsonConverter(typeof(UnixEpochDateTimeConverter))]
            public DateTimeOffset Issued { get; set; }

            public string Issuer { get; set; }

            public string Reason { get; set; }

            public string Status { get; set; }

            public static VerifyResult Parse(string json)
            {
                return JsonConvert.DeserializeObject<VerifyResult>(json);
            }

            public override string ToString()
            {
                return JsonConvert.SerializeObject(this, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                });
            }
        }
    }
}
