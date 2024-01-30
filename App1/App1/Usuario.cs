using System;
using System.Collections.Generic;
using System.Text;

namespace App1
{
    internal class Usuario
    {
        public int Id_Usuario { get; set; }
        public int Id_Persona { get; set; }
        public string usuario { get; set; }
        public string password { get; set; }
        public string token { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public List<Partner> data { get; set; }
    }
}
