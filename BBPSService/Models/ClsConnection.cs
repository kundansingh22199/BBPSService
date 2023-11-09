using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBPSService.Models
{
    public class ClsConnection
    {
        public string strcon = @"Data Source=103.48.51.217,1232;DataBase=KundanWts;User Id=sa;Password=ffbj*2hFWn#2sn3@dd";
        public string SqlConn()
        {
            return strcon;
        }
    }
}