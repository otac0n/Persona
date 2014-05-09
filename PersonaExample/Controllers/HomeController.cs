// -----------------------------------------------------------------------
// <copyright file="HomeController.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace PersonaExample.Controllers
{
    using System.Web.Mvc;

    /// <summary>
    /// An example controller.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// An example action.
        /// </summary>
        /// <returns>A <see cref="ViewResult"/> object.</returns>
        public ActionResult Index()
        {
            return this.View();
        }
    }
}
