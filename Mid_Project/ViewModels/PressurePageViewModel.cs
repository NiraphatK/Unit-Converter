using CommunityToolkit.Mvvm.ComponentModel;
using Mid_Project.Models;
using System.Collections.ObjectModel;

namespace Mid_Project.ViewModels
{
    public partial class PressurePageViewModel : ObservableObject
    {
        // ObservableCollection for pressure units
        public ObservableCollection<Pressure> Pressures { get; set; }

        // Property to store the selected "From" unit
        [ObservableProperty]
        public Pressure selectedFromPressure;

        // Property to store the selected "To" unit
        [ObservableProperty]
        public Pressure selectedToPressure;

        private readonly ConversionService _conversionService;

        public PressurePageViewModel()
        {
            _conversionService = new ConversionService();
            Pressures = new ObservableCollection<Pressure>()
            {
                new Pressure(){ PressureID = 1, PressureName="Pascal", ConversionFactor = 1 },
                new Pressure(){ PressureID = 2, PressureName="Bar", ConversionFactor = 100000 }, // 1 Bar = 100,000 Pa
                new Pressure(){ PressureID = 3, PressureName="Pound per square inch (psi)", ConversionFactor = 6894.76 }, // 1 psi ≈ 6894.76 Pa
                new Pressure(){ PressureID = 4, PressureName="Millimeter of mercury (mmHg)", ConversionFactor = 133.322 }, // 1 mmHg ≈ 133.322 Pa
                new Pressure(){ PressureID = 5, PressureName="Atmosphere (atm)", ConversionFactor = 101325 }, // 1 atm = 101,325 Pa
            };
        }

        // Method to convert between selected pressure units
        public double ConvertPressure(double inputValue)
        {
            if (selectedFromPressure == null || selectedToPressure == null)
                return 0;

            double fromFactor = selectedFromPressure.ConversionFactor;
            double toFactor = selectedToPressure.ConversionFactor;

            // Convert to Pascals first, then to the target unit
            double resultInBaseUnit = inputValue * fromFactor;  // Convert input to base unit (Pa)
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
