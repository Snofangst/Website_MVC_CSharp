using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Reflection;


namespace BTLONLTWEB.Models
{
    public class IsProductNameUnique : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return false;
            else
            {
                QuanLyBanHangEntities DB = new QuanLyBanHangEntities();
                if (DB.Products.ToList().Exists(p => p.NameProduct.Trim() == value.ToString().Trim()) == true)
                {
                    return false;
                }
                return true;
            }
        }
    }
    public class IsPriceValid : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult("Vui lòng nhập giá sản phẩm");
            else
            {
               
                double x;
                var price = double.TryParse(value.ToString().Replace(".", string.Empty), out x);
                if (price == true)
                {
                    if (x < 1000.0)
                        return new ValidationResult("Vui lòng nhập giá trên 1.000 vnđ");
                    else
                        return ValidationResult.Success;
                }
                else
                    return new ValidationResult("Giá phải là số!");
            }
        }
    }
    public class IsImageValid : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult("Vui lòng chọn hình ảnh");
            else
            {
                return ValidationResult.Success;
            }
        }
    }
    public class Add_Product
    {
        public string IdProduct { get; set; }
        [IsProductNameUnique(ErrorMessage="Tên sản phẩm đã tồn tại")]
        [Required(ErrorMessage = "Vui lòng tên sản phẩm")]
        public string NameProduct { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn Catalog")]
        public string IdCatalog { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn thương hiệu")]
        public string IdBrand { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn giá")]
        [IsPriceValid(ErrorMessage = "Vui lòng chọn thương hiệu")]
        public string Price { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn số lượng sản phẩm")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Số lượng sản phẩm phải là số")]
        [Range(0, double.MaxValue, ErrorMessage = "Số lượng sản phải lớn hơn 0")]

        public int Quantity { get; set; }

        [Range(0.0, 1.0, ErrorMessage = "Giảm giá chỉ được nằm trong khoảng từ 0 đến 1")]
        public double Discount { get; set; }
      
        public string Description { get; set; }
        public string State { get; set; }
        [IsImageValid]
        public string Image { get; set; }
        public string[] SlideImage { get; set; }
    }
}