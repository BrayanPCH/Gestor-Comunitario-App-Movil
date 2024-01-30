using System;
using System.Collections.Generic;
using System.Text;

namespace App1
{
    public class Debt
    {
        public int id_cliente {  get; set; }
        public string monto {  get; set; }
        public string fecha { get; set; }
        public string nombre { get; set; }
        public string tipo { get; set; }
        public bool pagado { get; set; }

    }
}
