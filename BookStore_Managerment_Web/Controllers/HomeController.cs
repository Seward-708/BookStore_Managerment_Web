using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStore_Managerment_Web.Models;


namespace BookStore_Managerment_Web.Controllers
{
    public class HomeController : Controller
    {
        BookStoreDataContext db = new BookStoreDataContext();

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        //thêm where danh mục là máy tính lấy ngẫu nhiên
        public PartialViewResult Calculator_Partial() {
            return PartialView(db.Products.ToList());
        }

        //them where danh mục la light novel
        public PartialViewResult LightNovel_Partial()
        {
            return PartialView(db.Products.ToList());
        }

        //thêm where danh mục là đồ chơi
        public PartialViewResult Toy_Partial()
        {
            return PartialView(db.Products.ToList());
        }

        public PartialViewResult Category_List()
        {
            return PartialView(db.Categories.Where(p=>p.Category_State == true).ToList());
        }


        public ActionResult Category_search(int category_id) {
            return View(db.Products.Where(p => (p.Category_ID == category_id) && (p.Product_State == true)).ToList());
        } 

        public ActionResult Product_Detail(int product_id)
        {
            return View(db.Products.Where(p => (p.Product_ID == product_id) && (p.Product_State == true)).SingleOrDefault());
        }


        public ActionResult Product_Detail_Recommend_Partial(int product_id)
        {
            Product pro = db.Products.FirstOrDefault(p => p.Product_ID == product_id);
            int category_id = db.Categories.Where(p => (p.Category_State == true) && (p.Category_ID == pro.Category_ID)).Select(p => p.Category_ID).FirstOrDefault();
            return PartialView(db.Products.Where(p => (p.Category_ID == category_id) && (p.Product_State == true)).ToList());
        }



    }
}