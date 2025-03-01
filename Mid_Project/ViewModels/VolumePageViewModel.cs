using CommunityToolkit.Mvvm.ComponentModel;
using Mid_Project.Models;
using System.Collections.ObjectModel;

namespace Mid_Project.ViewModels
{
    public partial class VolumePageViewModel : ObservableObject
    {
        // ObservableCollection for volume units
        public ObservableCollection<Volume> Volumes { get; set; }

        // Property to store the selected "From" unit
        [ObservableProperty]
        public Volume selectedFromVolume;

        // Property to store the selected "To" unit
        [ObservableProperty]
        public Volume selectedToVolume;

        private readonly ConversionService _conversionService;

        public VolumePageViewModel()
        {
            _conversionService = new ConversionService();
            Volumes = new ObservableCollection<Volume>()
            {
                new Volume(){ VolumeID = 1, VolumeName="Milliliter", ConversionFactor = 1 },
                new Volume(){ VolumeID = 2, VolumeName="Liter", ConversionFactor = 1000 },
                new Volume(){ VolumeID = 3, VolumeName="Cubic Centimeter", ConversionFactor = 1 },
                new Volume(){ VolumeID = 4, VolumeName="Cubic Meter", ConversionFactor = 1000000 },
                new Volume(){ VolumeID = 5, VolumeName="Cubic Inch", ConversionFactor = 16.387 },
                new Volume(){ VolumeID = 6, VolumeName="Cubic Foot", ConversionFactor = 28316.8466 },
                new Volume(){ VolumeID = 7, VolumeName="Cubic Yard", ConversionFactor = 764554.858 },
                new Volume(){ VolumeID = 8, VolumeName="US Gallon", ConversionFactor = 3785.41 },
                new Volume(){ VolumeID = 9, VolumeName="UK Gallon", ConversionFactor = 4546.09 }
            };
        }

        // Method to convert between selected volume units
        public double ConvertVolume(double inputValue)
        {
            if (selectedFromVolume == null || selectedToVolume == null)
                return 0;

            double fromFactor = selectedFromVolume.ConversionFactor;
            double toFactor = selectedToVolume.ConversionFactor;

            // Convert to milliliters first, then to the target unit
            double resultInBaseUnit = inputValue * fromFactor;  // Convert input to base unit (mL)
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
