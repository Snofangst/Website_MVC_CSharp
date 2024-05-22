using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTLONLTWEB.Models
{
    public class Details
    {
        public string UserID { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string IdOrder { get; set; }
        public DateTime Date { get; set; }
        public string Payment { get; set; }
        public string Delivery { get; set; }
        public double Total { get; set; }
        public Details(string UserID, string Name, string PhoneNumber, string Address, string IdOrder, string Payment, string Delivery, double Total, DateTime Date)
        {
            this.UserID = UserID;
            this.Name = Name;
            this.PhoneNumber = PhoneNumber;
            this.Address = Address;
            this.IdOrder = IdOrder;
            this.Payment = Payment;
            this.Delivery = Delivery;
            this.Total = Total;
            this.Date = Date;
        }
        public Details()
        {

        }
    }
}