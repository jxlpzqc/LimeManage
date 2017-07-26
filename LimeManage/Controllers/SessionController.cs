using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FineUIMvc;
using LimeManage.Models;
using Newtonsoft.Json;

namespace LimeManage.Controllers
{
    public class SessionController : BaseController
    {
        //
        // GET: /Session/
        public ActionResult Index()
        {
            if (Session["username"] != null && !Session["username"].Equals("")) { return RedirectToAction("Index", "Home"); }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string UserName,string Password)
        {
            using ( var db = new ProjectManagementEntities())
            {
                var userResult = db.User.Where(u => (u.UserName.Equals(UserName) && u.Password.Equals(Password)));
                if (userResult.Count() > 0)
                {
                    User user = userResult.FirstOrDefault();
                    Session["username"] = UserName;
                    Session["permisson"] = user.UserClass.Permission;
                    return RedirectToAction("Index","Home");
                    
                }
                else
                {
                    ShowNotify("登录失败");
                    return UIHelper.Result();
                }
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            Session["username"] = null;
            Session["permisson"] = null;
            return RedirectToAction("Index");
        }
	}
}