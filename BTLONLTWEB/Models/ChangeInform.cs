using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Web.Mvc;
using System.Reflection;

namespace BTLONLTWEB.Models
{
    public class CheckEmail:ValidationAttribute
    {
        public string IDUser { get; set; }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var emailChecker = new System.ComponentModel.DataAnnotations.EmailAddressAttribute();
            PropertyInfo ID = validationContext.ObjectType.GetProperty(IDUser);
            if(value!=null)
            {
                QuanLyBanHangEntities DB = new QuanLyBanHangEntities();
                var id = ID.GetValue(validationContext.ObjectInstance).ToString();
                if (DB.Users.ToList().Exists(p => p.IdUser == id.ToString() && p.Email.Trim() == value.ToString().Trim()) == true)
                    return ValidationResult.Success;
                else
                {
                    if (DB.Users.ToList().Exists(p => p.IdUser != id.ToString() &&(p.Email!=null&&p.Email.Trim() == value.ToString().Trim())) == true)
                    {
                        return new ValidationResult("Địa chỉ email đã được đăng ký");
                    }
                    else if (emailChecker.IsValid(value.ToString().Trim()) == true)
                        return ValidationResult.Success;
                    else
                        return new ValidationResult("Địa chỉ email không hợp lệ");
                }
            }
            return ValidationResult.Success;
        }
    }
    public class CheckPassword:ValidationAttribute
    {
        public string IDUser { get; set; }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo ID = validationContext.ObjectType.GetProperty(IDUser);
            if(value!=null)
            {
                QuanLyBanHangEntities DB = new QuanLyBanHangEntities();
                var id = ID.GetValue(validationContext.ObjectInstance).ToString();
                if (DB.Users.ToList().Exists(p => p.IdUser == id.ToString() && p.Password == value.ToString()) == true)
                    return ValidationResult.Success;
                return new ValidationResult("Sai mật khẩu!");
            }
            return ValidationResult.Success;
        }
    }
    public class NotEqualAttribute : ValidationAttribute
    {
        public string OldPassword { get; set; }
        public string IDUser { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            
            PropertyInfo Old = validationContext.ObjectType.GetProperty(OldPassword);
            PropertyInfo ID = validationContext.ObjectType.GetProperty(IDUser);
            if (value == null || Old == null)
            {
                return ValidationResult.Success;
            }
            else
            {
                var oldpass = Old.GetValue(validationContext.ObjectInstance).ToString();
                var id = ID.GetValue(validationContext.ObjectInstance).ToString();
                QuanLyBanHangEntities DB = new QuanLyBanHangEntities();
                if(DB.Users.ToList().Exists(p=>p.IdUser.Trim()==id.ToString().Trim()&&p.Password==oldpass.Trim().ToString())==true)
                {
                    if (value.ToString() == oldpass.ToString())
                        return new ValidationResult("Mật khẩu mới phải khác với mật khẩu cũ");
                    else
                        return ValidationResult.Success;
                }
            }
            return null;
        }
    }

    public class CheckUserNameAttribute : ValidationAttribute
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
                if (DB.Users.ToList().Exists(p => p.IdUser.Trim() == id.ToString().Trim() && p.Name == value.ToString().Trim()) == true)
                {
                    if (DB.Users.ToList().Exists(p => p.IdUser.Trim() == id.ToString().Trim() && p.Name != value.ToString().Trim()) == true)
                        return ValidationResult.Success;
                }
                else
                    if (DB.Users.ToList().Exists(p => p.IdUser.Trim() != id.ToString().Trim() && p.Name == value.ToString().Trim()) == true)
                        return new ValidationResult("Username đã tồn tại!");
            }
            return null;
        }
    }
    public class CheckPhoneNumberAttribute : ValidationAttribute
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
                
                if (DB.Users.ToList().Exists(p => p.IdUser.Trim() == id.ToString().Trim() &&p.PhoneNumber!=null&&p.PhoneNumber.Trim() == value.ToString().Trim()) == true)
                {
                    if (DB.Users.ToList().Exists(p => p.IdUser.Trim() == id.ToString().Trim() && p.PhoneNumber.Trim() != value.ToString().Trim()) == true)
                        return ValidationResult.Success;
                }
                else
                    if (DB.Users.ToList().Exists(p => p.IdUser.Trim() != id.ToString().Trim() &&(p.PhoneNumber!=null&& p.PhoneNumber.Trim() == value.ToString().Trim())) == true)
                        return new ValidationResult("Số điện thoại đã được đăng ký!");
            }
            return null;
        }
    }
    public class ChangeInform
    {
        public string ID { get; set; }
        [CheckUserName(IDUser="ID")]
        
        [MinLength(6, ErrorMessage = "Tên đăng nhập dài ít nhất 6 kí tự")]
        public string Name { get; set; }

        [CheckPassword(IDUser="ID")]
        [MinLength(5, ErrorMessage = "Mật khẩu dài ít nhất 5 kí tự")]
        public string OldPassword{ get;set; }
        [NotEqual(OldPassword="OldPassword",IDUser="ID")]
        public string NewPassword { get; set; }

        [CheckPhoneNumber(IDUser="ID")]
        [MinLength(10, ErrorMessage = "Số điện thoại dài 10 kí tự")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Đây không phải số điện thoại")]
        public string PhoneNumber { get; set; }

        [CheckEmail(IDUser="ID")]
        public string Email { get; set; }
        public string Address { get; set; }
        public string Path { get; set; }
    }
}