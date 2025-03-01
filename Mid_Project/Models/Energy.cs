using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mid_Project.Models
{
    public class Energy
    {
        public int EnergyID { get; set; }
        public string EnergyName { get; set; }
        public double ConversionFactor { get; set; } // Conversion factor relative to Joules (J)
    }
}
