using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BTLONLTWEB.Models
{
    public class IsTotalValid : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult("Vui lòng chọn sản phẩm muốn mua!");
            else
            {

                double price = double.Parse(value.ToString());
                if (price < 1000.0)
                    return new ValidationResult("Vui lòng chọn sản phẩm muốn mua!");
                else
                    return ValidationResult.Success;
            }
        }
    }
    public class Receipt
    {
        public string IdOrder { get; set; }
        public string IdUser { get; set; }
        public System.DateTime Date { get; set; }

        [IsTotalValid]
        public double Total { get; set; }
        public string Delivery { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán!")]
        public string Payment { get; set; }
    }
}