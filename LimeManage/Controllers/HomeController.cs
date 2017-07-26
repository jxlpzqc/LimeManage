using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FineUIMvc;

namespace LimeManage.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            if (Session["username"] == null || Session["username"].Equals("")) { return RedirectToAction("Index","Session"); }
            return View();
        }

        public ActionResult Hello()
        {
            if (Session["username"] == null || Session["username"].Equals("")) { return RedirectToAction("Index", "Session"); }
            return View();
        }

        public ActionResult LackOfJS()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult btnHello_Click()
        {
            Alert.Show("你好 FineUI！", MessageBoxIcon.Warning);

            return UIHelper.Result();
        }

        // GET: Themes
        public ActionResult Themes()
        {
            if (Session["username"] == null || Session["username"].Equals("")) { return RedirectToAction("Index", "Session"); }
            return View();
        }
    }
}