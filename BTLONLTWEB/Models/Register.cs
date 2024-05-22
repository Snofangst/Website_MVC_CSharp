using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTLONLTWEB.Models
{
    public class Register:BTLONLTWEB.Models.User
    {
        public User Regi(string id,string name, string email,string number,string pass,string gender,string address, string member, double total,string type,string avatar)
        {
            User a=new User();
            a.IdUser = id;
            a.Name = name;
            a.Email = email;
            a.PhoneNumber = number;
            a.Password = pass;
            a.Gender = gender;
            a.Address = address;
            a.Member = member;
            a.TotalSpent = total;
            a.TypeUser = type;
            a.IdAvatar = avatar;
            return a;
        }
    }
}