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
    
    public partial class Order
    {
        public Order()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
        }
    
        public string IdOrder { get; set; }
        public string IdUser { get; set; }
        public System.DateTime Date { get; set; }
        public Nullable<double> Total { get; set; }
        public string Delivery { get; set; }
        public string Payment { get; set; }
    
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual User User { get; set; }
    }
}
