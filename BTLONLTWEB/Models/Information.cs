using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace BTLONLTWEB.Models
{
    public class IsNameUnique:ValidationAttribute
    {
        public override bool IsValid(object value)
        {
           if(value==null)
               return false;
           else
           {
                QuanLyBanHangEntities DB = new QuanLyBanHangEntities();
                if (DB.Users.ToList().Exists(p => p.Name.Trim() == value.ToString().Trim()) == true)
                {
                    return false;
                }
                return true;
           }
        }
    }
    public class IsEmailUnique : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
           if(value==null)
               return false;
           else
           {
                QuanLyBanHangEntities DB = new QuanLyBanHangEntities();
                if (DB.Users.ToList().Exists(p =>p.Email!=null&&p.Email.Trim() == value.ToString().Trim())==true)
                {
                    return false;
                }
                return true;
           }
        }
    }
    public class IsPhoneNumberUnique : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return false;
            else
            {
                QuanLyBanHangEntities DB = new QuanLyBanHangEntities();
                if (DB.Users.ToList().Exists(p => p.PhoneNumber.Trim() == value.ToString().Trim()) == true)
                {
                    return false;
                }
                return true;
            }
        }
    }
    public class Information
    {
        
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [IsNameUnique(ErrorMessage = "Username đã tồn tại")]
        [MinLength(6, ErrorMessage = "Tên đăng nhập dài ít nhất 6 kí tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(5, ErrorMessage = "Mật khẩu dài ít nhất 5 kí tự")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Nhập xác định mật khẩu!")]
        [Compare("Password", ErrorMessage = "Mật khẩu không trùng khớp!")]
        public string ConfirmedPas { get; set; }
        
        //[IsPhoneNumberUnique(ErrorMessage = "Số điện thoại đã được đăng ký")]
        //[Required(ErrorMessage = "Vui lòng số điện thoại")]
        //[MinLength(10, ErrorMessage = "Số điện thoại dài 10 kí tự")]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Đây không phải số điện thoại")]
        //public string PhoneNumber { get; set; }

        [IsEmailUnique(ErrorMessage = "Email đã được đăng ký")]
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        public string Gender { get; set; }
    }
}