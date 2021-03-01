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
using WebThuvien.Models.CustomClass;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace WebThuvien.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        //giao diện trang chủ admin
        public ActionResult Index()
        {
            //kiểm tra sự tồn tại session
            if (Session["Taikhoan"] == null)
            {
                return Redirect("/Home/Login");
            }

            //
            QLTHUVIEN db = new QLTHUVIEN();
            List<SACH> lstSach = db.Database.SqlQuery<SACH>("exec dbo.TOPPOPULARBOOK ").ToList();
            List<SACH> lst = new List<SACH>();
            foreach (var item in lstSach)
            {
                SACH sach = db.SACHes.SingleOrDefault(x => x.ID == item.ID);
                lst.Add(sach);
            }
            ViewBag.Sach = lst;
            ViewBag.Soluongsach = db.SACHes.Count();
            ViewBag.Soluongbandoc = db.TAIKHOANs.Where(x=>x.LOAITAIKHOAN == 1).Count();
            int Soluongbaidangthang = 0;
            foreach (var item in db.BAIDANGTHONGTINs)
            {
                if (item.NGAYDANG != null)
                {
                    DateTime ngaydang = (DateTime)item.NGAYDANG;
                    if (ngaydang.Month == DateTime.Now.Month)
                    {
                        Soluongbaidangthang++;
                    }
                }
                

            }
            ViewBag.Soluongbaidangthang = Soluongbaidangthang;
            int Soluongquahan = 0;
            foreach (var item in db.CHITIETMUONTRASACHes)
            {
                if ((DateTime.Now -  item.NGAYMUON.Value).TotalDays > item.THOIGIANMUON && item.NGAYTRA ==  null)
                {
                    Soluongquahan++;
                }
            }

            ViewBag.Soluongquahan = Soluongquahan;

            //Thống kê mượn trả sách trong năm nay
            int nowyear = DateTime.Now.Year;
            ViewBag.nowyear = nowyear;
            List<int> luotmuon = new List<int>();
            List<int> luottra = new List<int>();
            for (int i = 1; i < 13; i++)
            {
                int muon = 0, tra = 0;
                muon = db.CHITIETMUONTRASACHes.Where(x => x.NGAYMUON.Value.Year == nowyear && x.NGAYMUON.Value.Month == i).Count();
                tra = db.CHITIETMUONTRASACHes.Where(x =>x.NGAYTRA!=null && x.NGAYTRA.Value.Year == nowyear && x.NGAYTRA.Value.Month == i).Count();
                luotmuon.Add(muon);
                luottra.Add(tra);

            }
            ViewBag.luotmuon = luotmuon;
            ViewBag.luottra = luottra;
            //đăng nhập gần nhất
            var Taikhoangannhat = db.TAIKHOANs.Where(x=>x.DANGNHAPGANNHAT!=null).OrderBy(x=>x.DANGNHAPGANNHAT).Take(5).ToList();
            ViewBag.Taikhoangannhat = Taikhoangannhat;

            return View();
        }

        [HttpPost]
        public JsonResult ChartMuontra(int year)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            List<int> luotmuon = new List<int>();
            List<int> luottra = new List<int>();
            for (int i = 1; i < 13; i++)
            {
                int muon = 0, tra = 0;
                muon = db.CHITIETMUONTRASACHes.Where(x => x.NGAYMUON.Value.Year == year && x.NGAYMUON.Value.Month == i).Count();
                tra = db.CHITIETMUONTRASACHes.Where(x => x.NGAYTRA != null && x.NGAYTRA.Value.Year == year && x.NGAYTRA.Value.Month == i).Count();
                luotmuon.Add(muon);
                luottra.Add(tra);

            }

            return Json(new { luotmuon = luotmuon, luottra = luottra });
        }

        //TẢI TÀI LIỆU ĐÃ SCAN LÊN CHO VÀO TRONG THƯ MỤC Ở SERVER
        public ActionResult Scan()
        {

            return View();
        }

        public ActionResult Sach(int pageNumber=1)
        {
            //kiểm tra sự tồn tại session
            if (Session["Taikhoan"] == null)
            {
                return Redirect("/Home/Login");
            }

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
            //kiểm tra sự tồn tại session
            if (Session["Taikhoan"] == null)
            {
                return Redirect("/Home/Login");
            }

            QLTHUVIEN db = new QLTHUVIEN();
            SACH sach = db.SACHes.SingleOrDefault(x => x.MASACH == MaSach);
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

        [HttpPost]
        public ActionResult Login(string tentaikhoan,string matkhau,bool luumk=false)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            TAIKHOAN taikhoan = db.TAIKHOANs.SingleOrDefault(x => x.TENTAIKHOAN == tentaikhoan);
            if (taikhoan != null && taikhoan.MATKHAU == matkhau)
            {
                taikhoan.DANGNHAPGANNHAT = DateTime.Now;
                db.SaveChanges();
                //tạo session
                if (taikhoan.LOAITAIKHOAN == 1)
                {
                    Session["TaikhoanBanDoc"] = taikhoan;
                    return Redirect("/Home");
                }
                else
                {
                    Session["Taikhoan"] = taikhoan;
                    Session["TaikhoanBanDoc"] = taikhoan;
                    return Redirect("/Admin");
                }

                //if (luumk)
                //{
                //    HttpCookie taikhoan_ck = new HttpCookie("tentaikhoan", tentaikhoan);
                //    HttpCookie matkhau_ck = new HttpCookie("matkhau", matkhau);

                //    taikhoan_ck.Expires = DateTime.Now.AddDays(30);
                //    matkhau_ck.Expires = DateTime.Now.AddDays(30);

                //    Response.Cookies.Add(taikhoan_ck);
                //    Response.Cookies.Add(matkhau_ck);
                //}
                
                
            }
            else
            {
                TempData["Thongbao"] = "Thông tin tài khoản hoặc mật khẩu không chính xác!!!";
                return Redirect("/Home/Login");
            }
            
        }
        //giao diện thêm sách, tải sách từ máy lên
        public ActionResult UploadSach()
        {
            //kiểm tra sự tồn tại session
            if (Session["Taikhoan"] == null)
            {
                return Redirect("/Home/Login");
            }

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
                    if (sach.MASACH == "" || sach.MASACH == null)
                    {
                        //ma sach
                        string masach = "";
                        do
                        {
                            masach = Tusinhma(2, 4);
                            sach.MASACH = masach;

                        }
                        while (db.SACHes.SingleOrDefault(x => x.MASACH == masach) != null);
                        new QRCode().GenerateQRCode(sach.MASACH, 1);
                    }
                
                    else
                    {
                        new QRCode().GenerateQRCode(sach.MASACH, 1);
                    }
                


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

                if (db.TACGIAs.SingleOrDefault(x=>x.TENTACGIA.ToUpper() == TENTACGIA.ToUpper()) == null)
                {
                    //THÊM MỚI TÁC GIẢ
                    TACGIA newTACGIA = new TACGIA();
                    newTACGIA.TENTACGIA = TENTACGIA.ToUpper();

                    string matacgia;
                    do
                    {
                        matacgia = Tusinhma(2, 4);
                    }
                    while (db.TACGIAs.SingleOrDefault(x => x.MATACGIA == matacgia) != null);
                    newTACGIA.MATACGIA = matacgia;
                    db.TACGIAs.Add(newTACGIA);
                    sach.MATACGIA = matacgia;
                }
                else
                {
                    sach.MATACGIA = db.TACGIAs.FirstOrDefault(x => x.TENTACGIA.ToUpper() == TENTACGIA.ToUpper()).MATACGIA;
                }

                sach.NGAYTAILEN = DateTime.Now;
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
            SACH sach_old = db.SACHes.SingleOrDefault(x => x.ID == sach.ID);
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
                    string targetPath = Path.Combine(targetFolder, sach_old.MASACH + fileExtend);
                    filepdf.SaveAs(targetPath);
                    sach.FILEPATH = sach_old.MASACH + fileExtend;

                }

                if (hinhanh != null)
                {
                    if (System.IO.File.Exists(Server.MapPath("~/Content/ClientContent/images/Books/" + sach_old.HINHANH)))
                    {
                        System.IO.File.Delete(Server.MapPath("~/Content/ClientContent/images/Books/" + sach_old.HINHANH));

                    }
                    string fileExtend = System.IO.Path.GetExtension(hinhanh.FileName);


                    string targetFolder = Server.MapPath("~/Content/ClientContent/images/Books/");
                    string targetPath = Path.Combine(targetFolder, sach_old.MASACH + fileExtend);
                    hinhanh.SaveAs(targetPath);
                    sach.HINHANH = sach_old.MASACH + fileExtend;
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
            //kiểm tra sự tồn tại session
            if (Session["Taikhoan"] == null)
            {
                return Redirect("/Home/Login");
            }

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
            List<LINHVUC> Linhvulienquan = db.LINHVUCs.Where(x => x.TENLINHVUC.ToUpper().Contains(noidung) == true).ToList();

            foreach (var item in Linhvulienquan)
            {
                List<SACH> sach = db.SACHes.Where(x => x.MALINHVUC == item.MALINHVUC).ToList();//tìm kiếm sách có lĩnh vực
                AddSach(SearchSach, sach);//thêm vào sách search
            }

            //lọc theo thể loại và lĩnh vực
            List<ChitietSachmuon> LIST = new List<ChitietSachmuon>();
            int check = 0;
            if (linhvuc != "")
            {
                check++;
                foreach (var item in SearchSach)
                {
                    if (item.LINHVUC.TENLINHVUC.ToUpper().Contains(linhvuc.ToUpper()))
                    {
                        ChitietSachmuon chitiet = new ChitietSachmuon();
                        chitiet.MASACH = item.MASACH;
                        chitiet.TENSACH = item.TENSACH;
                        chitiet.TENTACGIA = db.TACGIAs.SingleOrDefault(x => x.MATACGIA == item.MATACGIA).TENTACGIA;
                        chitiet.HINHANH = item.HINHANH;
                        LIST.Add(chitiet);
                    }
                }
                ViewBag.SearchSach = LIST;
            }
            if (loaisach != "")
            {
                check++;
                foreach (var item in SearchSach)
                {
                    if (item.LOAISACH.TENTHELOAI.ToUpper().Contains(loaisach.ToUpper()))
                    {
                        ChitietSachmuon chitiet = new ChitietSachmuon();
                        chitiet.MASACH = item.MASACH;
                        chitiet.TENSACH = item.TENSACH;
                        chitiet.TENTACGIA = db.TACGIAs.SingleOrDefault(x => x.MATACGIA == item.MATACGIA).TENTACGIA;
                        chitiet.HINHANH = item.HINHANH;
                        LIST.Add(chitiet);
                    }
                }
                ViewBag.SearchSach = LIST;

            }
            if (tacgia != "")
            {
                check++;
                foreach (var item in SearchSach)
                {
                    if (item.TACGIA.TENTACGIA.ToUpper().Contains(tacgia.ToUpper()))
                    {
                        ChitietSachmuon chitiet = new ChitietSachmuon();
                        chitiet.MASACH = item.MASACH;
                        chitiet.TENSACH = item.TENSACH;
                        chitiet.TENTACGIA = db.TACGIAs.SingleOrDefault(x => x.MATACGIA == item.MATACGIA).TENTACGIA;
                        chitiet.HINHANH = item.HINHANH;
                        LIST.Add(chitiet);
                    }
                }
                ViewBag.SearchSach = LIST;
            }
            
            if (check==0)
            {
                foreach (var item in SearchSach)
                {
                    ChitietSachmuon chitiet = new ChitietSachmuon();
                    chitiet.MASACH = item.MASACH;
                    chitiet.TENSACH = item.TENSACH;
                    chitiet.TENTACGIA = db.TACGIAs.SingleOrDefault(x => x.MATACGIA == item.MATACGIA).TENTACGIA;
                    chitiet.HINHANH = item.HINHANH;
                    LIST.Add(chitiet);
                }
                ViewBag.SearchSach = LIST;


            }

            ViewBag.noidungnhap = noidungnhap;
            ViewBag.linhvuc = linhvuc;
            ViewBag.loaisach = loaisach;
            ViewBag.tacgia = tacgia;
            return View();
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
            //kiểm tra sự tồn tại session
            if (Session["Taikhoan"] == null)
            {
                return Redirect("/Home/Login");
            }

            try
            {
                QLTHUVIEN db = new QLTHUVIEN();
                SACH sach = db.SACHes.SingleOrDefault(x => x.MASACH == MaSach);
                string hinhanh = sach.HINHANH;
                //nếu sách đó đã trả thì xóa bản ghi về sách đó
                //danh sách bản ghi về mượn trả của sách này
                List<MUONTRASACH> lstmuon = db.MUONTRASACHes.Where(x => x.MASACH == MaSach).ToList();
                foreach (var item in lstmuon)
                {
                    CHITIETMUONTRASACH chitiet = db.CHITIETMUONTRASACHes.SingleOrDefault(x => x.MAMUONTRASACH == item.MAMUONTRASACH);
                    if (chitiet.NGAYTRA ==null)
                    {
                        TempData["Error"] = "1";
                        return Redirect("/Admin/Sach");
                    }
                    db.CHITIETMUONTRASACHes.Remove(chitiet);
                    db.MUONTRASACHes.Remove(item);

                    
                }


                if (System.IO.File.Exists(Server.MapPath("~/Content/ClientContent/images/Books/" + sach.HINHANH)))
                {
                    System.IO.File.Delete(Server.MapPath("~/Content/ClientContent/images/Books/" + sach.HINHANH));
                }
                if (System.IO.File.Exists(Server.MapPath("~/Content/Images/Sach/" + sach.MASACH + ".png")))
                {
                    System.IO.File.Delete(Server.MapPath("~/Content/Images/Sach/" + sach.MASACH + ".png"));
                }
                db.SACHes.Remove(sach);

                db.SaveChanges();
                TempData["Error"] = "";
            }
            catch (Exception)
            {
                TempData["Error"] = "1";
            }
            return Redirect("/Admin/Sach");
        }

        #region Xử lí mượn trả sách
        //tìm kiếm tất cả sách đang mượn thư viện
        public ActionResult Danhmucsachmuon()
        {
            //kiểm tra sự tồn tại session
            if (Session["Taikhoan"] == null)
            {
                return Redirect("/Home/Login");
            }

            QLTHUVIEN db = new QLTHUVIEN();
            List<CHITIETMUONTRASACH> lstmuon = db.CHITIETMUONTRASACHes.ToList();
            List<ChitietSachmuon> listsachmuon = new List<ChitietSachmuon>();
            if (lstmuon!=null)
            {
                foreach (var item in lstmuon)
                {
                    ChitietSachmuon sachmuon = new ChitietSachmuon();
                    string masach = db.MUONTRASACHes.SingleOrDefault(x => x.MAMUONTRASACH == item.MAMUONTRASACH).MASACH;
                    string mathe = db.MUONTRASACHes.SingleOrDefault(x => x.MAMUONTRASACH == item.MAMUONTRASACH).MATHE;

                    SACH sach = db.SACHes.SingleOrDefault(x => x.MASACH == masach);
                    THETHUVIEN the = db.THETHUVIENs.SingleOrDefault(x => x.MATHE == mathe);

                    sachmuon.MASACH = sach.MASACH;
                    sachmuon.TENSACH = sach.TENSACH;
                    sachmuon.HINHANH = sach.HINHANH;
                    sachmuon.NGAYMUON = item.NGAYMUON;
                    sachmuon.THOIGIANMUON = item.THOIGIANMUON;
                    sachmuon.NGUOIMUON = the.TENCHUTHE;
                    sachmuon.NGAYTRA = item.NGAYTRA;
                    sachmuon.MATHENGUOIMUON = the.MATHE;

                    listsachmuon.Add(sachmuon);
                }
            }
            ViewBag.listsachmuon = listsachmuon;


            return View();
        }

        //giao diện cho mượn sách
        public ActionResult MuonSach()
        {
            //kiểm tra sự tồn tại session
            if (Session["Taikhoan"] == null)
            {
                return Redirect("/Home/Login");
            }

            return View();
        }


        //cho mượn sách
        [HttpPost]
        public int MuonSach(string mathe,string[] lstmasach,string[] lstthoigianmuon)
        {
            string[] listmasach = lstmasach[0].Split(',');
            string[] listthoigian = lstthoigianmuon[0].Split(',');

            //thêm dữ liệu vào bảng muontra và bảng chi tiết mượn trả
            QLTHUVIEN db = new QLTHUVIEN();
            try
            {
                THETHUVIEN the = db.THETHUVIENs.SingleOrDefault(x => x.MATHE == mathe);
                if (the != null)
                {
                    for (int i = 0; i < listmasach.Length; i++)
                    {
                        string masach = listmasach[i];
                        SACH sach = db.SACHes.SingleOrDefault(x => x.MASACH == masach);

                        if (sach != null)
                        {
                            //kiểm tra sácH đó  mượn hay chưa
                            CHITIETMUONTRASACH sachmuon = db.CHITIETMUONTRASACHes.SingleOrDefault(x => x.MUONTRASACH.MASACH == masach && x.NGAYTRA == null);
                            if (sachmuon != null)
                            {
                                continue;
                            }

                            MUONTRASACH muontra = new MUONTRASACH();
                            muontra.MASACH = listmasach[i];
                            muontra.MATHE = mathe;
                            string mamuontra;
                            do {
                                mamuontra = Tusinhma(2, 4);
                            }
                            while (db.MUONTRASACHes.SingleOrDefault(x=>x.MAMUONTRASACH == mamuontra) !=null);
                            muontra.MAMUONTRASACH = mamuontra;

                            db.MUONTRASACHes.Add(muontra);
                            db.SaveChanges();

                            //dùng id này để thêm vào bảng chi tiết
                            CHITIETMUONTRASACH chitiet = new CHITIETMUONTRASACH();
                            chitiet.MAMUONTRASACH = mamuontra;
                            chitiet.NGAYMUON = DateTime.UtcNow;
                            chitiet.THOIGIANMUON = Convert.ToInt32(listthoigian[i]);
                            db.CHITIETMUONTRASACHes.Add(chitiet);
                            db.SaveChanges();


                        }
                    }
                    ViewBag.Success = "Lưu hoàn tất !!!";
                    return 1;
                }
                else
                {
                    ViewBag.Error = "Thẻ không tồn tại??";
                    return 0;
                }
            }
            catch (Exception)
            {
               ViewBag.Error = "Lỗi dữ liệu";
                return 0;
            }
        }


        [HttpPost]
        public JsonResult GetTheFrMathe(string mathe)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            THETHUVIEN thethuvien = db.THETHUVIENs.SingleOrDefault(x => x.MATHE == mathe);
            if (thethuvien != null)
            {
                return Json(new { mathe = thethuvien.MATHE, hoten = thethuvien.TENCHUTHE });

            }
            else return Json(new {loi = "1" });
        }



        [HttpPost]
        public List<ChitietSachmuon> GetSachMuonFromMaThe(string mathe)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            List<ChitietSachmuon>  lstSachMuon = db.Database.SqlQuery<ChitietSachmuon>("exec dbo.GetSachMuonFromMaThe '"+mathe+"'").ToList();
            return lstSachMuon;
        }


        [HttpPost]
        public JsonResult GetSachFrMasach(string masach)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            SACH sach = db.SACHes.SingleOrDefault(x => x.MASACH == masach);
            if (sach != null)
            {

                //kiểm tra sác đó  mượn hay chưa
                CHITIETMUONTRASACH sachmuon = db.CHITIETMUONTRASACHes.SingleOrDefault(x => x.MUONTRASACH.MASACH == masach && x.NGAYTRA == null);
                if (sachmuon != null)
                {
                    return Json(new { loi = "2" });//lỗi loại 1
                }
                
                    return Json(new{
                    masach = sach.MASACH,
                    tensach = sach.TENSACH,
                    hinhanh = sach.HINHANH});
            }
            else return Json(new { loi = "1" });//lỗi loại 1
        }


        [HttpPost]
        public JsonResult GetSachTraFrMasach(string masach)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            SACH sach = db.SACHes.SingleOrDefault(x => x.MASACH == masach);
            //kiểm tra sách này có tồn tại trong csdl hay không
            if (sach != null)
            {
                try
                {
                    //nếu tồn tại kiểm tra sách này đang mượn hay không
                    CHITIETMUONTRASACH sachmuon = db.CHITIETMUONTRASACHes.SingleOrDefault(x => x.MUONTRASACH.MASACH == masach && x.NGAYTRA == null);
                    if (sachmuon != null)
                    {
                        //nếu sách này đang mượn thì trả về thông tin sách đang mượn này
                        string mathe = db.MUONTRASACHes.SingleOrDefault(x=>x.MAMUONTRASACH == sachmuon.MAMUONTRASACH).MATHE;
                        string tenchuthe = db.THETHUVIENs.SingleOrDefault(x => x.MATHE == mathe).TENCHUTHE;
                        DateTime ngaymuondate = Convert.ToDateTime(sachmuon.NGAYMUON);
                        string ngaymuon = ngaymuondate.Day + "/" + ngaymuondate.Month + "/" + ngaymuondate.Year;
                        return Json(new { masach = sach.MASACH, tensach = sach.TENSACH, hinhanh = sach.HINHANH,nguoimuon = tenchuthe,ngaymuon = ngaymuon });
                    }
                    else
                    {
                        return Json(new { loi = "2" });//sách này hiện không mượn
                    }
                }
                catch (Exception)
                {

                    return Json(new { loi = "1" });//lỗi loại 1
                }



            }
            else return Json(new { loi = "1" });//lỗi loại 1
        }

        [HttpGet]
        public ActionResult TraSach()
        {
            //kiểm tra sự tồn tại session
            if (Session["Taikhoan"] == null)
            {
                return Redirect("/Home/Login");
            }
            return View();
        }

        [HttpPost]
        public string TraSach(string[] masach)
        {
            string[] listmasach = masach[0].Split(',');
            QLTHUVIEN db = new QLTHUVIEN();
            for (int i = 0; i < listmasach.Length; i++)
            {
                string masach1 = listmasach[i];
                try
                {
                    CHITIETMUONTRASACH chitiet = db.CHITIETMUONTRASACHes.SingleOrDefault(x => x.MUONTRASACH.MASACH == masach1 && x.NGAYTRA==null);
                    if (chitiet != null)
                    {
                        //LẤY NGÀY TRẢ
                        DateTime date = DateTime.Now;
                        chitiet.NGAYTRA = date;
                        db.SaveChanges();
                    }
                }
                catch (Exception)
                {

                    return "0";
                }

            }
            return "1";
        }

        #endregion

        #region danh sách partial

        public ActionResult DSTacgia()
        {
            //kiểm tra sự tồn tại session
            if (Session["Taikhoan"] == null)
            {
                return Redirect("/Home/Login");
            }

            QLTHUVIEN db = new QLTHUVIEN();
            List<TACGIA> tacgias = db.TACGIAs.ToList();
            ViewBag.tacgias = tacgias;


            return View();
        }




        public ActionResult DSLinhvuc()
        {
            //kiểm tra sự tồn tại session
            if (Session["Taikhoan"] == null)
            {
                return Redirect("/Home/Login");
            }
            QLTHUVIEN db = new QLTHUVIEN();
            List<LINHVUC> linhvucs = db.LINHVUCs.ToList();
            ViewBag.linhvucs = linhvucs;
            return View();
        }

        public ActionResult DSLoaisach()
        {
            //kiểm tra sự tồn tại session
            if (Session["Taikhoan"] == null)
            {
                return Redirect("/Home/Login");
            }
            QLTHUVIEN db = new QLTHUVIEN();
            List<LOAISACH> loaisachs = db.LOAISACHes.ToList();
            ViewBag.loaisachs = loaisachs;
            return View();
        }

        public ActionResult DSNXB()
        {
            //kiểm tra sự tồn tại session
            if (Session["Taikhoan"] == null)
            {
                return Redirect("/Home/Login");
            }

            QLTHUVIEN db = new QLTHUVIEN();
            List<NHAXUATBAN> NXB = db.NHAXUATBANs.ToList();
            ViewBag.NXB = NXB;


            return View();
        }



        public string Chinhsualinhvuc(string malinhvuc, string tenlinhvuc,string ghichu)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            LINHVUC linhvuc = db.LINHVUCs.SingleOrDefault(x=>x.MALINHVUC==malinhvuc);

            string ma = malinhvuc;
            if (linhvuc != null)
            {
                linhvuc.TENLINHVUC = tenlinhvuc;
                linhvuc.GHICHU = ghichu;
            }
            else
            {
                LINHVUC linhvuc1 = new LINHVUC();
                //tu sinh ma
                do
                {
                    ma = Tusinhma(2, 4);
                } while (db.LINHVUCs.SingleOrDefault(x => x.MALINHVUC == ma) != null);

                linhvuc1.MALINHVUC = ma;
                linhvuc1.TENLINHVUC = tenlinhvuc;
                linhvuc1.GHICHU = ghichu;
                try
                {
                    db.LINHVUCs.Add(linhvuc1);
                }
                catch (Exception)
                {
                    return "0";
                }
            }
            db.SaveChanges();
            return ma;
        }

        public int Xoalinhvuc(string malinhvuc)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            try
            {
                LINHVUC linhvuc = db.LINHVUCs.SingleOrDefault(x => x.MALINHVUC == malinhvuc);
                if (linhvuc == null)
                {
                    return 0;
                }
                else
                {
                    db.LINHVUCs.Remove(linhvuc);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {

                return 0;
            }


            return 1;
        }

        [HttpPost]
        public string Chinhsuatacgia(string matacgia, string tentacgia,string website,string ghichu)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            TACGIA tacgia = db.TACGIAs.SingleOrDefault(x => x.MATACGIA == matacgia);
            string ma = matacgia;
            if (tacgia != null)
            {
                tacgia.TENTACGIA = tentacgia;
                tacgia.LINKWEBSITE = website;
                tacgia.GHICHU = ghichu;
                

            }
            else
            {
                TACGIA tacgia1 = new TACGIA();
                //tu sinh ma
                do
                {
                    ma = Tusinhma(2, 4);
                } while (db.TACGIAs.SingleOrDefault(x=>x.MATACGIA == ma)!=null);


                tacgia1.MATACGIA = ma;
                tacgia1.TENTACGIA = tentacgia;
                tacgia1.LINKWEBSITE = website;
                tacgia1.GHICHU = ghichu;
                try
                {
                    db.TACGIAs.Add(tacgia1);
                }
                catch (Exception)
                {

                    return "0";
                }



            }
            db.SaveChanges();
            return ma;
        }

        public int Xoatacgia(string matacgia)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            try
            {
                TACGIA tacgia = db.TACGIAs.SingleOrDefault(x => x.MATACGIA == matacgia);
                if (tacgia==null)
                {
                    return 0;
                }
                else
                {
                    db.TACGIAs.Remove(tacgia);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {

                return 0;
            }


            return 1;
        }


        public string Chinhsualoaisach(string tentheloai,string maloaisach,string ghichu)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            LOAISACH loaisach = db.LOAISACHes.SingleOrDefault(x => x.MATHELOAI == maloaisach);
            string ma = maloaisach;
            if (loaisach != null)
            {
                loaisach.TENTHELOAI = tentheloai;
                loaisach.GHICHU = ghichu;
                
            }
            else
            {
                LOAISACH loaisach1 = new LOAISACH();
                //tu sinh ma
                do
                {
                    ma = Tusinhma(2, 4);
                } while (db.LOAISACHes.SingleOrDefault(x => x.MATHELOAI == ma) != null);

                loaisach1.MATHELOAI = ma;
                loaisach1.TENTHELOAI = tentheloai;
                loaisach1.GHICHU = ghichu;
                try
                {
                    db.LOAISACHes.Add(loaisach1);
                }
                catch (Exception)
                {
                    return "0";
                }
            }
            db.SaveChanges();
            return ma;
        }

        public int Xoaloaisach(string maloaisach)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            try
            {
                LOAISACH loaisach = db.LOAISACHes.SingleOrDefault(x => x.MATHELOAI == maloaisach);
                if (loaisach == null)
                {
                    return 0;
                }
                else
                {
                    db.LOAISACHes.Remove(loaisach);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {

                return 0;
            }


            return 1;
        }

        //chỉnh sửa nxb
        public string ChinhsuaNXB(string tennxb, string manxb, string diachi,string email)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            NHAXUATBAN nxb = db.NHAXUATBANs.SingleOrDefault(x => x.MANXB == manxb);
            string ma = manxb;
            if (nxb != null)
            {
                nxb.TENNXB = tennxb;
                nxb.DIACHI = diachi;
                nxb.EMAIL = email;
            }
            else
            {
                NHAXUATBAN nxb1 = new NHAXUATBAN();
                //tu sinh ma
                do
                {
                    ma = Tusinhma(2, 4);
                } while (db.NHAXUATBANs.SingleOrDefault(x => x.MANXB == ma) != null);

                nxb1.MANXB = ma;
                nxb1.TENNXB = tennxb;
                nxb1.DIACHI = diachi;
                nxb1.EMAIL = email;
                try
                {
                    db.NHAXUATBANs.Add(nxb1);
                }
                catch (Exception)
                {
                    return "0";
                }
            }
            db.SaveChanges();
            return ma;
        }

        //xóa nhà xuât bản
        public int XoaNXB(string manxb)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            try
            {
                NHAXUATBAN nxb = db.NHAXUATBANs.SingleOrDefault(x => x.MANXB == manxb);
                if (nxb == null)
                {
                    return 0;
                }
                else
                {
                    db.NHAXUATBANs.Remove(nxb);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {

                return 0;
            }


            return 1;
        }

        //tu sinh mã
        public string Tusinhma(int str,int number)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var charsnumber = "1234567890";

            var stringChars = new char[str];
            var numberChars = new char[number];
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

            string ma = str1 + str2;

            return ma;
        }

        //người dùng và thẻ
        //danh sách người dùng(tài khoản)
        public ActionResult Taikhoan()
        {
            //kiểm tra sự tồn tại session
            if (Session["Taikhoan"] == null)
            {
                return Redirect("/Home/Login");
            }
            QLTHUVIEN db = new QLTHUVIEN();
            ViewBag.taikhoans = db.TAIKHOANs.OrderBy(x=>x.LOAITAIKHOAN).ToList();
            ViewBag.loaitaikhoans = db.LOAITAIKHOANs.ToList();


            return View();
        }


        //chinh sửa người dùng
        public string Chinhsuataikhoan(string Matkhau, string Tendangnhap, string Loaitaikhoan)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            TAIKHOAN taikhoan = db.TAIKHOANs.SingleOrDefault(x => x.TENTAIKHOAN == Tendangnhap);
            string tendangnhap = Tendangnhap;
            if (taikhoan != null)
            {
                taikhoan.MATKHAU = Matkhau;
                taikhoan.LOAITAIKHOAN = Convert.ToInt32(Loaitaikhoan);
            }
            else
            {
                if (tendangnhap.Length < 6 || Matkhau.Length <6)
                {
                    return "0";//tên đăng nhập hoặc mật khẩu không hợp lệ
                }
                else//tạo mới
                {
                    TAIKHOAN taikhoanmoi = new TAIKHOAN();
                    taikhoanmoi.TENTAIKHOAN = Tendangnhap;
                    taikhoanmoi.MATKHAU = Matkhau;
                    taikhoanmoi.LOAITAIKHOAN = Convert.ToInt32(Loaitaikhoan);

                    db.TAIKHOANs.Add(taikhoanmoi);
                }
            }
            db.SaveChanges();
            return "1";
        }
        //xóa người dùng
        public int Xoataikhoan(string Tendangnhap)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            TAIKHOAN taikhoan = db.TAIKHOANs.SingleOrDefault(x => x.TENTAIKHOAN == Tendangnhap);
            if (taikhoan!= null)
            {
                try
                {
                    TAIKHOAN taikhoancurrent = Session["Taikhoan"] as TAIKHOAN;
                    if (taikhoancurrent.TENTAIKHOAN == Tendangnhap)
                    {
                        return 0;
                    }

                    db.TAIKHOANs.Remove(taikhoan);
                    db.SaveChanges();
                }
                catch (Exception)
                {

                    return 0;
                }
            }
            return 1;
        }

        


        //danh sách thẻ bạn đọc
        public ActionResult The()
        {
            //kiểm tra sự tồn tại session
            if (Session["Taikhoan"] == null)
            {
                return Redirect("/Home/Login");
            }

            QLTHUVIEN db = new QLTHUVIEN();
            ViewBag.thethuviens = db.THETHUVIENs.OrderBy(x => x.NGAYCAP).ToList();

            return View();
        }

        //thêm mới thẻ
        [HttpGet]
        public ActionResult ThemThe()
        {
            //kiểm tra sự tồn tại session
            if (Session["Taikhoan"] == null)
            {
                return Redirect("/Home/Login");
            }
            return View();
        }

        public ActionResult XemMauThe(string mathe)
        {
            //kiểm tra có thẻ này không
            QLTHUVIEN db = new QLTHUVIEN();
            THETHUVIEN thethuvien = db.THETHUVIENs.SingleOrDefault(x => x.MATHE == mathe);
            if (thethuvien == null)
            {
                return Redirect("/Admin/The");

            }
            else
            {
            //kiểm tra exist của ảnh mẫu thẻ
                bool check = System.IO.File.Exists(Server.MapPath("~/Content/ClientContent/Thethuvien/" + mathe + ".jpg"));
                if (!check)
                {
                    //tạo mẫu thẻ mới
                    if (!System.IO.File.Exists(Server.MapPath("/Images/The/" + mathe + ".png")))
                    {
                        //tạo mẫu barcode thẻ mới
                        new QRCode().GenerateQRCode(mathe, 2);
                    }

                    string file = Server.MapPath("~/Content/AdminContent/Mauthe/Thethuvien.jpg");
                    string img = Server.MapPath("~/Content/Images/The/" + mathe + ".png");

                    Bitmap bitMapImage = new System.Drawing.Bitmap(file);
                    Graphics graphicImage = Graphics.FromImage(bitMapImage);
                    graphicImage.SmoothingMode = SmoothingMode.AntiAlias;
                    //viết chữ
                    string gt, ns, nc, nhh;

                    if (thethuvien.GIOITINH == 1)
                    {
                        gt = "Nam";
                    }
                    else
                    {
                        gt = "Nữ";
                    }
                    ns = ConvertDate(Convert.ToDateTime(thethuvien.NGAYSINH));
                    nc = ConvertDate(Convert.ToDateTime(thethuvien.NGAYCAP));
                    nhh = ConvertDate(Convert.ToDateTime(thethuvien.NGAYHETHAN));
                    Font font = new Font("Arial", 12, FontStyle.Bold);
                    graphicImage.DrawString(thethuvien.TENCHUTHE.ToUpper(), font, SystemBrushes.WindowText, new Point(300, 158));
                    graphicImage.DrawString(gt, font, SystemBrushes.WindowText, new Point(300, 199));
                    graphicImage.DrawString(ns, font, SystemBrushes.WindowText, new Point(300, 243));

                    graphicImage.DrawString(thethuvien.DIACHI.ToUpper(), font, SystemBrushes.WindowText, new Point(300, 286));
                    graphicImage.DrawString(nc, font, SystemBrushes.WindowText, new Point(300, 328));
                    graphicImage.DrawString(nhh, font, SystemBrushes.WindowText, new Point(300, 371));


                    //thêm ảnh
                    Image barcode = Image.FromFile(img);
                    graphicImage.DrawImage(barcode, new Point(265, 428));

                    //lưu
                    Response.ContentType = "image/jpg";
                    //Save the new image to the response output stream.
                    bitMapImage.Save(Server.MapPath("~/Content/ClientContent/Thethuvien/" + mathe + ".jpg"), ImageFormat.Jpeg);

                    //Clean house.
                    graphicImage.Dispose();
                    bitMapImage.Dispose();

                }

            }
            return Redirect("/Content/ClientContent/Thethuvien/" + mathe + ".jpg");
        }

        [HttpPost]
        public ActionResult ThemThe(THETHUVIEN thethuvien)
        {
            //tự sinh mã thẻ
            string year = DateTime.Now.Year.ToString();
            string number = Tusinhma(0, 3);
            string mathe = "TV" + year +""+ number;
            //lưu thông tin thẻ vào cơ sở dữ liệu
            thethuvien.MATHE = mathe;
            QLTHUVIEN db = new QLTHUVIEN();
            try
            {
                db.THETHUVIENs.Add(thethuvien);
                new QRCode().GenerateQRCode(mathe,2);
                db.SaveChanges();
            }
            catch (Exception)
            {
                TempData["Loi"] = "Lỗi !Không thể thêm thẻ thư viện";
                return Redirect("/Admin/Themthe");
            }
            

            return XemMauThe(mathe);
        }

