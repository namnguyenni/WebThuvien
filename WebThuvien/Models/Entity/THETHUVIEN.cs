namespace WebThuvien.Models.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("THETHUVIEN")]
    public partial class THETHUVIEN
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public THETHUVIEN()
        {
            DOCGIAs = new HashSet<DOCGIA>();
            MUONTRASACHes = new HashSet<MUONTRASACH>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Key]
        [StringLength(20)]
        public string MATHE { get; set; }

        [StringLength(100)]
        public string TENCHUTHE { get; set; }

        public DateTime? NGAYSINH { get; set; }

        [StringLength(100)]
        public string DIACHI { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NGAYCAP { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NGAYHETHAN { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOCGIA> DOCGIAs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MUONTRASACH> MUONTRASACHes { get; set; }
    }
}
