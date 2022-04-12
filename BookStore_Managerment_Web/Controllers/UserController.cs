using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.UI;
using System.Windows.Forms;
using BookStore_Managerment_Web.Models;


namespace BookStore_Managerment_Web.Controllers
{
    public class UserController : Controller
    {
        BookStoreDataContext db = new BookStoreDataContext();
        public static int OTP;
        public static User Account;
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(System.Web.Mvc.FormCollection collection, User kh)
        {
            var hoten = collection["Users_Name"];
            var sdt = collection["User_Phone"];
            var email = collection["User_Email"];

            var check = collection["CheckState"].ToString() == "nam" ? false : true;
            var gioitinh = check;
            var ngaysinh = DateTime.Parse(collection["User_Birthday"]);
            var taikhoan = collection["User_AccountName"];
            var matkhau = collection["User_AccountPassword"];
            var matkhau_xacnhan = collection["User_AccountPassword_authen"];
            if(db.Users.Where(p=>p.User_Email == email).Count() != 0)
            {
                ViewBag.ThongBao = "Email này đã được sử dụng để đăng ký";
                return this.DangKy();
            }


            if (string.IsNullOrEmpty(matkhau_xacnhan))
            {
                ViewData["NhapMKXN"] = "Phải nhập mật khẩu xác nhận!";
            }
            else
            {
                if (!matkhau.Equals(matkhau_xacnhan))
                {
                    ViewData["MatKhauGiongNhau"] = "Mật khẩu và mật khẩu xác nhận phải giống nhau";
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        kh.Users_Name = hoten;
                        kh.User_Phone = sdt;
                        kh.User_Email = email;
                        kh.User_Sex = gioitinh;
                        kh.User_Birthday = ngaysinh;
                        kh.User_State = true;
                        kh.User_AccountName = taikhoan;
                        kh.User_AccountPassword = matkhau;
                        kh.User_Admin = false;

                        db.Users.InsertOnSubmit(kh);
                        db.SubmitChanges();
                        //MessageBox.Show("thành công");
                    }
                    return RedirectToAction("DangKy", "User");
                }
            }
            return this.DangKy();
        }

        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhap(System.Web.Mvc.FormCollection collection)
        {

                var tendangnhap = collection["User_AccountName"];
                var matkhau = collection["User_AccountPassword"];
                User kh = db.Users.SingleOrDefault(n => n.User_AccountName == tendangnhap && n.User_AccountPassword == matkhau);
                if (kh != null)
                {
                    ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                    Session["TaiKhoan"] = kh;
                    if (kh.User_Admin == true)
                        return RedirectToAction("Index", "Admin/Admin");
                    else
                        return RedirectToAction("EditInfor", "User");
                }
                else
                {
                    ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
                    return this.DangNhap();
                }

        }


        public ActionResult DangXuat()
        {
            Session["TaiKhoan"] = null;
            return RedirectToAction("DangNhap", "User");
        }

        public ActionResult EditInfor()
        {
            var user = Session["TaiKhoan"] as User;
                var find = db.Users.Where(x => x.Users_ID == user.Users_ID).FirstOrDefault();
                Session["CartTemp"] = db.Carts.Where(x => x.Users_ID == user.Users_ID).Take(5).ToList();
                Session["Gia"] = db.TongTiens.ToList();
                return View(find);

        }
        [HttpPost]
        public ActionResult EditInfor(System.Web.Mvc.FormCollection collection)
        {
            var user = Session["TaiKhoan"] as User;
            var find = db.Users.Where(x => x.Users_ID == user.Users_ID).FirstOrDefault();
            var ten = collection["Users_Name"];
            var sdt = collection["User_Phone"];
            var email = collection["User_Email"];
            var check = collection["CheckState"];
            var ngaysinh = collection["User_Birthday"];
            if (String.IsNullOrEmpty(ten) || String.IsNullOrEmpty(sdt) || String.IsNullOrEmpty(email) || String.IsNullOrEmpty(ngaysinh))
            {
                ViewBag.TB = "Yêu cầu nhập đầy đủ thông tin";
                return this.EditInfor();
            }
            else
            {
                find.Users_Name = ten.Trim();
                find.User_Phone = sdt;
                find.User_Email = email;
                find.User_Birthday = DateTime.Parse(ngaysinh);
                if (check == "nam")
                {
                    find.User_Sex = false;
                }
                else
                {
                    find.User_Sex = true;
                }
                UpdateModel(find);
                db.SubmitChanges();

            }
            return RedirectToAction("EditInfor", "User");

        }
        
