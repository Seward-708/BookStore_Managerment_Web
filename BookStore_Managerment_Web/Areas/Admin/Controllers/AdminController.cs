using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookStore_Managerment_Web.Models;
using PagedList;

namespace BookStore_Managerment_Web.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin/Admin
        BookStoreDataContext db = new BookStoreDataContext();
        public ActionResult Index()
        {
            int monday = FirstDateInWeek(DateTime.Now, DayOfWeek.Monday).Day;
            var time_now = DateTime.Now;
            var doanhthu = db.Doanhthus.Where(x => x.Date.Value.Month == time_now.Month && x.Date.Value.Year == time_now.Year).Sum(x => x.Sums);

            Session["Doanhthu"] = doanhthu;
            Session["HetHang"] = db.Products.Where(x => x.Product_Quantity == 0).Count();
            Session["SapHetHang"] = db.Users.Count();


            var series = new List<double>();

            double[] temp = new double[7];
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = 0;
            }
            var list = db.DoanhThuBangs.ToList();
            foreach (var ele in list)
            {
                switch (ele.Cart_DateCreate.Value.DayOfWeek)
                {
                    case DayOfWeek.Monday: temp[0] = (double)ele.Doanhthungay; break;
                    case DayOfWeek.Tuesday: temp[1] = (double)ele.Doanhthungay; break;
                    case DayOfWeek.Wednesday: temp[2] = (double)ele.Doanhthungay; ; break;
                    case DayOfWeek.Thursday: temp[3] = (double)ele.Doanhthungay; break;
                    case DayOfWeek.Friday: temp[4] = (double)ele.Doanhthungay; break;
                    case DayOfWeek.Saturday: temp[5] = (double)ele.Doanhthungay; break;
                    case DayOfWeek.Sunday: temp[6] = (double)ele.Doanhthungay; break;
                }
            }

            ViewBag.thu2 = temp[0];
            ViewBag.thu3 = temp[1];
            ViewBag.thu4 = temp[2];
            ViewBag.thu5 = temp[3];
            ViewBag.thu6 = temp[4];
            ViewBag.thu7 = temp[5];
            ViewBag.cn = temp[6];
            //for (int i = 0; i < temp.Length; i++)
            //{
            //    series.Add(temp[i]);
            //}

            //Session["Doanhthun"] = series;

            Session["DonHangChuaDuyet"] = db.Carts.Where(x => x.Cart_Delivery_State == null).Count();

            return View();
        }
        public DateTime FirstDateInWeek(DateTime dt, DayOfWeek weekStartDay)
        {
            while (dt.DayOfWeek != weekStartDay)
                dt = dt.AddDays(-1);
            return dt;
        }
        public ActionResult Sach(int? page, string searching)
        {
            if (page == null)
                page = 1;
            Session["ListCategory"] = db.Categories.ToList();
            Session["ListUnit"] = db.Units.ToList();
            Session["ListProviders"] = db.Providers.ToList();
            Session["ListPublisher"] = db.Publishers.ToList();

            if (searching == null)
            {
                var all_sach = db.Products.Where(x => x.Product_isBook == true).ToList().OrderBy(m => m.Product_Quantity);
                int pageSize = 10;
                int pageNum = page ?? 1;
                return View(all_sach.ToPagedList(pageNum, pageSize));
            }
            else
            {
                var all_sach = db.Products.Where(x => x.Product_isBook == true && x.Product_Name.Contains(searching) || x.Product_Quantity.ToString() == searching).ToList().OrderBy(m => m.Product_ID);
                int pageSize = 10;
                int pageNum = page ?? 1;
                return View(all_sach.ToPagedList(pageNum, pageSize));
            }
        }
        public ActionResult DetailSach(int id)
        {
            var D_sach = db.Products.Where(m => m.Product_ID == id).First();
            Session["ListCategory"] = db.Categories.ToList();
            Session["ListUnit"] = db.Units.ToList();
            Session["ListProviders"] = db.Providers.ToList();
            Session["ListPublisher"] = db.Publishers.ToList();
            var temp = db.Book_Writters.Where(x => x.Product_ID == id).FirstOrDefault();
            if (temp != null)
            {
                Session["Authortemp"] = db.Authors.Where(x => x.Author_ID == temp.Author_ID).FirstOrDefault();
                Session["BWtemp"] = temp;
            }
            return View(D_sach);
        }

        public ActionResult CreateSach()
        {
            var all_category = db.Categories.Where(x => x.Category_State == true && x.Category_isBook == true).ToList();
            ViewBag.VbCategoryList = new SelectList(all_category, "Category_ID", "Category_Name");
            var all_Unit = db.Units.Where(x => x.Unit_State == true).ToList();
            ViewBag.VbUnit = new SelectList(all_Unit, "Unit_ID", "Unit_Name");
            //
            var all_Providers = db.Providers.ToList();
            ViewBag.VbProviders = new SelectList(all_Providers, "Providers_ID", "Providers_Name");
            var all_Publisher = db.Publishers.ToList();
            ViewBag.VbPublisher = new SelectList(all_Publisher, "Publisher_ID", "Publisher_Name");
            return View();
        }
        [HttpPost]
        public ActionResult CreateSach(FormCollection collection, Product s)
        {
            var E_tensach = collection["Product_Name"];
            var E_Gia = collection["Product_SellPrice"];
            var E_SL = collection["Product_Quantity"];

            var E_Hinh = collection["Product_Image"];
            var E_TL = collection["Product_Weight"];
            var E_Category = collection["Category_ID"];
            var E_Unit = collection["Unit_ID"];
            var E_Providers = collection["Providers_ID"];
            var E_Publisher = collection["Publisher_ID"];
            var E_infor = collection["Product_Infor"];
            var E_tacgia = collection["tacgia"];
            var E_namviet = collection["namviet"];
            if (string.IsNullOrEmpty(E_tensach) || string.IsNullOrEmpty(E_Gia) ||
                string.IsNullOrEmpty(E_Hinh) || string.IsNullOrEmpty(E_TL) || string.IsNullOrEmpty(E_Category) || string.IsNullOrEmpty(E_Unit) || string.IsNullOrEmpty(E_Providers) ||
                string.IsNullOrEmpty(E_Publisher) || string.IsNullOrEmpty(E_SL) || string.IsNullOrEmpty(E_tacgia) || string.IsNullOrEmpty(E_namviet))
            {
                ViewBag.ThongBao = "Vui lòng nhập đầy đủ thông tin!";
            }
            else
            {
                if (double.Parse(E_Gia) < 0)
                {
                    ViewBag.ThongBao = "Vui lòng nhập GIÁ hợp lệ!";
                }
                if (int.Parse(E_SL) < 0)
                {
                    ViewBag.ThongBao = "Vui lòng nhập SỐ LƯỢNG hợp lệ!";
                }
                if (int.Parse(E_TL) < 0)
                {
                    ViewBag.ThongBao = "Vui lòng nhập TRỌNG LƯỢNG hợp lệ!";
                }
                if (double.Parse(E_Gia) >= 0 && int.Parse(E_SL) >= 0 && int.Parse(E_TL) >= 0)
                {
                    s.Product_Name = E_tensach.Trim();
                    s.Product_Image = E_Hinh;
                    s.Product_SellPrice = double.Parse(E_Gia);
                    s.Product_Quantity = int.Parse(E_SL);
                    s.Product_isBook = true;
                    s.Product_State = true;
                    s.Product_Weight = int.Parse(E_TL);
                    s.Product_Infor = E_infor;
                    s.Category_ID = int.Parse(E_Category);
                    s.Unit_ID = int.Parse(E_Unit);
                    s.Providers_ID = int.Parse(E_Providers);
                    s.Publisher_ID = int.Parse(E_Publisher);
                    db.Products.InsertOnSubmit(s);
                    db.SubmitChanges();
                    var temp_id = s.Product_ID;

                    var all_tg = db.Authors.Where(x => x.Author_Name.ToLower() == E_tacgia.ToLower().Trim()).ToList();
                    if (all_tg.Count() != 0)
                    {
                        Book_Writter bw = new Book_Writter();
                        bw.Author_ID = all_tg.Where(x => x.Author_Name.ToLower() == E_tacgia.ToLower().Trim()).Select(x => x.Author_ID).FirstOrDefault();
                        bw.Product_ID = temp_id;
                        bw.Year_Reprint = E_namviet;
                        db.Book_Writters.InsertOnSubmit(bw);
                        db.SubmitChanges();
                    }
                    else
                    {
                        Author au = new Author();
                        au.Author_Name = E_tacgia;
                        au.Author_State = true;
                        db.Authors.InsertOnSubmit(au);
                        db.SubmitChanges();
                        var temp_idauthor = au.Author_ID;
                        Book_Writter bw = new Book_Writter();
                        bw.Author_ID = temp_idauthor;
                        bw.Product_ID = temp_id;
                        bw.Year_Reprint = E_namviet;
                        db.Book_Writters.InsertOnSubmit(bw);
                        db.SubmitChanges();
                    }
                    return RedirectToAction("Sach");
                }
                return this.CreateSach();
            }
            return this.CreateSach();
        }
        public string ProcessUpload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return "";
            }
            file.SaveAs(Server.MapPath("~/Content/images/" + file.FileName));
            return "/Content/images/" + file.FileName;
        }

        public ActionResult EditSach(int id)
        {
            var E_sach = db.Products.First(m => m.Product_ID == id);
            var temp = db.Book_Writters.Where(x => x.Product_ID == id).FirstOrDefault();
            if (temp != null)
            {
                Session["Authortemp"] = db.Authors.Where(x => x.Author_ID == temp.Author_ID).FirstOrDefault();
                Session["BWtemp"] = temp;
            }
            var all_category = db.Categories.Where(x => x.Category_State == true && x.Category_isBook == true).ToList();
            ViewBag.VbCategoryList = new SelectList(all_category, "Category_ID", "Category_Name");
            var all_Unit = db.Units.Where(x => x.Unit_State == true).ToList();
            ViewBag.VbUnit = new SelectList(all_Unit, "Unit_ID", "Unit_Name");
            //
            var all_Providers = db.Providers.ToList();
            ViewBag.VbProviders = new SelectList(all_Providers, "Providers_ID", "Providers_Name");
            var all_Publisher = db.Publishers.ToList();
            ViewBag.VbPublisher = new SelectList(all_Publisher, "Publisher_ID", "Publisher_Name");

            return View(E_sach);

        }
        [HttpPost]
        public ActionResult EditSach(int id, FormCollection collection)
        {
            var s = db.Products.First(m => m.Product_ID == id);

            var E_tensach = collection["Product_Name"];
            var E_Gia = collection["Product_SellPrice"];
            var E_SL = collection["Product_Quantity"];
            var E_Hinh = collection["Product_Image"];
            var E_TL = collection["Product_Weight"];
            var E_Category = collection["Category_ID"];
            var E_Unit = collection["Unit_ID"];
            var E_Providers = collection["Providers_ID"];
            var E_Publisher = collection["Publisher_ID"];
            var E_infor = collection["Product_Infor"];
            var check = collection["CheckState"];
            var E_tacgia = collection["tacgia"];
            var E_namviet = collection["namviet"];
            if (string.IsNullOrEmpty(E_tensach) || string.IsNullOrEmpty(E_Gia) ||
                string.IsNullOrEmpty(E_Hinh) || string.IsNullOrEmpty(E_TL) || string.IsNullOrEmpty(E_Category) || string.IsNullOrEmpty(E_Unit) || string.IsNullOrEmpty(E_Providers) ||
                string.IsNullOrEmpty(E_Publisher) || string.IsNullOrEmpty(E_SL))
            {
                ViewBag.ThongBao = "Vui lòng nhập đầy đủ thông tin!";
            }
            else
            {

                if (double.Parse(E_Gia) < 0)
                {
                    ViewBag.ThongBao = "Vui lòng nhập GIÁ hợp lệ!";
                }
                if (int.Parse(E_SL) < 0)
                {
                    ViewBag.ThongBao = "Vui lòng nhập SỐ LƯỢNG hợp lệ!";
                }
                if (int.Parse(E_TL) < 0)
                {
                    ViewBag.ThongBao = "Vui lòng nhập TRỌNG LƯỢNG hợp lệ!";
                }
                if(double.Parse(E_Gia) >= 0 && int.Parse(E_SL) >= 0 && int.Parse(E_TL) >= 0 )
                {
                    s.Product_Name = E_tensach.Trim();
                    s.Product_Image = E_Hinh;
                    s.Product_SellPrice = double.Parse(E_Gia);
                    s.Product_Quantity = int.Parse(E_SL);
                    s.Product_isBook = true;
                    if (check == "choban")
                    {
                        s.Product_State = true;
                    }
                    if (check == "khongchoban")
                    {
                        s.Product_State = false;
                    }

                    s.Product_Weight = int.Parse(E_TL);
                    s.Product_Infor = E_infor;
                    s.Category_ID = int.Parse(E_Category);
                    s.Unit_ID = int.Parse(E_Unit);
                    s.Providers_ID = int.Parse(E_Providers);
                    s.Publisher_ID = int.Parse(E_Publisher);
                    UpdateModel(s);
                    db.SubmitChanges();
                    if (string.IsNullOrEmpty(E_tacgia) || string.IsNullOrEmpty(E_namviet))
                    { }
                    else
                    {
                        var all_tg = db.Authors.Where(x => x.Author_Name.ToLower() == E_tacgia.ToLower().Trim()).ToList();
                        if (all_tg.Count() != 0)
                        {
                            var bw = db.Book_Writters.Where(x => x.Product_ID == id).FirstOrDefault();
                            //bw.Author_ID = all_tg.Where(x => x.Author_Name.ToLower() == E_tacgia.ToLower().Trim()).Select(x => x.Author_ID).FirstOrDefault();
                            //bw.Product_ID = id;
                            bw.Year_Reprint = E_namviet;
                            UpdateModel(bw);

                        }
                        else
                        {
                            //Author au = new Author();
                            //au.Author_Name = E_tacgia;
                            //au.Author_State = true;
                            //db.Authors.InsertOnSubmit(au);
                            //db.SubmitChanges();
                            //var temp_idauthor = au.Author_ID;
                            Book_Writter bw = db.Book_Writters.Where(x => x.Product_ID == id).FirstOrDefault();
                            //bw.Author_ID = temp_idauthor;
                            //bw.Product_ID = id;
                            bw.Year_Reprint = E_namviet;
                            UpdateModel(bw);

                        }
                        db.SubmitChanges();
                    }
                    return RedirectToAction("Sach");
                }
                return this.EditSach(id);
            }
            return this.EditSach(id);
        }
        ////----------------------------------------- 
        public ActionResult DeleteSach(int id)
        {
            var D_sach = db.Products.First(m => m.Product_ID == id);
            Session["ListCategory"] = db.Categories.ToList();
            Session["ListUnit"] = db.Units.ToList();
            Session["ListProviders"] = db.Providers.ToList();
            Session["ListPublisher"] = db.Publishers.ToList();
            var temp = db.Book_Writters.Where(x => x.Product_ID == id).FirstOrDefault();
            if (temp != null)
            {
                Session["Authortemp"] = db.Authors.Where(x => x.Author_ID == temp.Author_ID).FirstOrDefault();
                Session["BWtemp"] = temp;
            }
            if (D_sach.Product_State == true)
                return View(D_sach);
            else
                return RedirectToAction("Sach");
        }
        [HttpPost]
        public ActionResult DeleteSach(int id, FormCollection collection)
        {
            var D_sach = db.Products.Where(m => m.Product_ID == id).First();
            D_sach.Product_State = false;
            UpdateModel(D_sach);
            db.SubmitChanges();
            return RedirectToAction("Sach");
        }
        //````````````````VĂN PHÒNG PHẨM`````````````/
        public ActionResult VPP(int? page, string searchings)
        {
            if (page == null)
                page = 1;
            if (searchings == null)
            {
                var all_sach = db.Products.Where(x => x.Product_isBook == false).ToList().OrderBy(m => m.Product_ID);
                int pageSize = 10;
                int pageNum = page ?? 1;
                return View(all_sach.ToPagedList(pageNum, pageSize));
            }
            else
            {
                var all_sach = db.Products.Where(x => x.Product_isBook == false && x.Product_Name.Contains(searchings)).ToList().OrderBy(m => m.Product_ID);
                int pageSize = 10;
                int pageNum = page ?? 1;
                return View(all_sach.ToPagedList(pageNum, pageSize));
            }
        }
        public ActionResult DetailVPP(int id)
        {
            var D_sach = db.Products.Where(m => m.Product_ID == id).First();
            return View(D_sach);
        }

        public ActionResult CreateVPP()
        {
            var all_category = db.Categories.Where(x => x.Category_State == true && x.Category_isBook == false).ToList();
            ViewBag.VbCategoryList = new SelectList(all_category, "Category_ID", "Category_Name");
            var all_Unit = db.Units.Where(x => x.Unit_State == true).ToList();
            ViewBag.VbUnit = new SelectList(all_Unit, "Unit_ID", "Unit_Name");
            //
            var all_Providers = db.Providers.ToList();
            ViewBag.VbProviders = new SelectList(all_Providers, "Providers_ID", "Providers_Name");
            var all_Publisher = db.Publishers.ToList();
            ViewBag.VbPublisher = new SelectList(all_Publisher, "Publisher_ID", "Publisher_Name");
            return View();
        }
        [HttpPost]
        public ActionResult CreateVPP(FormCollection collection, Product s)
        {
            var E_tensach = collection["Product_Name"];
            var E_Gia = collection["Product_SellPrice"];
            var E_SL = collection["Product_Quantity"];

            var E_Hinh = collection["Product_Image"];
            var E_TL = collection["Product_Weight"];
            var E_Category = collection["Category_ID"];
            var E_Unit = collection["Unit_ID"];
            var E_Providers = collection["Providers_ID"];
            var E_Publisher = collection["Publisher_ID"];
            var E_infor = collection["Product_Infor"];
            if (string.IsNullOrEmpty(E_tensach) || string.IsNullOrEmpty(E_Gia) ||
                string.IsNullOrEmpty(E_Hinh) || string.IsNullOrEmpty(E_TL) || string.IsNullOrEmpty(E_Category) || string.IsNullOrEmpty(E_Unit) || string.IsNullOrEmpty(E_Providers) ||
                string.IsNullOrEmpty(E_Publisher) || string.IsNullOrEmpty(E_SL))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                if (double.Parse(E_Gia) < 0 || int.Parse(E_SL) < 0 || int.Parse(E_TL) < 0)
                {
                    ViewData["Error"] = "Vui lòng nhập lớn hơn 0 !";
                }
                else
                {
                    s.Product_Name = E_tensach.Trim();
                    s.Product_Image = E_Hinh;
                    s.Product_SellPrice = double.Parse(E_Gia);
                    s.Product_Quantity = int.Parse(E_SL);
                    s.Product_isBook = false;
                    s.Product_State = true;
                    s.Product_Weight = int.Parse(E_TL);
                    s.Product_Infor = E_infor;
                    s.Category_ID = int.Parse(E_Category);
                    s.Unit_ID = int.Parse(E_Unit);
                    s.Providers_ID = int.Parse(E_Providers);
                    s.Publisher_ID = int.Parse(E_Publisher);
                    db.Products.InsertOnSubmit(s);
                    db.SubmitChanges();
                    return RedirectToAction("VPP");
                }
            }
            return this.CreateVPP();
        }

        public ActionResult EditVPP(int id)
        {
            var E_sach = db.Products.First(m => m.Product_ID == id);
            var all_category = db.Categories.Where(x => x.Category_State == true && x.Category_isBook == false).ToList();
            ViewBag.VbCategoryList = new SelectList(all_category, "Category_ID", "Category_Name");
            var all_Unit = db.Units.Where(x => x.Unit_State == true).ToList();
            ViewBag.VbUnit = new SelectList(all_Unit, "Unit_ID", "Unit_Name");
            //
            var all_Providers = db.Providers.ToList();
            ViewBag.VbProviders = new SelectList(all_Providers, "Providers_ID", "Providers_Name");
            var all_Publisher = db.Publishers.ToList();
            ViewBag.VbPublisher = new SelectList(all_Publisher, "Publisher_ID", "Publisher_Name");
            return View(E_sach);
        }
        [HttpPost]
        public ActionResult EditVPP(int id, FormCollection collection)
        {
            var s = db.Products.First(m => m.Product_ID == id);

            var E_tensach = collection["Product_Name"];
            var E_Gia = collection["Product_SellPrice"];
            var E_SL = collection["Product_Quantity"];
            var E_Hinh = collection["Product_Image"];
            var E_TL = collection["Product_Weight"];
            var E_Category = collection["Category_ID"];
            var E_Unit = collection["Unit_ID"];
            var E_Providers = collection["Providers_ID"];
            var E_Publisher = collection["Publisher_ID"];
            var E_infor = collection["Product_Infor"];
            var check = collection["CheckState"];

            if (string.IsNullOrEmpty(E_tensach) || string.IsNullOrEmpty(E_Gia) ||
                string.IsNullOrEmpty(E_Hinh) || string.IsNullOrEmpty(E_TL) || string.IsNullOrEmpty(E_Category) || string.IsNullOrEmpty(E_Unit) || string.IsNullOrEmpty(E_Providers) ||
                string.IsNullOrEmpty(E_Publisher) || string.IsNullOrEmpty(E_SL))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                if (double.Parse(E_Gia) < 0 || int.Parse(E_SL) < 0 || int.Parse(E_TL) < 0)
                {
                    ViewData["Error"] = "Vui lòng nhập lớn hơn 0 !";
                }
                else
                {
                    s.Product_Name = E_tensach.Trim();
                    s.Product_Image = E_Hinh;
                    s.Product_SellPrice = double.Parse(E_Gia);
                    s.Product_Quantity = int.Parse(E_SL);
                    s.Product_isBook = false;
                    if (check == "choban")
                    {
                        s.Product_State = true;
                    }
                    if (check == "khongchoban")
                    {
                        s.Product_State = false;
                    }

                    s.Product_Weight = int.Parse(E_TL);
                    s.Product_Infor = E_infor;
                    s.Category_ID = int.Parse(E_Category);
                    s.Unit_ID = int.Parse(E_Unit);
                    s.Providers_ID = int.Parse(E_Providers);
                    s.Publisher_ID = int.Parse(E_Publisher);
                    UpdateModel(s);
                    db.SubmitChanges();
                    return RedirectToAction("VPP");
                }
            }
            return this.EditVPP(id);
        }
        ////----------------------------------------- 
        public ActionResult DeleteVPP(int id)
        {
            var D_sach = db.Products.First(m => m.Product_ID == id);
            Session["ListCategory"] = db.Categories.ToList();
            Session["ListUnit"] = db.Units.ToList();
            Session["ListProviders"] = db.Providers.ToList();
            Session["ListPublisher"] = db.Publishers.ToList();
            var temp = db.Book_Writters.Where(x => x.Product_ID == id).FirstOrDefault();
            if (D_sach.Product_State == true)
                return View(D_sach);
            else
                return RedirectToAction("VPP");
        }
        [HttpPost]
        public ActionResult DeleteVPP(int id, FormCollection collection)
        {
            var D_sach = db.Products.Where(m => m.Product_ID == id).First();
            D_sach.Product_State = false;
            UpdateModel(D_sach);
            db.SubmitChanges();
            return RedirectToAction("VPP");
        }
    }
}