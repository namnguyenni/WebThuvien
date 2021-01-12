namespace WebThuvien.Models.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NHANVIEN")]
    public partial class NHANVIEN
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Key]
        [StringLength(20)]
        public string MANHANVIEN { get; set; }

        [StringLength(100)]
        public string TENNHANVIEN { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NGAYSINH { get; set; }

        [StringLength(10)]
        public string SODIENTHOAI { get; set; }

        [StringLength(20)]
        public string MATAIKHOAN { get; set; }

        public virtual TAIKHOAN TAIKHOAN { get; set; }
    }
}
