using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mid_Project.Models
{
    public class ConversionHistory
    {
        public double InputValue { get; set; }
        public string FromUnit { get; set; }
        public string ToUnit { get; set; }
        public double ResultValue { get; set; }
        public DateTime Timestamp { get; set; }

    }
}
