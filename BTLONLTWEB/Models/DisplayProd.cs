using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTLONLTWEB.Models
{
    public class DisplayProd
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public double Discount { get; set; }
        public DisplayProd(string ID,string Name,string Path,double Price, string Description,double Discount)
        {
            this.ID = ID;
            this.Name = Name;
            this.Path = Path;
            this.Price = Price;
            this.Description = Description;
            this.Discount = Discount;
        }
        public DisplayProd()
        {

        }
    }
}