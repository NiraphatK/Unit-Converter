using CommunityToolkit.Mvvm.ComponentModel;
using Mid_Project.Models;
using System.Collections.ObjectModel;

namespace Mid_Project.ViewModels
{
    public partial class LengthPageViewModel : ObservableObject
    {
        // ObservableCollection สำหรับเก็บรายการหน่วย
        public ObservableCollection<Length> Lengths { get; set; }

        // Property สำหรับเก็บหน่วยที่ผู้ใช้เลือก (From)
        [ObservableProperty]
        public Length selectedFromLength;

        // Property สำหรับเก็บหน่วยที่ผู้ใช้เลือก (To)
        [ObservableProperty]
        public Length selectedToLength;

        private readonly ConversionService _conversionService;
        public LengthPageViewModel()
        {
            _conversionService = new ConversionService();
            Lengths = new ObservableCollection<Length>()
            {
                new Length(){ LengthID = 1, LengthName="Millimeter", ConversionFactor = 1 },
                new Length(){ LengthID = 2, LengthName="Centimeter", ConversionFactor = 10 },
                new Length(){ LengthID = 3, LengthName="Meter", ConversionFactor = 1000 },
                new Length(){ LengthID = 4, LengthName="Kilometer", ConversionFactor = 1000000 },
                new Length(){ LengthID = 5, LengthName="Inch", ConversionFactor = 25.4 },
                new Length(){ LengthID = 6, LengthName="Foot", ConversionFactor = 304.8 },
                new Length(){ LengthID = 7, LengthName="Yard", ConversionFactor = 914.4 },
                new Length(){ LengthID = 8, LengthName="Mile", ConversionFactor = 1609344 },
            };
        }
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
