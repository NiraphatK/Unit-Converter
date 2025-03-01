using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mid_Project.Models
{
    public class Speed
    {
        public int SpeedID { get; set; }
        public string SpeedName { get; set; }
        public double ConversionFactor { get; set; } // Conversion factor relative to meters per second
    }
}
