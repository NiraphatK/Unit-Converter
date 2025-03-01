using CommunityToolkit.Mvvm.ComponentModel;
using Mid_Project.Models;
using System.Collections.ObjectModel;

namespace Mid_Project.ViewModels
{
    public partial class TempPageViewModel : ObservableObject
    {
        // ObservableCollection สำหรับเก็บรายการหน่วยวัดอุณหภูมิ
        public ObservableCollection<Temp> TemperatureUnits { get; set; }

        // Property สำหรับเก็บหน่วยที่ผู้ใช้เลือก (From)
        [ObservableProperty]
        public Temp selectedFromTemp;

        // Property สำหรับเก็บหน่วยที่ผู้ใช้เลือก (To)
        [ObservableProperty]
        public Temp selectedToTemp;

        private readonly ConversionService _conversionService;

        public TempPageViewModel()
        {
            _conversionService = new ConversionService();

            // กำหนดค่าเริ่มต้นสำหรับรายการหน่วยอุณหภูมิ
            TemperatureUnits = new ObservableCollection<Temp>()
            {
                new Temp() { TempID = 1, TempName = "Celsius" },
                new Temp() { TempID = 2, TempName = "Fahrenheit" },
                new Temp() { TempID = 3, TempName = "Kelvin" },
            };
        }

        // ฟังก์ชันสำหรับการแปลงอุณหภูมิจากหน่วยหนึ่งไปยังอีกหน่วยหนึ่ง
        public double ConvertTemperature(double inputValue, Temp fromUnit, Temp toUnit)
        {
            // ถ้าหน่วยต้นทางและหน่วยปลายทางเหมือนกัน ก็คืนค่าค่าป้อนเข้าไปเลย
            if (fromUnit.TempName == toUnit.TempName)
                return inputValue;

            double result = 0;

            try
            {
                // การแปลงระหว่างหน่วยต่างๆ
                if (fromUnit.TempName == "Celsius" && toUnit.TempName == "Fahrenheit")
                    result = (inputValue * 9 / 5) + 32;
                else if (fromUnit.TempName == "Celsius" && toUnit.TempName == "Kelvin")
                    result = inputValue + 273.15;

                else if (fromUnit.TempName == "Fahrenheit" && toUnit.TempName == "Celsius")
                    result = (inputValue - 32) * 5 / 9;
                else if (fromUnit.TempName == "Fahrenheit" && toUnit.TempName == "Kelvin")
                    result = (inputValue - 32) * 5 / 9 + 273.15;

                else if (fromUnit.TempName == "Kelvin" && toUnit.TempName == "Celsius")
                    result = inputValue - 273.15;
                else if (fromUnit.TempName == "Kelvin" && toUnit.TempName == "Fahrenheit")
                    result = (inputValue - 273.15) * 9 / 5 + 32;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                result = 0;
            }

            return result;
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

            // บันทึกข้อมูลประวัติลงในไฟล์
            _conversionService.SaveHistoryToFile(history);

        }
    }
}

