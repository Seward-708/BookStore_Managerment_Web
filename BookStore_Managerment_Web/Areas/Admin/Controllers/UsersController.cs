using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStore_Managerment_Web.Models;
using PagedList;

namespace BookStore_Managerment_Web.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        // GET: Admin/Users
        BookStoreDataContext db = new BookStoreDataContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Users(int? page, string searching)
        {
            if (page == null)
                page = 1;
            if (searching == null)
            {
                var all_unit = db.Users.ToList().OrderBy(m => m.Users_ID);
                int pageSize = 10;
                int pageNum = page ?? 1;
                return View(all_unit.ToPagedList(pageNum, pageSize));
            }
            else
            {
                var all_unit = db.Users.Where(x => x.Users_Name.Contains(searching)).ToList().OrderBy(m => m.Users_ID);
                int pageSize = 10;
                int pageNum = page ?? 1;
                return View(all_unit.ToPagedList(pageNum, pageSize));
            }
        }
        public ActionResult Providers(int? page, string searching)
        {
            if (page == null)
                page = 1;
            if (searching == null)
            {
                var all_unit = db.Providers.ToList().OrderBy(m => m.Providers_ID);
                int pageSize = 10;
                int pageNum = page ?? 1;
                return View(all_unit.ToPagedList(pageNum, pageSize));
            }
            else
            {
                var all_unit = db.Providers.Where(x => x.Providers_Name.Contains(searching)).ToList().OrderBy(m => m.Providers_ID);
                int pageSize = 10;
                int pageNum = page ?? 1;
                return View(all_unit.ToPagedList(pageNum, pageSize));
            }
        }
        public ActionResult CreateProviders()
        {

            return View();
        }
        [HttpPost]
        public ActionResult CreateProviders(FormCollection collection, Provider s)
        {

            var E_Publisher = collection["Providers_Name"];
            if (string.IsNullOrEmpty(E_Publisher))
            {
                ViewBag.ThongBao = "Vui lòng nhập đầy đủ thông tin!";

            }
            else
            {
                s.Providers_Name = E_Publisher;
                db.Providers.InsertOnSubmit(s);
                db.SubmitChanges();
                return RedirectToAction("Providers");
            }

            return this.CreateProviders();
        }
        public ActionResult EditProviders(int id)
        {
            var s = db.Providers.First(m => m.Providers_ID == id);

            return View(s);

        }
        [HttpPost]
        public ActionResult EditProviders(int id, FormCollection collection)
        {
            var s = db.Providers.First(m => m.Providers_ID == id);
            var E_namviet = collection["Providers_Name"];
            if (string.IsNullOrEmpty(E_namviet))
            {
                ViewBag.ThongBao = "Vui lòng nhập đầy đủ thông tin!";
            }
            else
            {
                s.Providers_Name = E_namviet.Trim();
                UpdateModel(s);
                db.SubmitChanges();
                return RedirectToAction("Providers");
            }

            return this.EditProviders(id);

        }
        //
        public ActionResult Publishers(int? page, string searching)
        {
            if (page == null)
                page = 1;
            if (searching == null)
            {
                var all_unit = db.Publishers.ToList().OrderBy(m => m.Publisher_ID);
                int pageSize = 10;
                int pageNum = page ?? 1;
                return View(all_unit.ToPagedList(pageNum, pageSize));
            }
            else
            {
                var all_unit = db.Publishers.Where(x => x.Publisher_Name.Contains(searching)).ToList().OrderBy(m => m.Publisher_ID);
                int pageSize = 10;
                int pageNum = page ?? 1;
                return View(all_unit.ToPagedList(pageNum, pageSize));
            }
        }
        public ActionResult CreatePublishers()
        {

            return View();
        }
        [HttpPost]
        public ActionResult CreatePublishers(FormCollection collection, Publisher s)
        {

            var E_Publisher = collection["Publisher_Name"];
            if (string.IsNullOrEmpty(E_Publisher))
            {
                ViewBag.ThongBao = "Vui lòng nhập đầy đủ thông tin!";

            }
            else
            {
                s.Publisher_Name = E_Publisher;
                db.Publishers.InsertOnSubmit(s);
                db.SubmitChanges();
                return RedirectToAction("Publishers");
            }

            return this.CreatePublishers();
        }
        public ActionResult EditPublishers(int id)
        {
            var s = db.Publishers.First(m => m.Publisher_ID == id);

            return View(s);

        }
        [HttpPost]
        public ActionResult EditPublishers(int id, FormCollection collection)
        {
            var s = db.Publishers.First(m => m.Publisher_ID == id);
            var E_namviet = collection["Publisher_Name"];
            if (string.IsNullOrEmpty(E_namviet))
            {
                ViewBag.ThongBao = "Vui lòng nhập đầy đủ thông tin!";
            }
            else
            {
                s.Publisher_Name = E_namviet.Trim();
                UpdateModel(s);
                db.SubmitChanges();
                return RedirectToAction("Publishers");
            }

            return this.EditPublishers(id);

        }

    }
}
