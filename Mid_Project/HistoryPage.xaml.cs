using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace Mid_Project
{
    public partial class HistoryPage : ContentPage
    {
        private readonly ConversionService _conversionService;
        public ObservableCollection<ConversionHistory> HistoryList { get; set; }
        private DateTime _lastFileWriteTime; // เก็บเวลาที่ไฟล์ถูกเขียนครั้งสุดท้าย
        private bool _isDataLoaded; 

        public HistoryPage()
        {
            InitializeComponent();
            _conversionService = new ConversionService();
            HistoryList = new ObservableCollection<ConversionHistory>();
            BindingContext = this; // เชื่อมโยง BindingContext
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // แสดง UI ให้ผู้ใช้ก่อน แล้วค่อยโหลดข้อมูล
            await Task.Delay(100); // หน่วงเวลาสั้น ๆ เพื่อให้หน้าแสดงผล

            // ถ้ายังไม่ได้โหลดข้อมูล หรือมีการเปลี่ยนแปลงในไฟล์
            if (!_isDataLoaded)
            {
                await LoadHistoryAsync();
                _isDataLoaded = true; // ตั้งค่าสถานะให้โหลดข้อมูลแล้ว
            }
            else
            {
                // ตรวจสอบว่าไฟล์มีการแก้ไขใหม่หรือไม่
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "conversion_history.json");
                DateTime lastWriteTime = File.Exists(filePath) ? File.GetLastWriteTime(filePath) : DateTime.MinValue;

                // หากไฟล์มีการแก้ไขใหม่ให้โหลดข้อมูลใหม่
                if (lastWriteTime > _lastFileWriteTime)
                {
                    await LoadHistoryAsync();
                }

                _lastFileWriteTime = lastWriteTime; // อัปเดตเวลาเขียนไฟล์
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(); // แจ้งให้ UI รู้ว่าค่าเปลี่ยน
            }
        }
        // ฟังก์ชันสำหรับโหลดข้อมูลประวัติการแปลงจากไฟล์แบบ Asynchronous
        public async Task LoadHistoryAsync()
        {
            IsLoading = true; // เริ่มแสดง ActivityIndicator  

            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "conversion_history.json");

                // ตรวจสอบว่าไฟล์มีข้อมูลหรือไม่
                if (File.Exists(filePath))
                {
                    // เพิ่ม Delay เล็กน้อยเพื่อจำลองการโหลดข้อมูล
                    await Task.Delay(100);

                    // ใช้ Task.Run เพื่อโหลดข้อมูลใน background thread
                    var historyList = await Task.Run(() =>
                    {
                        return _conversionService.LoadHistoryFromFile();
                    });

                    if (historyList != null && historyList.Count > 0)
                    {
                        Dispatcher.Dispatch(() =>
                        {
                            // เคลียร์ข้อมูลเก่าใน ObservableCollection
                            HistoryList.Clear();

                            // เรียงลำดับข้อมูลจากล่าสุดไปเก่า
                            foreach (var history in historyList.OrderByDescending(h => h.Timestamp))
                            {
                                HistoryList.Add(history);
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false; // หยุดแสดง ActivityIndicator
            }
        }

        // ฟังก์ชันสำหรับบันทึกข้อมูลประวัติการแปลง
        public void SaveConversionHistory(double inputValue, string fromUnit, string toUnit, double resultValue)
        {
            var history = new ConversionHistory
            {
                InputValue = inputValue,
                FromUnit = fromUnit,
                ToUnit = toUnit,
                ResultValue = resultValue,
                Timestamp = DateTime.Now
            };

            // บันทึกข้อมูลประวัติการแปลงลงในไฟล์
            _conversionService.SaveHistoryToFile(history);

            // เพิ่มประวัติการแปลงใน ObservableCollection เพื่อให้อัปเดต UI
            HistoryList.Insert(0, history);
        }

        // ฟังก์ชันเคลียร์ประวัติการแปลง
        private async void OnClearHistoryClicked(object sender, EventArgs e)
        {
            if (sender is Image clearButton)
            {
                // ย่อขนาดปุ่มพร้อมลดความโปร่งใสของหน้าจอพร้อมกัน
                await Task.WhenAll(
                    clearButton.ScaleTo(0.8, 150, Easing.CubicIn) // ย่อขนาดปุ่ม
                );

                // เพิ่มความโปร่งใสกลับและขยายปุ่ม
                await Task.WhenAll(
                    clearButton.ScaleTo(1.1, 150, Easing.BounceOut) // ขยายขนาดปุ่ม
                );

                // รีเซ็ตขนาดปุ่มกลับเป็นปกติ
                await clearButton.ScaleTo(1, 100, Easing.BounceOut); // รีเซ็ตขนาด

                // หลังจาก Animation เสร็จแล้ว แสดงป๊อปอัพยืนยันการลบ
                bool isConfirmed = await DisplayAlert("Confirm Deletion", "Are you sure you want to clear the history?", "Yes, delete", "Cancel");

                if (isConfirmed)
                {
                    // เคลียร์ข้อมูลจาก ObservableCollection
                    HistoryList.Clear();

                    // ลบไฟล์ที่เก็บประวัติการแปลง
                    string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "conversion_history.json");
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
            }
        }


    }
}
