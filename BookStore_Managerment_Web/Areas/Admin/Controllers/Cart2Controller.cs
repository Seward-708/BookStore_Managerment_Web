using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStore_Managerment_Web.Models;
using PagedList;

namespace BookStore_Managerment_Web.Areas.Admin.Controllers
{
    public class Cart2Controller : Controller
    {
        // GET: Admin/Cart
        BookStoreDataContext db = new BookStoreDataContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Cart(int? page, string searching)
        {
            if (page == null)
                page = 1;
            Session["KhachHangTemp"] = db.Users.ToList();

            if (searching == null)
            {
                var all_cart = db.Carts.Where(x => x.Cart_State == true).ToList().OrderByDescending(x => x.Cart_DateCreate);
                int pageSize = 10;
                int pageNum = page ?? 1;
                return View(all_cart.ToPagedList(pageNum, pageSize));
            }
            else
            {
                var temp_id = db.Users.Where(x => x.Users_Name.Contains(searching)).Select(x => x.Users_ID).FirstOrDefault();
                var all_cart = db.Carts.Where(x => x.Users_ID == temp_id).ToList().OrderByDescending(m => m.Cart_DateCreate);
                int pageSize = 10;
                int pageNum = page ?? 1;
                return View(all_cart.ToPagedList(pageNum, pageSize));
            }
        }
        public ActionResult Cart_No(int? page, string searching)
        {
            if (page == null)
                page = 1;
            Session["KhachHangTemp"] = db.Users.ToList();

            if (searching == null)
            {
                var all_cart = db.Carts.Where(x => x.Cart_State == true && x.Cart_Delivery_State == null).ToList().OrderByDescending(x => x.Cart_DateCreate);
                int pageSize = 10;
                int pageNum = page ?? 1;
                return View(all_cart.ToPagedList(pageNum, pageSize));
            }
            else
            {
                var temp_id = db.Users.Where(x => x.Users_Name.Contains(searching)).Select(x => x.Users_ID).FirstOrDefault();
                var all_cart = db.Carts.Where(x => x.Users_ID == temp_id && x.Cart_Delivery_State == null).ToList().OrderByDescending(m => m.Cart_DateCreate);
                int pageSize = 10;
                int pageNum = page ?? 1;
                return View(all_cart.ToPagedList(pageNum, pageSize));
            }
        }
        public ActionResult Cart_Detail(int id, int? page)
        {
            Session["SP"] = db.Products.ToList();
            ViewBag.TongTien = db.TongTiens.Where(x => x.Cart_ID == id).Select(x=>x.gia).FirstOrDefault();
            ViewBag.getid = id;
              var detail = db.Cart_Details.Where(x => x.Cart_ID == id).ToList();
            int pageSize = 5;
            int pageNum = page ?? 1;
            return View(detail.ToPagedList(pageNum, pageSize));
        }
        public ActionResult Cart_Accept(int id)
        {
            var find_x = db.Carts.Where(x => x.Cart_ID == id).FirstOrDefault();
            find_x.Cart_Delivery_State = false;
            UpdateModel(find_x);
            db.SubmitChanges();
            return RedirectToAction("Cart_No");
        }
        public ActionResult CartALL_Detail(int id, int? page)
        {
            Session["SP"] = db.Products.ToList();
            ViewBag.TongTien = db.TongTiens.Where(x => x.Cart_ID == id).Select(x => x.gia).FirstOrDefault();
            ViewBag.getid = id;
            var detail = db.Cart_Details.Where(x => x.Cart_ID == id).ToList();
            int pageSize = 5;
            int pageNum = page ?? 1;
            return View(detail.ToPagedList(pageNum, pageSize));
        }
        public ActionResult CartALL_Accept(int id)
        {
            var find_x = db.Carts.Where(x => x.Cart_ID == id).FirstOrDefault();
            find_x.Cart_Delivery_State = false;
            UpdateModel(find_x);
            db.SubmitChanges();
            return RedirectToAction("Cart");
        }
    }
}