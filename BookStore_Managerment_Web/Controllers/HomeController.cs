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




        public PartialViewResult category()
        {
            List<Category> lst = new List<Category>();
            foreach (var item in db.Categories)
            {
                if (db.Products.Where(p => p.Category_ID == item.Category_ID).Count() != 0)
                    lst.Add(item);
            }
            return PartialView(lst);
        }

        public PartialViewResult category_items(int id)
        {
            List<Product> lst = new List<Product>();
            foreach (var item in db.Products)
            {
                if (item.Category_ID == id)
                    lst.Add(item);
            }
            return PartialView(lst);
        }




        public PartialViewResult Category_List()
        {
            return PartialView(db.Categories.Where(p=>p.Category_State == true).ToList());
        }


        public ActionResult Category_search(int category_id) {
            return View(db.Products.Where(p => (p.Category_ID == category_id) && (p.Product_State == true)).ToList());
        }

        public ActionResult Category_searchByName(string category_name)
        {
            return View(db.Products.Where(p => (p.Category.Category_Name == category_name) && (p.Product_State == true)).ToList());
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