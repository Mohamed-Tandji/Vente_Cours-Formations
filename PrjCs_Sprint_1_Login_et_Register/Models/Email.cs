using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace PrjCs_Sprint_1_Login_et_Register.Models
{
    public class Email
    {

        public string To { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }

        
    }
}