        [HttpGet]
        public  ActionResult QuenMatKhau()
        {
            return View();
        }
        [HttpPost]
        public ActionResult QuenMatKhau(System.Web.Mvc.FormCollection collection)
        {
            var email = collection["User_Email"];

            Account = db.Users.Where(p => p.User_Email == email).FirstOrDefault();
            if (Account == null)
            {
                ViewBag.ThongBao = "Email này chưa được dùng để đăng ký!";
                return this.QuenMatKhau();
            }
            else
            {
                if (email.Contains("@gmail.com"))
                {
                    Random rand = new Random();
                    OTP = rand.Next(100000, 999999);
                    mail(email);
                }
                else
                {
                    ViewBag.ThongBao = "Vui lòng nhập đúng định dạng email";
                    return this.QuenMatKhau();
                }
            }
            return RedirectToAction("OTP_Assign");
        }

        [HttpGet]
        public ActionResult OTP_Assign()
        {
            return View();
        }
        [HttpPost]
        public ActionResult OTP_Assign(System.Web.Mvc.FormCollection collection)
        {
            var otp = collection["Users_Name"];
            int otp_code;
            if(!int.TryParse(otp,out otp_code))
            {
                ViewBag.ThongBao = "Vui lòng nhập OTP";
            }
            else
            {
                if (otp_code != OTP)
                {
                    ViewBag.ThongBao = "OTP chưa chính xác !";
                    return this.OTP_Assign();
                }
            }
            return RedirectToAction("NewPassword");
            
        }


        [HttpGet]
        public ActionResult NewPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult NewPassword(System.Web.Mvc.FormCollection collection)
        {
            var password = collection["User_AccountPassword"];
            var repassword = collection["Users_Name"];

            if (string.Compare(password, repassword) != 0)
            {
                ViewBag.ThongBao = "Mật khẩu nhập lại phải giống nhau!";
                return this.NewPassword();
            }
            var temp = db.Users.Where(p => p.Users_ID == Account.Users_ID).FirstOrDefault();
            temp.User_AccountPassword = password;
            UpdateModel(temp);
            db.SubmitChanges();
            return RedirectToAction("DangNhap");
        }


        public void mail(string Receive_Email)
        {
            Random rnd = new Random();
            const string p = "123123asD";


            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();

            message.From = new MailAddress("tiennguyennaraka@gmail.com");

            //Enter your email blow and also change in database too

            message.To.Add(new MailAddress(Receive_Email));
            message.Subject = "Mã xác nhận tài khoản";
            message.Body = "Đây là mã xác nhận của bạn: " + OTP + "\nXin cám ơn!";

            smtp.Port = 587;
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("tiennguyennaraka@gmail.com", p);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Send(message);
        }


        public ActionResult CancelOrder(int id)
        {
            Cart cart = db.Carts.FirstOrDefault(p => p.Cart_ID == id);
            cart.Cart_State = false;
            cart.Cart_Delivery_State = null;
                    UpdateModel(cart);
                    db.SubmitChanges();            
            return RedirectToAction("EditInfor");
        }

        public ActionResult DisCancelOrder(int id)
        {
            Cart cart = db.Carts.FirstOrDefault(p => p.Cart_ID == id);
            if(cart.Cart_State == false)
            {
                cart.Cart_State = true;
                    UpdateModel(cart);
                    db.SubmitChanges();
            }
            return RedirectToAction("EditInfor");
        }

        

    }
}