using CommunityToolkit.Mvvm.ComponentModel;
using Mid_Project.Models;
using System.Collections.ObjectModel;

namespace Mid_Project.ViewModels
{
    public partial class TimePageViewModel : ObservableObject
    {
        // ObservableCollection for time units
        public ObservableCollection<Time> Times { get; set; }

        // Property to store the selected "From" unit
        [ObservableProperty]
        public Time selectedFromTime;

        // Property to store the selected "To" unit
        [ObservableProperty]
        public Time selectedToTime;

        private readonly ConversionService _conversionService;

        public TimePageViewModel()
        {
            _conversionService = new ConversionService();
            Times = new ObservableCollection<Time>()
            {
                new Time(){ TimeID = 1, TimeName="Second", ConversionFactor = 1 },
                new Time(){ TimeID = 2, TimeName="Minute", ConversionFactor = 60 }, // 1 minute = 60 seconds
                new Time(){ TimeID = 3, TimeName="Hour", ConversionFactor = 3600 }, // 1 hour = 3600 seconds
                new Time(){ TimeID = 4, TimeName="Day", ConversionFactor = 86400 }, // 1 day = 86400 seconds
                new Time(){ TimeID = 5, TimeName="Week", ConversionFactor = 604800 }, // 1 week = 604800 seconds
                new Time(){ TimeID = 6, TimeName="Month", ConversionFactor = 2592000 }, // 1 month = 2592000 seconds (assuming 30 days per month)
                new Time(){ TimeID = 7, TimeName="Year", ConversionFactor = 31536000 } // 1 year = 31536000 seconds (assuming 365 days per year)
            };
        }

        // Method to convert between selected time units
        public double ConvertTime(double inputValue)
        {
            if (selectedFromTime == null || selectedToTime == null)
                return 0;

            double fromFactor = selectedFromTime.ConversionFactor;
            double toFactor = selectedToTime.ConversionFactor;

            // Convert to seconds first, then to the target unit
            double resultInBaseUnit = inputValue * fromFactor;  // Convert input to base unit (seconds)
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
