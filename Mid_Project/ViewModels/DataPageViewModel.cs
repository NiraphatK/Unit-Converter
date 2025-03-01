using CommunityToolkit.Mvvm.ComponentModel;
using Mid_Project.Models;
using System.Collections.ObjectModel;

namespace Mid_Project.ViewModels
{
    public partial class DataPageViewModel : ObservableObject
    {
        public ObservableCollection<Data> Datas { get; set; }

        [ObservableProperty]
        public Data selectedFromData;

        [ObservableProperty]
        public Data selectedToData;

        private readonly ConversionService _conversionService;

        public DataPageViewModel()
        {
            _conversionService = new ConversionService();

            // Define the data units with their conversion factors relative to "Bit"
            Datas = new ObservableCollection<Data>()
            {
                new Data() { DataID = 1, DataName = "Bit", ConversionFactor = 1 },
                new Data() { DataID = 2, DataName = "Byte", ConversionFactor = 8 },
                new Data() { DataID = 3, DataName = "Kilobyte", ConversionFactor = 8 * (long)Math.Pow(1024, 1) },
                new Data() { DataID = 4, DataName = "Megabyte", ConversionFactor = 8 * (long)Math.Pow(1024, 2) },
                new Data() { DataID = 5, DataName = "Gigabyte", ConversionFactor = 8 * (long)Math.Pow(1024, 3) },
                new Data() { DataID = 6, DataName = "Terabyte", ConversionFactor = 8 * (long)Math.Pow(1024, 4) },
                new Data() { DataID = 7, DataName = "Petabyte", ConversionFactor = 8 * (long)Math.Pow(1024, 5) },
                new Data() { DataID = 8, DataName = "Exabyte", ConversionFactor = 8 * (long)Math.Pow(1024, 6) },
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
