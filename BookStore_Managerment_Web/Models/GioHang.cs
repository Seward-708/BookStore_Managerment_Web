using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace BookStore_Managerment_Web.Models
{
    public class GioHang
    {
        BookStoreDataContext db = new BookStoreDataContext();

        public int Product_ID { get; set; }

        [Display(Name = "Tên sản phẩm")]
        public string Product_Name { get; set; }

        [Display(Name = "Hình ảnh")]
        public string Product_Image { get; set; }


        [Display(Name = "Giá bán")]
        public Double Cart_Detail_Price { get; set; }

        [Display(Name = "Số lượng")]
        public int Cart_Detail_Quantity { get; set; }

        [Display(Name = "Thành Tiền")]
        public double TotalMoney {

            get { return Cart_Detail_Quantity * Cart_Detail_Price; } }

        public GioHang(int id)
        {
            Product_ID = id;
            Product sach = db.Products.FirstOrDefault(p => p.Product_ID == id);
            Product_Name = sach.Product_Name;
            Product_Image = sach.Product_Image;
            Cart_Detail_Price = double.Parse(sach.Product_SellPrice.ToString());
            Cart_Detail_Quantity = 1;

        }
    }

}