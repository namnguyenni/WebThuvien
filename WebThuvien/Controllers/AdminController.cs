using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebThuvien.Models.Entity;
using System.IO;
using WebThuvien.Models.Function;
using System.Data.SqlClient;
using System.Data;

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

            ViewBag.Sach = Sach;
            return View();
        }

        //xem chi tiết sách
        public ActionResult XemChiTietSach(string MaSach)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            SACH sach = db.SACHes.Single(x => x.MASACH == MaSach);
            ViewBag.Sach = sach;
            ViewBag.Loaisach = db.LOAISACHes.ToList();
            ViewBag.Linhvuc = db.LINHVUCs.ToList();
            ViewBag.NXB = db.NHAXUATBANs.ToList();
            ViewBag.Tacgia = db.TACGIAs.ToList();

            return View();
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
            QLTHUVIEN db = new QLTHUVIEN();
            ViewBag.Loaisach = db.LOAISACHes.ToList();
            ViewBag.Linhvuc = db.LINHVUCs.ToList();
            ViewBag.NXB = db.NHAXUATBANs.ToList();
            ViewBag.Tacgia = db.TACGIAs.ToList();
            return View();
        }
        //hàm xử lí thêm sách vào thư viện
        [HttpPost]
        public ActionResult UploadSach(SACH sach,HttpPostedFileBase filepdf=null,HttpPostedFileBase hinhanh=null,string TENTACGIA="CHƯA XÁC ĐỊNH")
        {
            QLTHUVIEN db = new QLTHUVIEN();
            try
            {
               
                if (sach.MASACH.Trim() != "")
                {
                    //ma sach
                    string masach = "";
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
                            numberChars[i] = numberChars[random.Next(charsnumber.Length)];
                        }

                        //hai chu dau tien
                        string str1 = new String(stringChars);
                        string str2 = new String(numberChars);

                        masach = str1 + str2;
                        sach.MASACH = masach;

                    }
                    while (db.SACHes.SingleOrDefault(x => x.MASACH == masach) != null);
                }
                new QRCode().GenerateQRCode(sach.MASACH);


                if (filepdf != null)
                {
                    string fileExtend = System.IO.Path.GetExtension(filepdf.FileName);


                    string targetFolder = Server.MapPath("~/Content/ClientContent/FILE_PDF/");
                    string targetPath = Path.Combine(targetFolder, sach.MASACH + fileExtend);
                    filepdf.SaveAs(targetPath);
                    sach.FILEPATH = sach.MASACH + fileExtend;

                }

                if (hinhanh != null)
                {
                    string fileExtend = System.IO.Path.GetExtension(hinhanh.FileName);


                    string targetFolder = Server.MapPath("~/Content/ClientContent/images/Books/");
                    string targetPath = Path.Combine(targetFolder, sach.MASACH + fileExtend);
                    hinhanh.SaveAs(targetPath);
                    sach.HINHANH = sach.MASACH + fileExtend;
                }

                if (db.TACGIAs.SingleOrDefault(x=>x.TENTACGIA == TENTACGIA.ToUpper()) == null)
                {
                    //THÊM MỚI TÁC GIẢ
                    TACGIA newTACGIA = new TACGIA();
                    newTACGIA.TENTACGIA = TENTACGIA.ToUpper();

                    string matacgia;
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
                            numberChars[i] = charsnumber[random.Next(charsnumber.Length)];
                        }

                        //hai chu dau tien
                        string str1 = new String(stringChars);
                        string str2 = new String(numberChars);

                        matacgia = str1 + str2;


                    }


                    while (db.TACGIAs.SingleOrDefault(x => x.MATACGIA == matacgia) != null);


                    newTACGIA.MATACGIA = matacgia;

                    db.TACGIAs.Add(newTACGIA);
                    sach.MATACGIA = matacgia;
                }
                else
                {
                    sach.MATACGIA = db.TACGIAs.SingleOrDefault(x => x.TENTACGIA == TENTACGIA.ToUpper()).MATACGIA;
                }


                db.SACHes.Add(sach);
                db.SaveChanges();
            }
            catch (Exception)
            {
                ViewBag.Error = "Không thể thêm sách do thiếu thông tin hoặc đã tồn tại sách";
                return Redirect("/Admin");
            }
            return Redirect("/Admin/XemchitietSach?MaSach="+sach.MASACH);
        }

        //hàm sửa sách
        [HttpPost]
        public ActionResult ChinhsuaSach(SACH sach,HttpPostedFileBase filepdf=null,HttpPostedFileBase hinhanh=null, string TENTACGIA="Chưa xác định")
        {
            QLTHUVIEN db = new QLTHUVIEN();
            SACH sach_old = db.SACHes.Single(x => x.ID == sach.ID);
            try
            {
                
                if (filepdf != null)
                {
                    if (System.IO.File.Exists(Server.MapPath("~/Content/ClientContent/FILE_PDF/" + sach_old.FILEPATH)))
                    {
                        System.IO.File.Delete(Server.MapPath("~/Content/ClientContent/FILE_PDF/" + sach_old.FILEPATH));

                    }
                    string fileExtend = System.IO.Path.GetExtension(filepdf.FileName);


                    string targetFolder = Server.MapPath("~/Content/ClientContent/FILE_PDF/");
                    string targetPath = Path.Combine(targetFolder, sach.MASACH + fileExtend);
                    filepdf.SaveAs(targetPath);
                    sach.FILEPATH = sach.MASACH + fileExtend;

                }

                if (hinhanh != null)
                {
                    if (System.IO.File.Exists(Server.MapPath("~/Content/ClientContent/images/Books/" + sach_old.HINHANH)))
                    {
                        System.IO.File.Delete(Server.MapPath("~/Content/ClientContent/images/Books/" + sach_old.HINHANH));

                    }
                    string fileExtend = System.IO.Path.GetExtension(hinhanh.FileName);


                    string targetFolder = Server.MapPath("~/Content/ClientContent/images/Books/");
                    string targetPath = Path.Combine(targetFolder, sach.MASACH + fileExtend);
                    hinhanh.SaveAs(targetPath);
                    sach.HINHANH = sach.MASACH + fileExtend;
                }

                if (TENTACGIA.ToUpper() != sach_old.TACGIA.TENTACGIA)
                {
                    //THÊM MỚI TÁC GIẢ
                    TACGIA newTACGIA = new TACGIA();
                    newTACGIA.TENTACGIA = TENTACGIA.ToUpper(); ;

                    string matacgia;
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
                        string str1 =  new String(stringChars);
                        string str2 = new String(numberChars);

                        matacgia = str1 + str2;
                        

                    }
                    while (db.TACGIAs.SingleOrDefault(x => x.MATACGIA == matacgia) != null);

                    newTACGIA.MATACGIA = matacgia;

                    db.TACGIAs.Add(newTACGIA);
                    sach.MATACGIA = matacgia;
                }

                sach_old.TENSACH = sach.TENSACH;
                sach_old.SOTRANG = sach.SOTRANG;
                sach_old.FILEPATH = sach.FILEPATH;
                sach_old.HINHANH = sach.HINHANH;
                sach_old.LUOTXEM = sach.LUOTXEM;
                sach_old.TRANGTHAI = sach.TRANGTHAI;
                sach_old.NGONNGU = sach.NGONNGU;
                sach_old.MALOAISACH = sach.MALOAISACH;
                sach_old.MALINHVUC = sach.MALINHVUC;
                sach_old.MATACGIA = sach.MATACGIA;
                sach_old.MANHAXUATBAN = sach.MANHAXUATBAN;
                sach_old.NGAYTAILEN = sach.NGAYTAILEN;
                sach_old.NAMXUATBAN = sach.NAMXUATBAN;
                sach_old.GHICHU = sach.GHICHU;


                db.SaveChanges();

                return Redirect("/Admin/XemchitietSach?MaSach=" + sach_old.MASACH);

            }
            catch (Exception)
            {

            }
            return Redirect("/Admin/XemchitietSach?MaSach=" + sach_old.MASACH);

        }

        public ActionResult Timkiemsach(string noidungnhap = "", string linhvuc = "", string loaisach = "", string tacgia="")
        {

            string noidung = noidungnhap.ToUpper();
            QLTHUVIEN db = new QLTHUVIEN();
            List<SACH> SearchSach = new List<SACH>();

            //tim kiem gần đúng trên tên sách
            List<SACH> SearchSachName = db.SACHes.Where(x => x.TENSACH.Contains(noidung) == true).ToList();
            SearchSach.AddRange(SearchSachName);

            //tim kiem gần đúng mã sách
            List<SACH> SearchSachMa = db.SACHes.Where(x => x.MASACH.Contains(noidung) == true).ToList();
            AddSach(SearchSach, SearchSachMa);

            //tìm kiếm vào tên tác giả
            SqlParameter noidungparam = new SqlParameter("@noidung", noidung);
            noidungparam.SqlDbType = SqlDbType.NVarChar;
            List<SACH> SearchSachTacgia = db.Database.SqlQuery<SACH>("exec dbo.SearchSachTacgia '" + noidung + "'").ToList();
            AddSach(SearchSach, SearchSachTacgia);

            //tim kiếm theo lĩnh vực
            List<LINHVUC> Linhvulienquan = db.LINHVUCs.Where(x => x.TENLINHVUC.Contains(noidung) == true).ToList();

            foreach (var item in Linhvulienquan)
            {
                List<SACH> sach = db.SACHes.Where(x => x.MALINHVUC == item.MALINHVUC).ToList();//tìm kiếm sách có lĩnh vực
                AddSach(SearchSach, sach);//thêm vào sách search
            }

            //lọc theo thể loại và lĩnh vực
            List<SACH> LIST = new List<SACH>();
            if (linhvuc != "Tất cả" || loaisach != "Tất cả" || tacgia != "Tất cả")
            {
                foreach (var item in SearchSach)
                {
                    if (item.LINHVUC.TENLINHVUC.Contains(linhvuc.ToUpper()) || item.LOAISACH.TENTHELOAI.Contains(loaisach.ToUpper()) || item.TACGIA.TENTACGIA.Contains(tacgia.ToUpper()))
                    {
                        LIST.Add(item);
                    }
                }
                SearchSach = LIST;

            }

            ViewBag.SearchSach = SearchSach;
            
            ViewBag.noidungnhap = noidungnhap;
            ViewBag.linhvuc = linhvuc;
            ViewBag.loaisach = loaisach;
            ViewBag.tacgia = tacgia;
            return Redirect("/Admin/Sach");
        }

        private void AddSach(List<SACH> list1, List<SACH> list2)
        {

            foreach (var item2 in list2)
            {
                bool exist = false;
                foreach (var item1 in list1)
                {
                    if (item2.MASACH == item1.MASACH)
                    {
                        exist = true;
                        break;
                    }
                }
                if (!exist)
                {
                    list1.Add(item2);
                }
            }
        }

        //hàm xóa cuốn sách ra khỏi thư viện
        public ActionResult Xoasach(string MaSach)
        {
            try
            {
                QLTHUVIEN db = new QLTHUVIEN();
                SACH sach = db.SACHes.Single(x => x.MASACH == MaSach);
                db.SACHes.Remove(sach);
                db.SaveChanges();
                TempData["Error"] = "0";
            }
            catch (Exception)
            {
                TempData["Error"] = "1";
            }
            return Redirect("/Admin/Sach");
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
        [HttpPost]
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

        #region danh sách partial

        public ActionResult Thongketongquan()
        {
            return View();
        }

        #endregion



    }
}