using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebThuvien.Models.Entity;

namespace WebThuvien.Controllers
{
    public class SachController : Controller
    {
        // GET: Sach
        public ActionResult Index(int pageNumber=1)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            List<SACH> SearchSach = db.SACHes.ToList();

            //phân trang và tính toán
            if (SearchSach.Count < 9)
            {
                ViewBag.SearchSach = SearchSach;
            }
            else if (SearchSach.Count - (pageNumber - 1) * 9 > 0 && SearchSach.Count - (pageNumber - 1) * 9 < 9)
            {
                ViewBag.SearchSach = SearchSach.GetRange((pageNumber - 1) * 9, SearchSach.Count - (pageNumber - 1) * 9);

            }
            else
            {
                ViewBag.SearchSach = SearchSach.GetRange((pageNumber - 1) * 9, 9);
            }
            //số lượng trang
            int countPage = SearchSach.Count() / 9;
            if (countPage * 9 < SearchSach.Count())
            {
                countPage += 1;
            }

            ViewBag.countPage = countPage;
            ViewBag.currentPage = pageNumber;
            return View();
        }


        public ActionResult Chitietsach(string MaSach)
        {
            QLTHUVIEN db = new QLTHUVIEN();

            
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

            
            return View();
        }


        public ActionResult Timkiemsach(string noidungnhap="",string linhvuc="",string loaisach="",int pageNumber=1)
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
            List<SACH> SearchSachTacgia = db.Database.SqlQuery<SACH>("exec dbo.SearchSachTacgia '"+ noidung+"'").ToList();
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
            if (linhvuc != "0" || loaisach != "0")
            {
                foreach (var item in SearchSach)
                {
                    if (item.MALINHVUC == linhvuc || item.MALOAISACH == loaisach)
                    {
                        LIST.Add(item);
                    }
                }
                SearchSach = LIST;

            }

            //phân trang và tính toán
            if (SearchSach.Count<9)
            {
                ViewBag.SearchSach = SearchSach;
            }
            else if(SearchSach.Count - (pageNumber - 1) * 9 > 0 && SearchSach.Count - (pageNumber - 1) * 9 < 9)
            {
                ViewBag.SearchSach = SearchSach.GetRange((pageNumber - 1) * 9, SearchSach.Count - (pageNumber-1) * 9);

            }
            else
            {
                ViewBag.SearchSach = SearchSach.GetRange((pageNumber - 1)*9 , 9);
            }

            //số lượng trang
            int countPage = SearchSach.Count()/9;
            if (countPage*9 < SearchSach.Count())
            {
                countPage += 1;
            }
            
            ViewBag.countPage = countPage;
            ViewBag.currentPage = pageNumber;
            ViewBag.noidungnhap = noidungnhap;
            ViewBag.linhvuc = linhvuc;
            ViewBag.loaisach = loaisach;

            return View();
        }


        private void AddSach(List<SACH> list1,List<SACH> list2)
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

    }
}