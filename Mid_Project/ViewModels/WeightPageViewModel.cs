using CommunityToolkit.Mvvm.ComponentModel;
using Mid_Project.Models;
using System.Collections.ObjectModel;

namespace Mid_Project.ViewModels
{
    public partial class WeightPageViewModel : ObservableObject
    {
        // ObservableCollection สำหรับเก็บรายการหน่วยน้ำหนัก
        public ObservableCollection<Weight> Weights { get; set; }

        // Property สำหรับเก็บหน่วยที่ผู้ใช้เลือก (From)
        [ObservableProperty]
        public Weight selectedFromWeight;

        // Property สำหรับเก็บหน่วยที่ผู้ใช้เลือก (To)
        [ObservableProperty]
        public Weight selectedToWeight;

        private readonly ConversionService _conversionService;
        public WeightPageViewModel()
        {
            _conversionService = new ConversionService();
            Weights = new ObservableCollection<Weight>()
            {
                new Weight(){ WeightID = 1, WeightName="Milligram", ConversionFactor = 0.001 },
                new Weight(){ WeightID = 2, WeightName="Gram", ConversionFactor = 1 },
                new Weight(){ WeightID = 3, WeightName="Kilogram", ConversionFactor = 1000 },
                new Weight(){ WeightID = 4, WeightName="Ton", ConversionFactor = 1000000 },
                new Weight(){ WeightID = 5, WeightName="Ounce", ConversionFactor = 28.3495 },
                new Weight(){ WeightID = 6, WeightName="Pound", ConversionFactor = 453.592 },
                new Weight(){ WeightID = 7, WeightName="Short Ton", ConversionFactor = 907184.74 },
                new Weight(){ WeightID = 8, WeightName="Long Ton", ConversionFactor = 1016046.91 },
            };
        }

        // ฟังก์ชันสำหรับบันทึกประวัติการแปลง
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
