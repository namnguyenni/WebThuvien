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
            List<SACH> TOPNEWBOOK10 = db.Database.SqlQuery<SACH>("exec dbo.TOPNEWBOOK10").ToList();
            ViewBag.TOPNEWBOOK10 = TOPNEWBOOK10;
            //SÁCH XEM NHIỀU TOPPOPULARBOOK
            List<SACH> TOPPOPULARBOOK = db.Database.SqlQuery<SACH>("exec dbo.TOPPOPULARBOOK").ToList();
            ViewBag.TOPPOPULARBOOK = TOPPOPULARBOOK;
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

            return View();
        }
        public ActionResult SearchSection()
        {
            return View();
        }
        public ActionResult FeaturesSection()
        {
            return View();
        }
        //danh mục sách trong kho
        public ActionResult CatagorySection()
        {
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



        public ActionResult Chitietsach(string masach)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            //thong tin cuốn sách đó
            SACH detailSach = db.SACHes.Single(x => x.MASACH == masach);
            //những sách có liên quan
            //những cuốn sách cùng loại
            List<SACH> SachLienQuan_Loai = db.SACHes.Where(x => x.MALOAISACH == detailSach.MALOAISACH).Take(3).ToList();
            ViewBag.SachLienQuanCungLoai = SachLienQuan_Loai;
            //những cuốn sách liên quan về lĩnh vực
            List<SACH> SachLienQuan_LinhVuc = db.SACHes.Where(x => x.MALINHVUC == detailSach.MALINHVUC).Take(3).ToList();
            ViewBag.SachLienQuan_LinhVuc = SachLienQuan_LinhVuc;
            //những sách cùng tác giả
            List<SACH> SachLienQuan_Tacgia = db.SACHes.Where(x => x.MATACGIA == detailSach.MATACGIA).Take(3).ToList();
            ViewBag.SachLienQuan_Tacgia = SachLienQuan_Tacgia;
            //bình luận về cuốn sách này
            List<BINHLUAN> binhluan = db.BINHLUANs.Where(x => x.MASACH == masach).ToList();
            ViewBag.binhluan = binhluan;
            return View(detailSach);
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