using CommunityToolkit.Mvvm.ComponentModel;
using Mid_Project.Models;
using System.Collections.ObjectModel;

namespace Mid_Project.ViewModels
{
    public partial class FrequencyPageViewModel : ObservableObject
    {
        // ObservableCollection for storing frequency units
        public ObservableCollection<Frequency> Frequencies { get; set; }

        // Property for the selected 'From' unit
        [ObservableProperty]
        public Frequency selectedFromFrequency;

        // Property for the selected 'To' unit
        [ObservableProperty]
        public Frequency selectedToFrequency;

        private readonly ConversionService _conversionService;

        public FrequencyPageViewModel()
        {
            _conversionService = new ConversionService();
            Frequencies = new ObservableCollection<Frequency>()
            {
                new Frequency(){ FrequencyID = 1, FrequencyName="Hertz", ConversionFactor = 1 },
                new Frequency(){ FrequencyID = 2, FrequencyName="Kilohertz", ConversionFactor = 1000 },
                new Frequency(){ FrequencyID = 3, FrequencyName="Megahertz", ConversionFactor = 1000000 },
                new Frequency(){ FrequencyID = 4, FrequencyName="Gigahertz", ConversionFactor = 1000000000 },
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
