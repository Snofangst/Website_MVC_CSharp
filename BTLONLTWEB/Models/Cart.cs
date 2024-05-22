using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BTLONLTWEB.Models
{
    public class IsQuantityValid : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult("Vui lòng nhập số lượng sản phẩm");
            else
            {

                int x;
                var price = int.TryParse(value.ToString().Replace(".", string.Empty), out x);
                if (price == true)
                {
                    if (x <=0)
                        return new ValidationResult("Vui lòng chọn số lượng lớn hơn 0!");
                    else
                        return ValidationResult.Success;
                }
                else
                    return new ValidationResult("Giá phải là số!");
            }
        }
    }
    public class Cart
    {
        public string IDUser { get; set; }
        public string IDOrder { get; set; }
        public string IDProduct { get; set; }
        [Required(ErrorMessage="Vui lòng chọn số lượng sản phẩm muốn mua!")]
        [IsQuantityValid]
        public int Quantity { get; set; }
        public string NameProduct { get; set; }
        public double Price { get; set; }
        public double SubTotal { get; set; }
        public string Image { get; set; }
        public int maxQuantity { get; set; }
    }
}