//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BTLONLTWEB.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Image
    {
        public Image()
        {
            this.ImageDetails = new HashSet<ImageDetail>();
        }
    
        public string IdImage { get; set; }
        public string FileImage { get; set; }
    
        public virtual ICollection<ImageDetail> ImageDetails { get; set; }
    }
}
