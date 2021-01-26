namespace WebThuvien.Models.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CHITIETMUONTRASACH")]
    public partial class CHITIETMUONTRASACH
    {
        public int ID { get; set; }

        [StringLength(20)]
        public string MAMUONTRASACH { get; set; }

        public int? THOIGIANMUON { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NGAYMUON { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NGAYTRA { get; set; }

        [StringLength(100)]
        public string GHICHUMUONTRA { get; set; }

        public virtual MUONTRASACH MUONTRASACH { get; set; }
    }
}
