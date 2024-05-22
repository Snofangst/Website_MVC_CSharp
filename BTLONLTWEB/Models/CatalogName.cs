using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace BTLONLTWEB.Models
{
    public class CheckCatalogName: ValidationAttribute
    {
        public string IDCatalog { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo ID = validationContext.ObjectType.GetProperty(IDCatalog);
            var id = ID.GetValue(validationContext.ObjectInstance).ToString();
            QuanLyBanHangEntities DB = new QuanLyBanHangEntities();
            if (DB.Catalogs.ToList().Exists(p => p.IdCatalog.Trim() != id.ToString().Trim() &&(value != null &&p.NameCatalog == value.ToString().Trim())) == false)
            {
                if (DB.Catalogs.ToList().Exists(p => p.IdCatalog.Trim() == id.ToString().Trim() && p.NameCatalog != value.ToString().Trim()) == true)
                    return ValidationResult.Success;
                else
                    if (DB.Catalogs.ToList().Exists(p => p.IdCatalog.Trim() == id.ToString().Trim() && p.NameCatalog == value.ToString().Trim()) == true)
                        return ValidationResult.Success;
                 else if(DB.Catalogs.ToList().Exists(p=>p.IdCatalog.Trim()==id.ToString().Trim())==false)
                    return ValidationResult.Success;
                return new ValidationResult("Tên Catalog đã tồn tại!");
            }
            else
                    return new ValidationResult("Vui lòng điền thông tin Catolog!");
        }
    }
    public class CatalogName
    {
        public string ID { get; set; }
        [CheckCatalogName(IDCatalog="ID")]
        public string Name { get; set; }
    }
}