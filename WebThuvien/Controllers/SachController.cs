using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebThuvien.Models.Entity;

namespace WebThuvien.Controllers
{
    public class SachController : Controller
    {
        // GET: Sach
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Chitietsach(string MaSach)
        {
            QLTHUVIEN db = new QLTHUVIEN();

            try
            {
                //lấy cuốn sách
                SACH sach = db.SACHes.Single(x => x.MASACH == MaSach);
                ViewBag.sach = sach;

                //lấy loại sách
                LOAISACH loaisach = db.LOAISACHes.Single(x => x.MATHELOAI == sach.MALOAISACH);
                ViewBag.loaisach = loaisach;

                //lấy lĩnh vực sách
                LINHVUC linhvuc = db.LINHVUCs.Single(x => x.MALINHVUC == sach.MALINHVUC);
                ViewBag.linhvuc = linhvuc;

                //tác giả sách
                TACGIA tacgia = db.TACGIAs.Single(x => x.MATACGIA == sach.MATACGIA);
                ViewBag.tacgia = tacgia;

                //nha xuat ban
                NHAXUATBAN nxb = db.NHAXUATBANs.Single(x => x.MANXB == sach.MANHAXUATBAN);
                ViewBag.nxb = nxb;

                //những cuốn sách cùng loại
                List<SACH> SachLienQuan_Loai = db.SACHes.Where(x => x.MALOAISACH == sach.MALOAISACH).Take(3).ToList();
                ViewBag.SachLienQuanCungLoai = SachLienQuan_Loai;

                //những cuốn sách liên quan về lĩnh vực
                List<SACH> SachLienQuan_LinhVuc = db.SACHes.Where(x => x.MALINHVUC == sach.MALINHVUC).Take(3).ToList();
                ViewBag.SachLienQuan_LinhVuc = SachLienQuan_LinhVuc;

                //những sách cùng tác giả
                List<SACH> SachLienQuan_Tacgia = db.SACHes.Where(x => x.MATACGIA == sach.MATACGIA).Take(3).ToList();
                ViewBag.SachLienQuan_Tacgia = SachLienQuan_Tacgia;

                //bình luận về cuốn sách này
                List<BINHLUAN> binhluan = db.BINHLUANs.Where(x => x.MASACH == MaSach).ToList();
                ViewBag.binhluan = binhluan;

            }
            catch (Exception)
            {

                return Redirect("/Home");
            }
            return View();
        }

    }
}