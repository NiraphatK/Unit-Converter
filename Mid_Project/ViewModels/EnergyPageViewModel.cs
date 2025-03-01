using CommunityToolkit.Mvvm.ComponentModel;
using Mid_Project.Models;
using System.Collections.ObjectModel;

namespace Mid_Project.ViewModels
{
    public partial class EnergyPageViewModel : ObservableObject
    {
        // ObservableCollection for energy units
        public ObservableCollection<Energy> Energies { get; set; }

        // Property to store the selected "From" unit
        [ObservableProperty]
        public Energy selectedFromEnergy;

        // Property to store the selected "To" unit
        [ObservableProperty]
        public Energy selectedToEnergy;

        private readonly ConversionService _conversionService;

        public EnergyPageViewModel()
        {
            _conversionService = new ConversionService();
            Energies = new ObservableCollection<Energy>()
            {
                new Energy(){ EnergyID = 1, EnergyName="Joule", ConversionFactor = 1 },
                new Energy(){ EnergyID = 2, EnergyName="Kilojoule", ConversionFactor = 1000 }, // 1 kJ = 1000 J
                new Energy(){ EnergyID = 3, EnergyName="Calorie", ConversionFactor = 4.184 }, // 1 cal ≈ 4.184 J
                new Energy(){ EnergyID = 4, EnergyName="Kilocalorie", ConversionFactor = 4184 }, // 1 kcal ≈ 4184 J
                new Energy(){ EnergyID = 5, EnergyName="Watt-hour", ConversionFactor = 3600 }, // 1 Wh = 3600 J
                new Energy(){ EnergyID = 6, EnergyName="Kilowatt-hour", ConversionFactor = 3600000 }, // 1 kWh = 3,600,000 J
            };
        }

        // Method to convert between selected energy units
        public double ConvertEnergy(double inputValue)
        {
            if (selectedFromEnergy == null || selectedToEnergy == null)
                return 0;

            double fromFactor = selectedFromEnergy.ConversionFactor;
            double toFactor = selectedToEnergy.ConversionFactor;

            // Convert to Joules first, then to the target unit
            double resultInBaseUnit = inputValue * fromFactor;  // Convert input to base unit (J)
            double result = resultInBaseUnit / toFactor;  // Convert base unit to target unit

            return result;
        }

        // Save conversion history (similar to previous models)
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
