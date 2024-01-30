using System;
using System.Collections.Generic;
using System.Text;

namespace App1
{
    internal class Sensor
    {
        public string status { get; set; }
        public string message { get; set; }
        public List<Payment> data { get; set; }
    }
}
