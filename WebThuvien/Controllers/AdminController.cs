using System;
using System.Collections.Generic;
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