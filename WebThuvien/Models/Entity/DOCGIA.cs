namespace WebThuvien.Models.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DOCGIA")]
    public partial class DOCGIA
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Key]
        [StringLength(20)]
        public string MADOCGIA { get; set; }

        [StringLength(100)]
        public string TENDOCGIA { get; set; }

        [StringLength(100)]
        public string DIACHI { get; set; }

        [StringLength(20)]
        public string MATAIKHOAN { get; set; }

        [StringLength(20)]
        public string MASOTHE { get; set; }

        public virtual THETHUVIEN THETHUVIEN { get; set; }

        public virtual TAIKHOAN TAIKHOAN { get; set; }
    }
}
