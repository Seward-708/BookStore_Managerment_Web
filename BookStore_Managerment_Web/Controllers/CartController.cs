using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using BookStore_Managerment_Web.Models;

namespace BookStore_Managerment_Web.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        BookStoreDataContext db = new BookStoreDataContext();

        public List<GioHang> Laygiohang()
        {
            List<GioHang> lstGioHang = Session["Giohang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
                Session["Giohang"] = lstGioHang;
            }
            return lstGioHang;
        }

        public ActionResult ThemGioHang(int id, string strURL)
        {
            List<GioHang> lstGioHang = Laygiohang();
            var sach = db.Products.FirstOrDefault(p => p.Product_ID == id);
            GioHang sanpham = lstGioHang.Find(p => p.Product_ID == id);
            if (sanpham == null)
            {
                sanpham = new GioHang(id);
                lstGioHang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                if (sanpham.Cart_Detail_Quantity < sach.Product_Quantity)
                {
                    sanpham.Cart_Detail_Quantity++;
                    return Redirect(strURL);
                }
            }
            return Redirect(strURL);
        }


        private int TongSoLuong()
        {
            int tsl = 0;
            List<GioHang> lstGioHang = Session["Giohang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                tsl = lstGioHang.Sum(n => n.Cart_Detail_Quantity);
            }
            return tsl;
        }

        private int TongSoLuongSanPham()
        {
            int tsl = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                tsl = lstGioHang.Count();
            }
            return tsl;
        }

        private double TongTien()
        {
            double tt = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                tt = lstGioHang.Sum(n => n.TotalMoney);
            }
            return tt;
        }

        public ActionResult GioHang()
        {
            List<GioHang> lstGioHang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();

            return View(lstGioHang);
        }

        public ActionResult GioHangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return PartialView();
        }
        public ActionResult Xoagiohang(int id)
        {
            List<GioHang> lstGioHang = Laygiohang();
            GioHang sanpham = lstGioHang.SingleOrDefault(n => n.Product_ID == id);
            if (sanpham != null)
            {
                lstGioHang.RemoveAll(n => n.Product_ID == id);
                return RedirectToAction("GioHang");
            }
            return RedirectToAction("GioHang");
        }

        public ActionResult Capnhatgiohang(int id, System.Web.Mvc.FormCollection collection)
        {
            List<GioHang> lstGioHang = Laygiohang();
            var sach = db.Products.FirstOrDefault(p => p.Product_ID == id);
            GioHang sanpham = lstGioHang.SingleOrDefault(p => p.Product_ID == id);
            if (sanpham != null)
            {
                sanpham.Cart_Detail_Quantity = int.Parse(collection["txtSoLg"].ToString().Trim());
                if (sanpham.Cart_Detail_Quantity > sach.Product_Quantity)
                {
                    //MessageBox.Show("Không còn đủ sách để bán");
                    sanpham.Cart_Detail_Quantity = 1;
                }
                else if(sanpham.Cart_Detail_Quantity <=0)
                    {
                    sanpham.Cart_Detail_Quantity = 1;
                }

            }
            return RedirectToAction("GioHang");
        }
        public ActionResult Xoatatcagiohang()
        {
            List<GioHang> lstGioHang = Laygiohang();
            lstGioHang.Clear();
            return RedirectToAction("GioHang");
        }
        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "User");
            }
            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<GioHang> lstGioHang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            ViewBag.Tongsoluongsanpham = TongSoLuongSanPham();
            return View(lstGioHang);
        }
        [HttpPost]
        public ActionResult DatHang(System.Web.Mvc.FormCollection collection)
        {
            Cart dh = new Cart();
            User kh = (User)Session["TaiKhoan"];
            Product s = new Product();

            List<GioHang> gh = Laygiohang();
            var ngaygiao = string.Format("{0:MM/dd/yyyy}", collection["NgayGiao"]);


            dh.Users_ID = kh.Users_ID;
            dh.Cart_DateCreate = DateTime.Now;
            dh.Cart_Deliveryday = DateTime.Parse(ngaygiao);
            dh.Cart_State = true;
            dh.Cart_Address = collection["DiaChi"];

            db.Carts.InsertOnSubmit(dh);
            db.SubmitChanges();
            foreach (var ele in gh)
            {
                Cart_Detail ctdh = new Cart_Detail();
                ctdh.Cart_ID = dh.Cart_ID;
                ctdh.Product_ID = ele.Product_ID;
                ctdh.Cart_Detail_Quantity = ele.Cart_Detail_Quantity;
                ctdh.Cart_Detail_Price = ele.Cart_Detail_Price;
                s = db.Products.Single(n => n.Product_ID == ele.Product_ID);
                s.Product_Quantity -= ctdh.Cart_Detail_Quantity;
                db.SubmitChanges();
                db.Cart_Details.InsertOnSubmit(ctdh);
            }
            db.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("XacNhanDonHang", "Cart");
        }

        // GET: GioHang
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult XacNhanDonHang()
        {
            return View();
        }

        public ActionResult TangSoLuong(int id)
        {
            List<GioHang> lst = Laygiohang();
            GioHang item = lst.FirstOrDefault(p => p.Product_ID == id);
            Product sanpham = db.Products.FirstOrDefault(p => p.Product_ID == id);
            if(item.Cart_Detail_Quantity < sanpham.Product_Quantity)
            {
                item.Cart_Detail_Quantity++;
            }
            return RedirectToAction("GioHang", "Cart");
        }

        public ActionResult GiamSoLuong(int id)
        {
            List<GioHang> lst = Laygiohang();
            GioHang item = lst.FirstOrDefault(p => p.Product_ID == id);
            Product sanpham = db.Products.FirstOrDefault(p => p.Product_ID == id);
            if (item.Cart_Detail_Quantity == 1)
            {
                return RedirectToAction("GioHang", "Cart");
            }else
            {
                item.Cart_Detail_Quantity--;
            }
            return RedirectToAction("GioHang", "Cart");
        }
    }
}