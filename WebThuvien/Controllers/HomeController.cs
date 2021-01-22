using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebThuvien.Models.Function;
using WebThuvien.Models.Entity;
using System.Data.SqlClient;
using System.Data;

namespace WebThuvien.Controllers
{
    public class HomeController : Controller
    {
        //trang chủ trang web
        public ActionResult Index()
        {
            if (Session["TaikhoanBanDoc"] == null)
            {
                return Redirect("/Home/Login");
            }

            QLTHUVIEN db = new QLTHUVIEN();
            //SÁCH MỚI CẬP NHẬT : 10 sách
            //List<SACH> TOPNEWBOOK10 = db.Database.SqlQuery<SACH>("exec dbo.TOPNEWBOOK10").ToList();
            //ViewBag.TOPNEWBOOK10 = TOPNEWBOOK10;
            ////SÁCH XEM NHIỀU TOPPOPULARBOOK
            //List<SACH> TOPPOPULARBOOK = db.Database.SqlQuery<SACH>("exec dbo.TOPPOPULARBOOK").ToList();
            //ViewBag.TOPPOPULARBOOK = TOPPOPULARBOOK;
            //SÁCH THEO LĨNH VỰC
            ViewBag.TatcaSach = db.SACHes.ToList();
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {

            return View();
        }

        public ActionResult Logout()
        {
            if (Session["TaikhoanBanDoc"] != null)
            {
                Session.Remove("TaikhoanBanDoc");
            }

            if (Session["Taikhoan"] != null)
            {
                Session.Remove("Taikhoan");
                Session.Remove("TaikhoanBanDoc");

            }
            return View("Login");
        }


        #region partial trang home client

        public ActionResult HeaderSection()
        {

            return View();
        }
        public ActionResult SliderSection()
        {
            QLTHUVIEN db = new QLTHUVIEN();

            List<SACH> SliderSach = db.Database.SqlQuery<SACH>("exec dbo.GetTopSach_LuotXem ").ToList();
            ViewBag.Sach = SliderSach;

            return View();
        }
        public ActionResult SearchSection()
        {
            QLTHUVIEN db = new QLTHUVIEN();
            List<LOAISACH> list_loaisach = db.LOAISACHes.ToList();
            ViewBag.list_loaisach = list_loaisach;
            List<LINHVUC> list_linhvuc = db.LINHVUCs.ToList();
            ViewBag.list_linhvuc = list_linhvuc;

            return View();
        }
        public ActionResult FeaturesSection()
        {
            QLTHUVIEN db = new QLTHUVIEN();
            List<LOAISACH> Loaisach = new List<LOAISACH>();
            Loaisach = db.LOAISACHes.ToList();
            ViewBag.Loaisach = Loaisach;
            return View();
        }
        //danh mục sách trong kho
        public ActionResult CatagorySection()
        {
            QLTHUVIEN db = new QLTHUVIEN();
            List<SACH> Tatcasach = new List<SACH>();
            //LINH VỰC
            List<LINHVUC> listLinhvuc = db.LINHVUCs.ToList();
            ViewBag.Linhvuc = listLinhvuc;
            //SÁCH THEO LĨNH VỰC
            foreach (var item in listLinhvuc)
            {
                List<SACH> sach_linhvuc = db.SACHes.Where(x => x.MALINHVUC == item.MALINHVUC).Take(10).ToList();
                Tatcasach.AddRange(sach_linhvuc);
            }
            ViewBag.TatcaSach = Tatcasach;
            return View();
        }

        public ActionResult CountBook()
        {
            QLTHUVIEN db = new QLTHUVIEN();
            List<LOAISACH> list_loai = db.LOAISACHes.ToList();
            ViewBag.list_loaisach = list_loai;
            List<int> soluong = new List<int>();
            foreach (var item in list_loai)
            {
                int soluong_sub = db.SACHes.Count(x => x.MALOAISACH == item.MATHELOAI);
                soluong.Add(soluong_sub);
            }
            ViewBag.soluong = soluong;
            return View();
        }
        public ActionResult EventSection()
        {
            return View();
        }
        #endregion

        public ActionResult MenuSach()
        {
            QLTHUVIEN db = new QLTHUVIEN();
            //lấy tất cả lĩnh vực
            ViewBag.Linhvuc = db.LINHVUCs.ToList();
            //lấy tất cả loại sách
            ViewBag.Loaisach = db.LOAISACHes.ToList();
            

            return View();
        }





    }
}