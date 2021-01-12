namespace WebThuvien.Models.Entity
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class QLTHUVIEN : DbContext
    {
        public QLTHUVIEN()
            : base("name=QLTHUVIEN")
        {
        }

        public virtual DbSet<CHITIETMUONTRASACH> CHITIETMUONTRASACHes { get; set; }
        public virtual DbSet<DOCGIA> DOCGIAs { get; set; }
        public virtual DbSet<LINHVUC> LINHVUCs { get; set; }
        public virtual DbSet<LOAISACH> LOAISACHes { get; set; }
        public virtual DbSet<LOAITAIKHOAN> LOAITAIKHOANs { get; set; }
        public virtual DbSet<MUONTRASACH> MUONTRASACHes { get; set; }
        public virtual DbSet<NHANVIEN> NHANVIENs { get; set; }
        public virtual DbSet<NHAXUATBAN> NHAXUATBANs { get; set; }
        public virtual DbSet<SACH> SACHes { get; set; }
        public virtual DbSet<TACGIA> TACGIAs { get; set; }
        public virtual DbSet<TAIKHOAN> TAIKHOANs { get; set; }
        public virtual DbSet<THETHUVIEN> THETHUVIENs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DOCGIA>()
                .Property(e => e.MADOCGIA)
                .IsUnicode(false);

            modelBuilder.Entity<DOCGIA>()
                .Property(e => e.MATAIKHOAN)
                .IsUnicode(false);

            modelBuilder.Entity<DOCGIA>()
                .Property(e => e.MASOTHE)
                .IsUnicode(false);

            modelBuilder.Entity<LINHVUC>()
                .Property(e => e.MALINHVUC)
                .IsUnicode(false);

            modelBuilder.Entity<LOAISACH>()
                .Property(e => e.MATHELOAI)
                .IsUnicode(false);

            modelBuilder.Entity<LOAISACH>()
                .HasMany(e => e.SACHes)
                .WithOptional(e => e.LOAISACH)
                .HasForeignKey(e => e.MALOAISACH);

            modelBuilder.Entity<LOAITAIKHOAN>()
                .HasMany(e => e.TAIKHOANs)
                .WithOptional(e => e.LOAITAIKHOAN1)
                .HasForeignKey(e => e.LOAITAIKHOAN);

            modelBuilder.Entity<MUONTRASACH>()
                .Property(e => e.MATHE)
                .IsUnicode(false);

            modelBuilder.Entity<MUONTRASACH>()
                .Property(e => e.MASACH)
                .IsUnicode(false);

            modelBuilder.Entity<MUONTRASACH>()
                .HasMany(e => e.CHITIETMUONTRASACHes)
                .WithOptional(e => e.MUONTRASACH)
                .HasForeignKey(e => e.MAMUONTRASACH);

            modelBuilder.Entity<NHANVIEN>()
                .Property(e => e.MANHANVIEN)
                .IsUnicode(false);

            modelBuilder.Entity<NHANVIEN>()
                .Property(e => e.SODIENTHOAI)
                .IsUnicode(false);

            modelBuilder.Entity<NHANVIEN>()
                .Property(e => e.MATAIKHOAN)
                .IsUnicode(false);

            modelBuilder.Entity<NHAXUATBAN>()
                .Property(e => e.MANXB)
                .IsUnicode(false);

            modelBuilder.Entity<NHAXUATBAN>()
                .Property(e => e.EMAIL)
                .IsUnicode(false);

            modelBuilder.Entity<NHAXUATBAN>()
                .Property(e => e.TRANGWEB)
                .IsUnicode(false);

            modelBuilder.Entity<NHAXUATBAN>()
                .HasMany(e => e.SACHes)
                .WithOptional(e => e.NHAXUATBAN)
                .HasForeignKey(e => e.MANHAXUATBAN);

            modelBuilder.Entity<SACH>()
                .Property(e => e.MASACH)
                .IsUnicode(false);

            modelBuilder.Entity<SACH>()
                .Property(e => e.FILEPATH)
                .IsUnicode(false);

            modelBuilder.Entity<SACH>()
                .Property(e => e.MALOAISACH)
                .IsUnicode(false);

            modelBuilder.Entity<SACH>()
                .Property(e => e.MALINHVUC)
                .IsUnicode(false);

            modelBuilder.Entity<SACH>()
                .Property(e => e.MATACGIA)
                .IsUnicode(false);

            modelBuilder.Entity<SACH>()
                .Property(e => e.MANHAXUATBAN)
                .IsUnicode(false);

            modelBuilder.Entity<TACGIA>()
                .Property(e => e.MATACGIA)
                .IsUnicode(false);

            modelBuilder.Entity<TACGIA>()
                .Property(e => e.LINKWEBSITE)
                .IsUnicode(false);

            modelBuilder.Entity<TAIKHOAN>()
                .Property(e => e.TENTAIKHOAN)
                .IsUnicode(false);

            modelBuilder.Entity<TAIKHOAN>()
                .Property(e => e.MATKHAU)
                .IsUnicode(false);

            modelBuilder.Entity<TAIKHOAN>()
                .Property(e => e.EMAIL)
                .IsUnicode(false);

            modelBuilder.Entity<TAIKHOAN>()
                .HasMany(e => e.DOCGIAs)
                .WithOptional(e => e.TAIKHOAN)
                .HasForeignKey(e => e.MATAIKHOAN);

            modelBuilder.Entity<TAIKHOAN>()
                .HasMany(e => e.NHANVIENs)
                .WithOptional(e => e.TAIKHOAN)
                .HasForeignKey(e => e.MATAIKHOAN);

            modelBuilder.Entity<THETHUVIEN>()
                .Property(e => e.MATHE)
                .IsUnicode(false);

            modelBuilder.Entity<THETHUVIEN>()
                .HasMany(e => e.DOCGIAs)
                .WithOptional(e => e.THETHUVIEN)
                .HasForeignKey(e => e.MASOTHE);
        }
    }
}
