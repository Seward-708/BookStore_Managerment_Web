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

            var check = collection["CheckState"].ToString() == "nam" ? false : true;
            var gioitinh = check;
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
    }
}