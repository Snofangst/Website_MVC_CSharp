using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTLONLTWEB.Models
{
    public class ProductDetails
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double SubTotal { get; set; }
        public ProductDetails(string ID,string Name,double Price,int Quantity,double SubTotal)
        {
            this.ID = ID;
            this.Name = Name;
            this.Price = Price;
            this.Quantity = Quantity;
            this.SubTotal = SubTotal;
        }
        public ProductDetails()
        {

        }
    }
}