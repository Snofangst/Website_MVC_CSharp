using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BTLONLTWEB.Models
{
    public class Login
    {
        public class IsUserExist : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                if (value == null)
                    return false;
                else
                {
                    QuanLyBanHangEntities DB = new QuanLyBanHangEntities();
                    User User =DB.Users.ToList().Where(p => p.Name.Trim() == value.ToString().Trim() || (p.Email != null && p.Email.Trim() == value.ToString().Trim())).FirstOrDefault();
                    if (User == null)
                        return false;
                    else if (User.Member == "BANNED")
                        return false;
                    return true;
                }
            }
        }
       
        [Required(ErrorMessage = "Vui lòng tên đăng nhập")]
        [IsUserExist(ErrorMessage = "Username không tồn tại")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string Password { get; set; }
    }
}