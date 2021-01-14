namespace WebThuvien.Models.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BAIDANGTHONGTIN")]
    public partial class BAIDANGTHONGTIN
    {
        [Key]
        public int MABAIDANG { get; set; }

        [StringLength(50)]
        public string TIEUDE { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NGAYDANG { get; set; }

        [StringLength(20)]
        public string TACGIA { get; set; }

        [StringLength(3000)]
        public string NOIDUNG { get; set; }

        [StringLength(50)]
        public string HINHANH1 { get; set; }

        [StringLength(50)]
        public string HINHANH2 { get; set; }

        public virtual TAIKHOAN TAIKHOAN { get; set; }
    }
}
