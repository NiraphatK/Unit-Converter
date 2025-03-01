using CommunityToolkit.Mvvm.ComponentModel;
using Mid_Project.Models;
using System.Collections.ObjectModel;

namespace Mid_Project.ViewModels
{
    public partial class SpeedPageViewModel : ObservableObject
    {
        // ObservableCollection for speed units
        public ObservableCollection<Speed> Speeds { get; set; }

        // Property to store the selected "From" unit
        [ObservableProperty]
        public Speed selectedFromSpeed;

        // Property to store the selected "To" unit
        [ObservableProperty]
        public Speed selectedToSpeed;

        private readonly ConversionService _conversionService;

        public SpeedPageViewModel()
        {
            _conversionService = new ConversionService();
            Speeds = new ObservableCollection<Speed>()
            {
                new Speed(){ SpeedID = 1, SpeedName="Meter per second", ConversionFactor = 1 },
                new Speed(){ SpeedID = 2, SpeedName="Kilometer per hour", ConversionFactor = 5.0 / 18 }, // 1 km/h = 5/18 m/s
                new Speed(){ SpeedID = 3, SpeedName="Miles per hour", ConversionFactor = 0.44704 }, // 1 mph = 0.44704 m/s
                new Speed(){ SpeedID = 4, SpeedName="Knots", ConversionFactor = 0.514444 }, // 1 knot = 0.514444 m/s
            };
        }

        // Method to convert between selected speed units
        public double ConvertSpeed(double inputValue)
        {
            if (selectedFromSpeed == null || selectedToSpeed == null)
                return 0;

            double fromFactor = selectedFromSpeed.ConversionFactor;
            double toFactor = selectedToSpeed.ConversionFactor;

            // Convert to meters per second first, then to the target unit
            double resultInBaseUnit = inputValue * fromFactor;  // Convert input to base unit (m/s)
            double result = resultInBaseUnit / toFactor;  // Convert base unit to target unit

            return result;
        }

        // Save conversion history (similar to the previous models)
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
