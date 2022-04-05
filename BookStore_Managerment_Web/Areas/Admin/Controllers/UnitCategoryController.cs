using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStore_Managerment_Web.Models;
using PagedList;

namespace BookStore_Managerment_Web.Areas.Admin.Controllers
{
    public class UnitCategoryController : Controller
    {
        // GET: Admin/UnitCategory
        BookStoreDataContext db = new BookStoreDataContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Unit(int? page, string searching)
        {
            if (page == null)
                page = 1;


            if (searching == null)
            {
                var all_unit = db.Units.ToList().OrderBy(m => m.Unit_ID);
                int pageSize = 10;
                int pageNum = page ?? 1;
                return View(all_unit.ToPagedList(pageNum, pageSize));
            }
            else
            {
                var all_unit = db.Units.Where(x => x.Unit_Name.Contains(searching)).ToList().OrderBy(m => m.Unit_ID);
                int pageSize = 10;
                int pageNum = page ?? 1;
                return View(all_unit.ToPagedList(pageNum, pageSize));
            }
        }
        public ActionResult CreateUnit()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateUnit(FormCollection collection, Unit s)
        {
            var name = collection["Unit_Name"];
            var check = collection["CheckState"];
            s.Unit_Name = name;

            if (check == "choban")
            {
                s.Unit_State = true;
            }
            else
            if (check == "khongchoban")
            {
                s.Unit_State = false;
            }
            db.Units.InsertOnSubmit(s);
            db.SubmitChanges();
            return RedirectToAction("Unit");
        }
        public ActionResult EditUnit(int id)
        {
            var find_x = db.Units.Where(x => x.Unit_ID == id).FirstOrDefault();
            return View(find_x);
        }
        [HttpPost]
        public ActionResult EditUnit(int id, FormCollection collection)
        {
            var find_x = db.Units.Where(x => x.Unit_ID == id).FirstOrDefault();
            var name = collection["Unit_Name"];
            var check = collection["CheckState"];
            find_x.Unit_Name = name;
            if (check == "choban")
            {
                find_x.Unit_State = true;
            }
            else
          if (check == "khongchoban")
            {
                find_x.Unit_State = false;
            }
            UpdateModel(find_x);
            db.SubmitChanges();

            return RedirectToAction("Unit");
        }
        public ActionResult DeleteUnit(int id)
        {
            var find_x = db.Units.Where(x => x.Unit_ID == id).FirstOrDefault();
            if (find_x.Unit_State == true)
            {
                return View(find_x);
            }
            else
            {
                return RedirectToAction("Unit");
            }
        }
        [HttpPost]
        public ActionResult DeleteUnit(int id, FormCollection collection)
        {
            var find_x = db.Units.Where(x => x.Unit_ID == id).FirstOrDefault();
            find_x.Unit_State = false;
            UpdateModel(find_x);
            db.SubmitChanges();

            return RedirectToAction("Unit");
        }

        ///////////////////Category
        public ActionResult Category(int? page, string searching)
        {
            if (page == null)
                page = 1;


            if (searching == null)
            {
                var all_unit = db.Categories.ToList().OrderBy(m => m.Category_ID);
                int pageSize = 10;
                int pageNum = page ?? 1;
                return View(all_unit.ToPagedList(pageNum, pageSize));
            }
            else
            {
                var all_unit = db.Categories.Where(x => x.Category_Name.Contains(searching)).ToList().OrderBy(m => m.Category_ID);
                int pageSize = 10;
                int pageNum = page ?? 1;
                return View(all_unit.ToPagedList(pageNum, pageSize));
            }
        }
        public ActionResult CreateCategory()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateCategory(FormCollection collection, Category s)
        {
            var name = collection["Category_Name"];
            var check = collection["CheckState"];
            var checkbook = collection["CheckState1"];
            s.Category_Name = name;

            if (check == "choban")
            {
                s.Category_State = true;
            }
            else
            if (check == "khongchoban")
            {
                s.Category_State = false;
            }
            if (checkbook == "book")
            {
                s.Category_isBook = true;
            }
            else
            if (checkbook == "notbook")
            {
                s.Category_isBook = false;
            }
            db.Categories.InsertOnSubmit(s);
            db.SubmitChanges();
            return RedirectToAction("Category");
        }
        public ActionResult EditCategory(int id)
        {
            var find_x = db.Categories.Where(x => x.Category_ID == id).FirstOrDefault();
            return View(find_x);
        }
        [HttpPost]
        public ActionResult EditCategory(int id, FormCollection collection)
        {
            var s = db.Categories.Where(x => x.Category_ID == id).FirstOrDefault();
            var name = collection["Category_Name"];
            var check = collection["CheckState"];
            var checkbook = collection["CheckState1"];
            s.Category_Name = name;

            if (check == "choban")
            {
                s.Category_State = true;
            }
            else
            if (check == "khongchoban")
            {
                s.Category_State = false;
            }
            if (checkbook == "book")
            {
                s.Category_isBook = true;
            }
            else
            if (checkbook == "notbook")
            {
                s.Category_isBook = false;
            }
            UpdateModel(s);
            db.SubmitChanges();

            return RedirectToAction("Category");
        }
        public ActionResult DeleteCategory(int id)
        {
            var find_x = db.Categories.Where(x => x.Category_ID == id).FirstOrDefault();
            if (find_x.Category_State == true)
            {
                return View(find_x);
            }
            else
            {
                return RedirectToAction("Category");
            }
        }
        [HttpPost]
        public ActionResult DeleteCategory(int id, FormCollection collection)
        {
            var find_x = db.Categories.Where(x => x.Category_ID == id).FirstOrDefault();
            find_x.Category_State = false;
            UpdateModel(find_x);
            db.SubmitChanges();

            return RedirectToAction("Category");
        }
    }
}