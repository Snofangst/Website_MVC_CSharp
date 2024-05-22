using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BTLONLTWEB.Models
{
    public class CheckPhoneNumberUnique : ValidationAttribute
    {
        public string IDUser { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo ID = validationContext.ObjectType.GetProperty(IDUser);
            if (value == null)
            {
                return ValidationResult.Success;
            }
            else
            {
                var id = ID.GetValue(validationContext.ObjectInstance).ToString();
                QuanLyBanHangEntities DB = new QuanLyBanHangEntities();
                User user = DB.Users.ToList().Where(p => p.IdUser == id).FirstOrDefault();
                if (user.Member != "NONE" &&(user.PhoneNumber!=null&&user.PhoneNumber.Trim()!=value.ToString())&&DB.Users.ToList().Exists(P => P.PhoneNumber != null && P.PhoneNumber.Trim() == value.ToString()))
                {
                    return new ValidationResult("Số điện thoại đã được đăng ký!");

                }
                else if (user.Member != "NONE" && DB.Users.ToList().Exists(P => P.PhoneNumber != null && P.PhoneNumber.Trim() == value.ToString() && P.IdUser != id))
                {
                    return new ValidationResult("Số điện thoại đã được đăng ký!");

                }
                else
                    return ValidationResult.Success;
            }
        }
    }
    public class Receiver_Form
    {
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại!")]
        public string OrderID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại!")]
        public string UserID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên!")]
        public string Name{get;set;}

        [CheckPhoneNumberUnique(IDUser="UserID")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại!")]
        [MinLength(10, ErrorMessage = "Số điện thoại dài 10 kí tự")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Đây không phải số điện thoại")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ!")]
        public string Address { get; set; }
        public string Delivery{ get; set; }
    }
}