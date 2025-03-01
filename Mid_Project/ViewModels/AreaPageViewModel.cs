using CommunityToolkit.Mvvm.ComponentModel;
using Mid_Project.Models;
using System.Collections.ObjectModel;

namespace Mid_Project.ViewModels
{
    public partial class AreaPageViewModel : ObservableObject
    {
        // ObservableCollection สำหรับเก็บรายการหน่วยพื้นที่
        public ObservableCollection<Area> Areas { get; set; }

        // Property สำหรับเก็บหน่วยที่ผู้ใช้เลือก (From)
        [ObservableProperty]
        public Area selectedFromArea;

        // Property สำหรับเก็บหน่วยที่ผู้ใช้เลือก (To)
        [ObservableProperty]
        public Area selectedToArea;

        private readonly ConversionService _conversionService;

        public AreaPageViewModel()
        {
            _conversionService = new ConversionService();
            Areas = new ObservableCollection<Area>()
            {
                new Area(){ AreaID = 1, AreaName="Square Millimeter", ConversionFactor = 1 },
                new Area(){ AreaID = 2, AreaName="Square Centimeter", ConversionFactor = 100 },
                new Area(){ AreaID = 3, AreaName="Square Meter", ConversionFactor = 1000000 },
                new Area(){ AreaID = 4, AreaName="Square Kilometer", ConversionFactor = 1000000000000 },
                new Area(){ AreaID = 5, AreaName="Square Inch", ConversionFactor = 645.16 },
                new Area(){ AreaID = 6, AreaName="Square Foot", ConversionFactor = 92903.04 },
                new Area(){ AreaID = 7, AreaName="Square Yard", ConversionFactor = 836127.36 },
                new Area(){ AreaID = 8, AreaName="Square Mile", ConversionFactor = 2589988110.336 },

                // Added Thai Units
                new Area(){ AreaID = 9, AreaName="Rai", ConversionFactor = 1600000 },      // 1 Rai = 1600 m²
                new Area(){ AreaID = 10, AreaName="Ngan", ConversionFactor = 400000 },      // 1 Ngan = 400 m²
                new Area(){ AreaID = 11, AreaName="Square Wah", ConversionFactor = 4000 },  // 1 Square Wah = 4 m²
            };
        }

        // Method to convert between selected area units
        public double ConvertArea(double inputValue)
        {
            if (selectedFromArea == null || selectedToArea == null)
                return 0;

            double fromFactor = selectedFromArea.ConversionFactor;
            double toFactor = selectedToArea.ConversionFactor;

            // Convert to square millimeters first, then to the target unit
            double resultInBaseUnit = inputValue * fromFactor;  // Convert input to base unit (mm²)
            double result = resultInBaseUnit / toFactor;  // Convert base unit to target unit

            return result;
        }

        // Save conversion history (similar to the LengthPageViewModel)
        public void SaveConversionHistory(double inputValue, string fromUnit, string toUnit, double result)
        {
            var history = new ConversionHistory
            {
                InputValue = inputValue,
                FromUnit = fromUnit,
                ToUnit = toUnit,
                ResultValue = Math.Round(result, 10),
                Timestamp = DateTime.Now
            };

            _conversionService.SaveHistoryToFile(history);
        }
    }
}
