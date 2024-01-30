using System;
using System.Collections.Generic;
using System.Text;

namespace App1
{
    internal class DetailPayment
    {
        public int id_detalle_cobro {  get; set; }
        public int id_cobro { get; set; }
        public string detalle {  get; set; }
        public string monto { get; set; }
    }
}
