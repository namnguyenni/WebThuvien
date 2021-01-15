using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebThuvien.Models.Entity;
namespace WebThuvien.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        //giao diện trang chủ admin
        public ActionResult Index()
        {
            //kiểm tra sự tồn tại session

            return View();
        }

        //TẢI TÀI LIỆU ĐÃ SCAN LÊN CHO VÀO TRONG THƯ MỤC Ở SERVER
        public ActionResult Scan()
        {

            return View();
        }

        public ActionResult Sach(int pageNumber=1)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            List<SACH> Sach = new List<SACH>();

            //tim kiem lấy tất cả loại sách
            List<SACH> SachName = db.SACHes.ToList();
            Sach.AddRange(SachName);

            //phân trang và tính toán
            if (Sach.Count < 30)
            {
                ViewBag.Sach = Sach;
            }
            else if (Sach.Count - (pageNumber - 1) * 30 > 0 && Sach.Count - (pageNumber - 1) * 30 < 30)
            {
                ViewBag.Sach = Sach.GetRange((pageNumber - 1) * 30, Sach.Count - (pageNumber - 1) * 30);
            }
            else
            {
                ViewBag.Sach = Sach.GetRange((pageNumber - 1) * 30, 30);
            }
            //số lượng trang
            int countPage = Sach.Count() / 30;
            if (countPage * 30 < Sach.Count())
            {
                countPage += 1;
            }

            ViewBag.countPage = countPage;
            ViewBag.currentPage = pageNumber;

            return View();
        }

        //xem chi tiết sách
        public ActionResult XemChiTietSach(string MaSach)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            SACH sach = db.SACHes.Single(x => x.MASACH == MaSach);

            return View(sach);
        }

        //

        public string SaveScan()
        {

            return "Lưu thành công";

        }

        //đăng nhập admin
        [HttpGet]
        public ActionResult Login()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult Login(string tentaikhoan,string matkhau)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            TAIKHOAN taikhoan = db.TAIKHOANs.Single(x => x.TENTAIKHOAN == tentaikhoan);
            if (taikhoan != null && taikhoan.MATKHAU == matkhau)
            {
                //tạo session

                
            }
            return View();
        }

        //giao diện thêm sách, tải sách từ máy lên
        public ActionResult UploadSach()
        {

            return View();
        }
        //hàm xử lí thêm sách vào thư viện
        [HttpPost]
        public ActionResult UploadSach(SACH sach)
        {
            return View();
        }

        //giao diện thêm sách, tải sách từ máy lên
        public ActionResult EditSach()
        {
            return View();
        }
        //hàm sửa sách
        [HttpPost]
        public ActionResult EditSach(SACH sach)
        {
            return View();
        }

        //hàm xóa cuốn sách ra khỏi thư viện
        [HttpPost]
        public ActionResult DeleteSach(SACH sach)
        {
            return View();
        }

        #region Xử lí mượn trả sách


        #endregion



    }
}