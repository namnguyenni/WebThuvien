namespace WebThuvien.Models.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MUONTRASACH")]
    public partial class MUONTRASACH
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MUONTRASACH()
        {
            CHITIETMUONTRASACHes = new HashSet<CHITIETMUONTRASACH>();
        }

        public int ID { get; set; }

        [StringLength(20)]
        public string MATHE { get; set; }

        [StringLength(20)]
        public string MASACH { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CHITIETMUONTRASACH> CHITIETMUONTRASACHes { get; set; }

        public virtual SACH SACH { get; set; }

        public virtual THETHUVIEN THETHUVIEN { get; set; }
    }
}
