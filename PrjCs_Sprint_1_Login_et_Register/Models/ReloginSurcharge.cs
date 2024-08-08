using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrjCs_Sprint_1_Login_et_Register.Models
{
    public class ReloginSurcharge
    {
        public string Statut { get; set; }
        public int id_eleves { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}