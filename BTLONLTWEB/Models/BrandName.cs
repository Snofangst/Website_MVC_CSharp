using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace BTLONLTWEB.Models
{
    public class CheckBrandName : ValidationAttribute
    {
        public string IDBrand { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo ID = validationContext.ObjectType.GetProperty(IDBrand);
            var id = ID.GetValue(validationContext.ObjectInstance).ToString();
            QuanLyBanHangEntities DB = new QuanLyBanHangEntities();
            if (DB.Brands.ToList().Exists(p => p.IdBrand.Trim() != id.ToString().Trim() &&(value != null &&p.NameBrand == value.ToString().Trim())) == false)
            {
                if (DB.Brands.ToList().Exists(p => p.IdBrand.Trim() == id.ToString().Trim() && p.NameBrand != value.ToString().Trim()) == true)
                    return ValidationResult.Success;
                else
                    if (DB.Brands.ToList().Exists(p => p.IdBrand.Trim() == id.ToString().Trim() && p.NameBrand == value.ToString().Trim()) == true)
                        return ValidationResult.Success;
                 else if(DB.Brands.ToList().Exists(p=>p.IdBrand.Trim()==id.ToString().Trim())==false)
                    return ValidationResult.Success;
                return new ValidationResult("Tên Brand đã tồn tại!");
            }
            else
                    return new ValidationResult("Vui lòng điền thông tin Brand!");
        }
    }
    public class BrandName
    {
        public string ID { get; set; }
        [CheckBrandName(IDBrand="ID")]
        public string Name { get; set; }
    }
}