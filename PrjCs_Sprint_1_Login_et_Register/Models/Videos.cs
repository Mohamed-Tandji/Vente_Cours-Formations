using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrjCs_Sprint_1_Login_et_Register.Models
{
    public class Videos
    {
        public int Id { get; set; }
        public string Titre { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Img { get; set; }
        public int Id_niveau { get; set; }
    }
}