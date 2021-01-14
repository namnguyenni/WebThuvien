using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebThuvien.Models.Entity;
namespace WebThuvien.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        //trang đăng nhập đăng kí
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Dangnhap(string tentaikhoan,string matkhau,bool luumk)
        {

            return View();
        }

        [HttpPost]
        public ActionResult Dangxuat(string tentaikhoan,string matkhau)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Dangki(TAIKHOAN thongtintaikhoan)
        {

            return View();
        }


        string macode_gen = "";
        [HttpGet]
        public ActionResult QuenMatKhau(string tentaikhoan)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            TAIKHOAN taikhoan = db.TAIKHOANs.Single(x => x.TENTAIKHOAN == tentaikhoan);
            //sinh ma code
            Random rdn = new Random();
            macode_gen = rdn.Next(1000, 9999).ToString();
            //gui ma code ve gmail
            GuiEmail("E-LIBRARY - Mã xác nhận thay đổi thông tin tài khoản",
                taikhoan.EMAIL,"nguyennam4work@gmail.com", "Namnam1702",
                "Mã xác nhận lấy lại mật khẩu của bạn là : " + macode_gen);
            return View();
        }

        [HttpPost]
        public ActionResult QuenMatKhau_NhapMa(string tentaikhoan,string macode,string matkhaumoi,string xacnhanmatkhaumoi)
        {
            QLTHUVIEN db = new QLTHUVIEN();
            TAIKHOAN taikhoan = db.TAIKHOANs.Single(x => x.TENTAIKHOAN == tentaikhoan);

            if (macode == macode_gen && matkhaumoi == xacnhanmatkhaumoi)
            {
                taikhoan.MATKHAU = matkhaumoi;

            }
            return View();
        }

        public void GuiEmail(string Title, string ToEmail, string FromEmail, string PassWord, string Content)
        {
            SmtpClient smtp = new SmtpClient();
            try
            {
                // goi email
                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.To.Add(ToEmail); // Địa chỉ nhận
                mail.From = new MailAddress(ToEmail); // Địa chửi gửi
                mail.Subject = Title;  // tiêu đề gửi
                mail.Body = Content;                 // Nội dung
                mail.IsBodyHtml = true;
                smtp.Host = "smtp.gmail.com"; // host gửi của Gmail
                smtp.Port = 587;               //port của Gmail
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential
                (FromEmail, PassWord);//Tài khoản password người gửi
                smtp.EnableSsl = true;   //kích hoạt giao tiếp an toàn SSL
                smtp.Send(mail);   //Gửi mail đi
            }
            catch (Exception ex)
            {
                ViewBag.ThongBao = "Không chính xác!!";
            }
        }



    }
}