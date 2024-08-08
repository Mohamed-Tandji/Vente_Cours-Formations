using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrjCs_Sprint_1_Login_et_Register.Models
{
    public class Cours
    {
        public int Id { get; set; }
        public string Titre { get; set; }
        public string Num_Cours { get; set; }
        public string Description { get; set;}
        public double Prix { get; set; }
        public string Url { get; set;}
        public int Id_niveau { get; set; }
    }
}