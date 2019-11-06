using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestMixArch.Account.Controls
{
    public class MasterViewController : Controller
    {
        //GET: MasterView
        public ActionResult Index()
        {
            return View();
        }
    }
}