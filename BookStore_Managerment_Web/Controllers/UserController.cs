using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using BookStore_Managerment_Web.Models;


namespace BookStore_Managerment_Web.Controllers
{
    public class UserController : Controller
    {
        BookStoreDataContext db = new BookStoreDataContext();
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
            var gioitinh = bool.Parse(collection["User_Sex"]);
            var ngaysinh = DateTime.Parse(collection["User_Birthday"]);
            var taikhoan = collection["User_AccountName"];
            var matkhau = collection["User_AccountPassword"];
            var matkhau_xacnhan = collection["User_AccountPassword_authen"];
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
                        MessageBox.Show("thành công");
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
                Session["Account"] = kh;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
                return this.DangNhap();
            }

        }
    }
}