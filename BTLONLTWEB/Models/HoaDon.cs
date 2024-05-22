using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTLONLTWEB.Models
{
    public class HoaDon
    {
        public string IdHoaDon { get; set; }
        public string IdUser { get; set; }
        public double Total { get; set; }
        public DateTime Date { get; set; }
    }
}