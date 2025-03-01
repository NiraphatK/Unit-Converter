using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mid_Project.Models
{
    public class Currency
    {
        public int CurrencyID { get; set; }
        public string CurrencyName { get; set; }
        public double ConversionRate { get; set; }
    }
}
