
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace BTLONLTWEB.Models
{
    
    public class ImagePath
    {
        [Required(ErrorMessage = "Vui lòng chọn hình ảnh cần thêm vào!")]
        public string Path { get; set; }
    }
}