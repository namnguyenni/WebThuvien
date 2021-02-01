using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebThuvien.Controllers
{
    public class LienheController : Controller
    {
        // GET: Lienhe
        public ActionResult Index()
        {
            if (Session["TaikhoanBanDoc"] == null)
            {
                return Redirect("/Home/Login");
            }
            return View();
        }
    }
}