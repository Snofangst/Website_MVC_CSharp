using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BTLONLTWEB.Models
{

    public class CheckEmailandUsername : ValidationAttribute
    {
        public string Textbox2 { get; set; }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo TextBox2 = validationContext.ObjectType.GetProperty(Textbox2);
            
            if (value != null)
            {
                var txtbox = TextBox2.GetValue(validationContext.ObjectInstance).ToString();
                QuanLyBanHangEntities DB = new QuanLyBanHangEntities();
                User user = DB.Users.ToList().Where(p => p.Name.Trim() == txtbox && p.Email != null && p.Email.Trim() == value.ToString() && p.TypeUser.Trim() != "Admin").FirstOrDefault();

                if (user == null)
                    return new ValidationResult("Email không trùng khớp!");
                else
                    return ValidationResult.Success;
            }
            else
                return new ValidationResult("Vui lòng nhập Email");
        }
    }
    public class CheckUsernameWithID : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value != null)
            {
                QuanLyBanHangEntities DB = new QuanLyBanHangEntities();
                User user = DB.Users.ToList().Where(p => p.Name.Trim() == value.ToString().Trim()&&p.TypeUser.Trim()!="Admin").FirstOrDefault();

                if (user == null)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }
    }
    public class RestorePassword
    {
       
        [Required(ErrorMessage="Vui lòng nhập Username!")]
        [CheckUsernameWithID(ErrorMessage="Username không tồn tại!")]

        public string Username { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Email!")]
        [CheckEmailandUsername(Textbox2 = "Username")]
        public string Email { get; set; }
    }
}