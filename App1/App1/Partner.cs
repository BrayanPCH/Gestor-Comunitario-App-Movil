using System;
using System.Collections.Generic;
using System.Text;

namespace App1
{
    public class Partner
    {
        public int id_cliente { get; set; }
        public int id_persona { get; set; }
        public string cedula { get; set; }
        public string nombres { get; set; }
        public string apellidos { get; set; }
        public string telefono { get; set; }
        public string correo { get; set; }
        public bool activo { get; set; }
        public string nombres_completos => $"{nombres} {apellidos}";
        public List<Debt> deudas { get; set; }
    }
}
