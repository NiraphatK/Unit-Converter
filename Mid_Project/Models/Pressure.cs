using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mid_Project.Models
{
    public class Pressure
    {
        public int PressureID { get; set; }
        public string PressureName { get; set; }
        public double ConversionFactor { get; set; } // Conversion factor relative to Pascals (Pa)
    }
}
