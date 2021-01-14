namespace WebThuvien.Models.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SACH")]
    public partial class SACH
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SACH()
        {
            BINHLUANs = new HashSet<BINHLUAN>();
            MUONTRASACHes = new HashSet<MUONTRASACH>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Key]
        [StringLength(20)]
        public string MASACH { get; set; }

        [StringLength(100)]
        public string TENSACH { get; set; }

        public int? SOTRANG { get; set; }

        public string FILEPATH { get; set; }

        public string HINHANH { get; set; }

        public int? LUOTXEM { get; set; }

        public int? TRANGTHAI { get; set; }

        [StringLength(100)]
        public string NGONNGU { get; set; }

        [StringLength(20)]
        public string MALOAISACH { get; set; }

        [StringLength(20)]
        public string MALINHVUC { get; set; }

        [StringLength(20)]
        public string MATACGIA { get; set; }

        [StringLength(20)]
        public string MANHAXUATBAN { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NGAYTAILEN { get; set; }

        [StringLength(1000)]
        public string GHICHU { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BINHLUAN> BINHLUANs { get; set; }

        public virtual LINHVUC LINHVUC { get; set; }

        public virtual LOAISACH LOAISACH { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MUONTRASACH> MUONTRASACHes { get; set; }

        public virtual NHAXUATBAN NHAXUATBAN { get; set; }

        public virtual TACGIA TACGIA { get; set; }
    }
}
