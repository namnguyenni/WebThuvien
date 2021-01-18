using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebThuvien.Models.Entity;
using System.IO;
using WebThuvien.Models.Function;

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

        //bảng thông skee trang chủ admi
        public ActionResult ThongKe1()
        {
            return View();
        }

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
        public ActionResult Login(string tentaikhoan,string matkhau,bool luumk)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            TAIKHOAN taikhoan = db.TAIKHOANs.Single(x => x.TENTAIKHOAN == tentaikhoan);
            if (taikhoan != null && taikhoan.MATKHAU == matkhau)
            {
                //tạo session
                Session["Taikhoan"] = taikhoan;
                if (luumk)
                {
                    HttpCookie taikhoan_ck = new HttpCookie("tentaikhoan", tentaikhoan);
                    HttpCookie matkhau_ck = new HttpCookie("matkhau", matkhau);

                    taikhoan_ck.Expires = DateTime.Now.AddDays(30);
                    matkhau_ck.Expires = DateTime.Now.AddDays(30);

                    Response.Cookies.Add(taikhoan_ck);
                    Response.Cookies.Add(matkhau_ck);

                }

            }
            return View();
        }

        private bool CheckSession(int role)
        {
            if (Session["Taikhoan"] != null)
            {
                TAIKHOAN taikhoan = Session["Taikhoan"] as TAIKHOAN;

                if (taikhoan.LOAITAIKHOAN == role)
                {
                    return true;
                }
            }
            return false;
        }
        //giao diện thêm sách, tải sách từ máy lên
        public ActionResult UploadSach()
        {

            return View();
        }
        //hàm xử lí thêm sách vào thư viện
        [HttpPost]
        public ActionResult UploadSach(SACH sach,HttpPostedFileBase filepdf=null,HttpPostedFileBase hinhanh=null)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            try
            {
                if (filepdf!=null)
                {
                    filepdf.SaveAs(Server.MapPath("~/Content/ClientContent/FILE_PDF/" + sach.TENSACH + "_" + sach.NAMXUATBAN));
                    sach.FILEPATH = sach.TENSACH + "_" + sach.NAMXUATBAN+ Path.GetExtension(filepdf.FileName);
                }
                if (hinhanh != null)
                {
                    filepdf.SaveAs(Server.MapPath("~/Content/ClientContent/images/Books/" + sach.TENSACH + "_" + sach.NAMXUATBAN));
                    sach.HINHANH = sach.TENSACH + "_" + sach.NAMXUATBAN+ Path.GetExtension(hinhanh.FileName);
                }

                //ma sach
                string masach;
                do
                {
                    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    var charsnumber = "1234567890";

                    var stringChars = new char[2];
                    var numberChars = new char[6];
                    var random = new Random();

                    for (int i = 0; i < stringChars.Length; i++)
                    {
                        stringChars[i] = chars[random.Next(chars.Length)];
                    }


                    for (int i = 0; i < numberChars.Length; i++)
                    {
                        numberChars[i] = chars[random.Next(charsnumber.Length)];
                    }

                    //hai chu dau tien
                    masach = new String(stringChars) + new String(numberChars);
                    sach.MASACH = masach;
                    
                }
                while (db.SACHes.SingleOrDefault(x => x.MASACH == masach) != null);

                new QRCode().GenerateQRCode(masach);
                db.SACHes.Add(sach);
                db.SaveChanges();
            }
            catch (Exception)
            {
                ViewBag.Error = "Không thể thêm sách do thiếu thông tin hoặc đã tồn tại sách";
                return Redirect("/Admin/UploadSach?MaSach=");
            }
            return Redirect("/Sach/UploadSach?MaSach=");
        }

        //giao diện thêm sách, tải sách từ máy lên
        public ActionResult ChinhsuaSach(string MaSach)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            try
            {
                SACH sach = db.SACHes.Single(x => x.MASACH == MaSach);
                ViewBag.sach = sach;

            }
            catch (Exception)
            {

                return Redirect("/Admin");
            }


            return View();
        }
        //hàm sửa sách
        [HttpPost]
        public ActionResult ChinhsuaSach(SACH sach,HttpPostedFileBase filepdf=null,HttpPostedFileBase hinhanh=null)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            try
            {
                SACH sach_old = db.SACHes.Single(x => x.MASACH == sach.MASACH);
                if (filepdf != null)
                {
                    
                    System.IO.File.Delete(Server.MapPath("~/Content/ClientContent/FILE_PDF/" + sach_old.FILEPATH));
                    filepdf.SaveAs(Server.MapPath("~/Content/ClientContent/FILE_PDF/" + sach.TENSACH + "_" + sach.NAMXUATBAN));
                    sach.FILEPATH = sach.TENSACH + "_" + sach.NAMXUATBAN + Path.GetExtension(filepdf.FileName);

                }

                if (hinhanh != null)
                {

                    System.IO.File.Delete(Server.MapPath("~/Content/ClientContent/images/Books/" + sach_old.HINHANH));
                    filepdf.SaveAs(Server.MapPath("~/Content/ClientContent/images/Books/" + sach.TENSACH + "_" + sach.NAMXUATBAN));
                    sach.HINHANH = sach.TENSACH + "_" + sach.NAMXUATBAN + Path.GetExtension(hinhanh.FileName);
                }

                sach_old = sach;
                db.SaveChanges();

                return View();
            }
            catch (Exception)
            {

            }

            return View();
        }

        //hàm xóa cuốn sách ra khỏi thư viện
        [HttpPost]
        public ActionResult Xoasach(string MaSach)
        {
            try
            {
                QLTHUVIEN db = new QLTHUVIEN();
                SACH sach = db.SACHes.Single(x => x.MASACH == MaSach);
                db.SACHes.Remove(sach);
                db.SaveChanges();
            }
            catch (Exception)
            {
                ViewBag.Error = "Không xóa được cuốn sách này";
            }
            return View();
        }

        #region Xử lí mượn trả sách
        //tìm kiếm tất cả sách đang mượn thư viện
        public ActionResult TatcaSachDangMuon()
        {
            return View();
        }

        //giao diện cho mượn sách
        public ActionResult MuonSach()
        {
            return View();
        }

        //cho mượn sách
        public ActionResult MuonSach(string mathe,string[] lstmasach,int[] lstthoigianmuon)
        {
            //thêm dữ liệu vào bảng muontra và bảng chi tiết mượn trả
            QLTHUVIEN db = new QLTHUVIEN();
            try
            {
                THETHUVIEN the = db.THETHUVIENs.Single(x => x.MATHE == mathe);
                if (the != null)
                    for (int i = 0; i < lstmasach.Length; i++)
                    {
                        SACH sach = db.SACHes.SingleOrDefault(x => x.MASACH == lstmasach[i]);
                        if (sach != null)
                        {
                            MUONTRASACH muontra = new MUONTRASACH();
                            muontra.MASACH = lstmasach[i];
                            muontra.MATHE = mathe;
                            db.MUONTRASACHes.Add(muontra);
                            db.SaveChanges();
                            //lay lại id muontrasach
                            int id = db.MUONTRASACHes.Single(x => x.MASACH == muontra.MASACH && x.MATHE == muontra.MATHE).ID;
                            //dùng id này để thêm vào bảng chi tiết
                            CHITIETMUONTRASACH chitiet = new CHITIETMUONTRASACH();
                            chitiet.MAMUONTRASACH = id;
                            chitiet.NGAYMUON = DateTime.UtcNow;
                            chitiet.THOIGIANMUON = lstthoigianmuon[i];
                            db.CHITIETMUONTRASACHes.Add(chitiet);
                            db.SaveChanges();


                        }
                    }
                ViewBag.Success = "Lưu hoàn tất !!!";
            }
            catch (Exception)
            {
                ViewBag.Error = "Lỗi dữ liệu";
                return View();
            }




            return View();
        }

        public ActionResult TraSach(string[] masach)
        {
            //xóa  dữ liệu vào bảng muontra và bảng chi tiết mượn trả
            QLTHUVIEN db = new QLTHUVIEN();
            for (int i = 0; i < masach.Length; i++)
            {
                CHITIETMUONTRASACH chitiet = db.CHITIETMUONTRASACHes.SingleOrDefault(x => x.MUONTRASACH.MASACH == masach[i]);
                if (chitiet!=null)
                {
                    //xóa chitiet
                    int? idmuontra = chitiet.MAMUONTRASACH;
                    db.CHITIETMUONTRASACHes.Remove(chitiet);
                    //xoa muon tra
                    
                    MUONTRASACH muontra = db.MUONTRASACHes.SingleOrDefault(x => x.ID == idmuontra);
                    if (muontra!=null)
                    {
                        db.MUONTRASACHes.Remove(muontra);
                    }
                }

            }
            return View();
        }

        //lấy thông tin bạn đọc mượn sách của thư viện dựa vào mã thẻ bạn đọc


        //kiểm tra sách quá hạn


        //In thông tin khách quá hạn


        

        #endregion



    }
}