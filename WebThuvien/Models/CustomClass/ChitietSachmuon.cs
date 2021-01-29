using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebThuvien.Models.CustomClass
{
    public class ChitietSachmuon
    {
        public int ID { get; set; }

        public string MASACH { get; set; }

        public string TENSACH { get; set; }

        public int? SOTRANG { get; set; }

        public string FILEPATH { get; set; }

        public string HINHANH { get; set; }

        public int? LUOTXEM { get; set; }

        public int? TRANGTHAI { get; set; }

        public string NGONNGU { get; set; }

        public string MALOAISACH { get; set; }
        public string TENLOAISACH { get; set; }

        public string MALINHVUC { get; set; }
        public string TENLINHVUC { get; set; }

        public string MATACGIA { get; set; }
        public string TENTACGIA { get; set; }

        public string MANHAXUATBAN { get; set; }
        public string TENNHAXUATBAN { get; set; }


        public DateTime? NGAYTAILEN { get; set; }

        public DateTime? NAMXUATBAN { get; set; }

        public string GHICHU { get; set; }
        public int? THOIGIANMUON { get; set; }
        public DateTime? NGAYMUON { get; set; }
        public string GHICHUMUONTRA { get; set; }
        public string NGUOIMUON { get; set; }
        public string MATHENGUOIMUON { get; set; }




    }
}