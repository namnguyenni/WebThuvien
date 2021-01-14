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
        //trang chủ trang web
        public ActionResult Index()
        {
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
        #region partial trang home client

        public ActionResult HeaderSection()
        {

            return View();
        }
        public ActionResult SliderSection()
        {
            QLTHUVIEN db = new QLTHUVIEN();
            //SÁCH LOẠI 1
            SACH sach1 = db.SACHes.First(x=>x.MALOAISACH == "1");
            //SÁCH LOẠI 2
            SACH sach2 = db.SACHes.First(x => x.MALOAISACH == "2");
            //SÁCH LOẠI 3
            SACH sach3 = db.SACHes.First(x => x.MALOAISACH == "3");

            ViewBag.sach1 = sach1;
            ViewBag.sach2 = sach2;
            ViewBag.sach3 = sach3;

            return View();
        }
        public ActionResult SearchSection()
        {
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

        public ActionResult Timkiemsach(string noidungnhap)
        {
            string noidung = noidungnhap.ToUpper();
            QLTHUVIEN db = new QLTHUVIEN();
            List<SACH> SearchSach = new List<SACH>();
            //tim kiem gần đúng trên tên sách
            List<SACH> SearchSachName = db.SACHes.Where(x => x.TENSACH.Contains(noidung) == true).ToList();
            SearchSach.AddRange(SearchSachName);
            //tìm kiếm vào tên tác giả
            List<SACH> SearchSachTacgia = db.Database.SqlQuery<SACH>("exec dbo.SearchSachTacgia").ToList();
            SearchSach.AddRange(SearchSachTacgia);
            //tim kiếm theo lĩnh vực
            List<LINHVUC> Linhvulienquan = db.LINHVUCs.Where(x => x.TENLINHVUC.Contains(noidung) == true).ToList();
            
            foreach (var item in Linhvulienquan)
            {
                List<SACH> sach =  db.SACHes.Where(x => x.MALINHVUC == item.MALINHVUC).ToList();//tìm kiếm sách có lĩnh vực
                SearchSach.AddRange(sach);//thêm vào sách search
            }
            
            ViewBag.SearchSach = SearchSach;


            return View();
        }



    }
}