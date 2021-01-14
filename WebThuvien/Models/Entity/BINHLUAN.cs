namespace WebThuvien.Models.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BINHLUAN")]
    public partial class BINHLUAN
    {
        [Key]
        public int MABINHLUAN { get; set; }

        [StringLength(20)]
        public string MASACH { get; set; }

        [StringLength(20)]
        public string MANGUOIBINHLUAN { get; set; }

        [StringLength(1000)]
        public string NOIDUNG { get; set; }

        public int MAREPBINHLUAN { get; set; }

        [Column(TypeName = "date")]
        public DateTime? THOIGIAN { get; set; }

        public virtual TAIKHOAN TAIKHOAN { get; set; }

        public virtual SACH SACH { get; set; }
    }
}
