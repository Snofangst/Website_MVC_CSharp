using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTLONLTWEB.Models
{
    public class Function
    {
        public string PrintNull(string text)
        {
            if (text == null)
                return "NULL";
            else
                return text;
        }
        public string PrintTotal(string text)
        {
            if (text == "")
                return "0";
            else
                return text;
        }
        public string PrintBan(string text)
        {
            if (text == "BANNED")
                return "Unban";
            else
                return "Ban";
        }
        public string PrintGender(string text)
        {
            if (text.Trim() == "F")
                return "NỮ";
            else
                if (text.Trim() == "M")
                    return "NAM";
                else
                    return "KHÔNG XÁC ĐỊNH";
        }
    }
}