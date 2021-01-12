using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebThuvien.Models.Entity;
namespace WebThuvien.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {

            QLTHUVIEN db = new QLTHUVIEN();
            //SÁCH MỚI CẬP NHẬT : 10 sách
            List<SACH> TOPNEWBOOK10 = db.Database.SqlQuery<SACH>("exec dbo.TOPNEWBOOK10").ToList();
            ViewBag.TOPNEWBOOK10 = TOPNEWBOOK10;
            //SÁCH XEM NHIỀU TOPPOPULARBOOK
            List<SACH> TOPPOPULARBOOK = db.Database.SqlQuery<SACH>("exec dbo.TOPPOPULARBOOK").ToList();
            ViewBag.TOPPOPULARBOOK = TOPPOPULARBOOK;
            //SÁCH THEO LĨNH VỰC



            return View();
        }



        public ActionResult Chitietsach(string masach)
        {
            //thong tin cuốn sách đó

            //những sách có liên quan

            //bình luận về cuốn sách này


            return View();
        }

        public ActionResult Timkiemsach(string noidungnhap)
        {
            //tim kiem gần đúng

            return View();
        }



    }
}