#region Section Hoạt động
        //Danh sách hoạt động
        public ActionResult DSHoatdong()
        {
            //kiểm tra sự tồn tại session
            if (Session["Taikhoan"] == null)
            {
                return Redirect("/Home/Login");
            }
            QLTHUVIEN db = new QLTHUVIEN();
            ViewBag.hoatdongs = db.BAIDANGTHONGTINs.ToList();


            return View();
        }

        //Chi tiết hoạt động
        public ActionResult Chitiethoatdong(string id)
        {
            //kiểm tra sự tồn tại session
            if (Session["Taikhoan"] == null)
            {
                return Redirect("/Home/Login");
            }
            QLTHUVIEN db = new QLTHUVIEN();
            if (db.BAIDANGTHONGTINs.SingleOrDefault(x => x.MABAIDANG == id)==null)
            {
                return Redirect("/Admin/Themhoatdong");
            }

            ViewBag.baidangs = db.BAIDANGTHONGTINs.SingleOrDefault(x => x.MABAIDANG == id);

            return View();
        }

        [HttpPost]
        public ActionResult Chinhsuahoatdong(BAIDANGTHONGTIN baidang,HttpPostedFileBase hinhanh1=null, HttpPostedFileBase hinhanh2 =null)
        {
            QLTHUVIEN db = new QLTHUVIEN();

            BAIDANGTHONGTIN baidangcu = db.BAIDANGTHONGTINs.SingleOrDefault(x => x.MABAIDANG == baidang.MABAIDANG);
            try
            {
                baidangcu.TIEUDE = baidang.TIEUDE;
                baidangcu.NOIDUNG = baidang.NOIDUNG;
                baidangcu.THOIGIANBATDAU = baidang.THOIGIANBATDAU;
                baidangcu.THOIGIANKETTHUC = baidang.THOIGIANKETTHUC;
                baidangcu.DIADIEM = baidang.DIADIEM;

                if (hinhanh1!=null)
                    {
                        if (System.IO.File.Exists(Server.MapPath("~/Content/ClientContent/Hoatdong/" + baidang.MABAIDANG+"_1.jpg")))
                        {
                            System.IO.File.Delete(Server.MapPath("~/Content/ClientContent/Hoatdong/" + baidang.MABAIDANG + "_1.jpg"));

                        }
                        string targetFolder = Server.MapPath("~/Content/ClientContent/Hoatdong/");
                        string targetPath = Path.Combine(targetFolder, baidang.MABAIDANG + "_1.jpg" );
                        hinhanh1.SaveAs(targetPath);
                        baidangcu.HINHANH1 = baidang.MABAIDANG + "_1.jpg";

                    }
                if (hinhanh2 != null)
                {
                    if (System.IO.File.Exists(Server.MapPath("~/Content/ClientContent/Hoatdong/" + baidang.MABAIDANG + "_2.jpg")))
                    {
                        System.IO.File.Delete(Server.MapPath("~/Content/ClientContent/Hoatdong/" + baidang.MABAIDANG + "_2.jpg"));

                    }
                    string targetFolder = Server.MapPath("~/Content/ClientContent/Hoatdong/");
                    string targetPath = Path.Combine(targetFolder, baidang.MABAIDANG + "_2.jpg");
                    hinhanh2.SaveAs(targetPath);
                    baidangcu.HINHANH2 = baidang.MABAIDANG + "_2.jpg";

                }
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Redirect("/Error");
            }



            return Redirect("/Admin/Chitiethoatdong?id="+baidang.MABAIDANG);
        }

        //xóa hoạt động
        [HttpPost]
        public string Xoahoatdong(string mahoatdong)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            try
            {
                BAIDANGTHONGTIN baidang = db.BAIDANGTHONGTINs.SingleOrDefault(x => x.MABAIDANG == mahoatdong);
                db.BAIDANGTHONGTINs.Remove(baidang);
                db.SaveChanges();
            }
            catch (Exception)
            {

                return "0";
            }

            return "1";
        }

        //Thêm hoạt động
        [HttpGet]
        public ActionResult Themhoatdong()
        {
            //kiểm tra sự tồn tại session
            if (Session["Taikhoan"] == null)
            {
                return Redirect("/Home/Login");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Themhoatdong(BAIDANGTHONGTIN baidang,HttpPostedFileBase hinhanh1, HttpPostedFileBase hinhanh2)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            //kiểm tra sự tồn tại session
            if (Session["Taikhoan"] != null)
            {
                TAIKHOAN tacgia = Session["Taikhoan"] as TAIKHOAN;
                baidang.TACGIA = tacgia.TENTAIKHOAN;
            }
            //SINH MÃ BÀI ĐĂNG
            string mabaidang;
            do
            {
                mabaidang = Tusinhma(2, 4);
            } while (db.BAIDANGTHONGTINs.SingleOrDefault(x=>x.MABAIDANG == mabaidang)!=null);

            baidang.MABAIDANG = mabaidang;

            try
            {
                if (hinhanh1 != null)
                {
                    if (System.IO.File.Exists(Server.MapPath("~/Content/ClientContent/Hoatdong/" + baidang.MABAIDANG + "_1.jpg")))
                    {
                        System.IO.File.Delete(Server.MapPath("~/Content/ClientContent/FILE_PDF/" + baidang.MABAIDANG + "_1.jpg"));

                    }
                    string fileExtend = ".jpg";
                    string targetFolder = Server.MapPath("~/Content/ClientContent/Hoatdong/");
                    string targetPath = Path.Combine(targetFolder, baidang.MABAIDANG + "_1" + fileExtend);
                    hinhanh1.SaveAs(targetPath);
                    baidang.HINHANH1 = baidang.MABAIDANG + "_1.jpg";

                }
                if (hinhanh2 != null)
                {
                    if (System.IO.File.Exists(Server.MapPath("~/Content/ClientContent/Hoatdong/" + baidang.MABAIDANG + "_2.jpg")))
                    {
                        System.IO.File.Delete(Server.MapPath("~/Content/ClientContent/FILE_PDF/" + baidang.MABAIDANG + "_2.jpg"));

                    }
                    string fileExtend = ".jpg";
                    string targetFolder = Server.MapPath("~/Content/ClientContent/Hoatdong/");
                    string targetPath = Path.Combine(targetFolder, baidang.MABAIDANG + "_2" + fileExtend);
                    hinhanh2.SaveAs(targetPath);
                    baidang.HINHANH2 = baidang.MABAIDANG + "_2.jpg";

                }
                baidang.NGAYDANG = DateTime.Now;
                db.BAIDANGTHONGTINs.Add(baidang);
                db.SaveChanges();
            }
            catch(Exception)
            {

            }


            return Redirect("/Admin/Chitiethoatdong?id="+baidang.MABAIDANG);
        }



        #endregion

        public string ConvertDate(DateTime date)
        {
            string day, month;
            if (date.Day < 10)
            {
                day = "0" + date.Day;
            }
            else day = date.Day.ToString();
            if (date.Month < 10)
            {
                month = "0" + date.Month;
            }
            else month = date.Month.ToString();

            return day + "/" + month + "/" + date.Year;
        }

        #endregion



    }
}