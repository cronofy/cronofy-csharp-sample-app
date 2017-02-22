﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CronofyCSharpSampleApp.Controllers
{
    public class EnterpriseConnectController : EnterpriseConnectControllerBase
    {
        public ActionResult Index()
        {
            ViewData["resources"] = CronofyHelper.GetResources();

            var users = DatabaseHandler.Many<Persistence.Models.EnterpriseConnectUserData>(String.Format("SELECT CronofyUID, Email, Status FROM EnterpriseConnectUserData WHERE OwnedBy='{0}'", uidCookie.Value));

            ViewData["users"] = users;

            return View ();
        }

        public ActionResult Login()
        {
            return View("Login");
        }
    }
}
