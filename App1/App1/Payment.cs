using System;
using System.Collections.Generic;
using System.Text;

namespace App1
{
    internal class Payment
    {
        public int id_cobro {  get; set; }
        public int id_cliente { get; set; }
        public string numero_factura { get; set; }
        public string concepto { get; set; }
        public string fecha_cobro { get; set; }
        public double total { get; set; }
        public List<DetailPayment> detalles { get; set; }

    }
}
