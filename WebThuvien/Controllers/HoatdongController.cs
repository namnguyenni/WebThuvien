using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebThuvien.Models.Entity;
namespace WebThuvien.Controllers
{
    public class HoatdongController : Controller
    {
        // GET: Hoatdong
        public ActionResult Index(int pageNumber=1)
        {
            if (Session["TaikhoanBanDoc"] == null)
            {
                return Redirect("/Home/Login");
            }
            //lấy danh sách hoạt động
            QLTHUVIEN db = new QLTHUVIEN();
            List<BAIDANGTHONGTIN> baidangs = db.BAIDANGTHONGTINs.OrderBy(x=>x.THOIGIANBATDAU).ToList();

            //những bài đăng mới
            ViewBag.newbaidang = baidangs.Take(3).ToList();
            //phần trang : mỗi tragn tối đa 5 bài
            if (baidangs.Count < 5)
            {
                ViewBag.baidangs = baidangs;
            }
            else if (baidangs.Count - (pageNumber - 1) * 5 > 0 && baidangs.Count - (pageNumber - 1) * 5 < 5)
            {
                ViewBag.baidangs = baidangs.GetRange((pageNumber - 1) * 5, baidangs.Count - (pageNumber - 1) * 5);

            }
            else
            {
                ViewBag.baidangs = baidangs.GetRange((pageNumber - 1) * 5, 5);
            }

            

            return View();
        }

        public ActionResult Chitiet(int mabaidang)
        {
            if (Session["TaikhoanBanDoc"] == null)
            {
                return Redirect("/Home/Login");
            }

            QLTHUVIEN db = new QLTHUVIEN();
            BAIDANGTHONGTIN baidang = db.BAIDANGTHONGTINs.SingleOrDefault(x => x.MABAIDANG == mabaidang);
            if (baidang==null)
            {
                return Redirect("/Error");
            }
            ViewBag.baidang = baidang;
            List<BAIDANGTHONGTIN> baidangs = db.BAIDANGTHONGTINs.OrderBy(x => x.THOIGIANBATDAU).ToList();
            //những bài đăng mới
            ViewBag.newbaidang = baidangs.Take(3).ToList();

            return View();
        }

    }
}