using Mid_Project.ViewModels;
using Mid_Project.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Mid_Project;

public partial class LengthPage : ContentPage
{
    public LengthPage()
    {
        InitializeComponent();
        this.BindingContext = new LengthPageViewModel();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // ตรวจสอบให้ InputValue พร้อมทำงาน
        if (InputValue.IsEnabled && InputValue.IsVisible)
        {
            await Task.Delay(100); // หน่วงเวลาให้หน้าโหลด
            InputValue.Focus();    // เปิดคีย์บอร์ด
        }

        // เช็คการตั้งค่าของ Picker เมื่อหน้าโหลด
        var viewModel = (LengthPageViewModel)BindingContext;

        // ตรวจสอบว่า Lengths มีข้อมูลหรือไม่ก่อนตั้งค่า
        if (viewModel.Lengths != null && viewModel.Lengths.Count > 0)
        {
            // ตั้งค่า default ให้กับ Picker
            viewModel.SelectedFromLength = viewModel.Lengths.FirstOrDefault();  // หน่วยเริ่มต้น "Millimeters"
            viewModel.SelectedToLength = viewModel.Lengths.ElementAtOrDefault(1);    // หน่วยเริ่มต้น "Millimeters"
        }
    }

    private async void OnConvertClicked(object sender, EventArgs e)
    {
        var viewModel = (LengthPageViewModel)BindingContext;

        string message;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        if (string.IsNullOrWhiteSpace(InputValue.Text))
        {
            message = "Please enter a value";
            await Toast.Make(message, ToastDuration.Short, 14).Show(cancellationTokenSource.Token);
            return;
        }

        if (!double.TryParse(InputValue.Text, out double inputValue))
        {
            message = "Invalid input, Please enter a numeric value";
            await Toast.Make(message, ToastDuration.Short, 14).Show(cancellationTokenSource.Token);
            return;
        }

        if (viewModel.SelectedFromLength == null || viewModel.SelectedToLength == null)
        {
            message = "Please select both 'From' and 'To' units";
            await Toast.Make(message, ToastDuration.Short, 14).Show(cancellationTokenSource.Token);
            return;
        }

        var fromUnit = viewModel.SelectedFromLength;
        var toUnit = viewModel.SelectedToLength;

        double result = ConvertLength(inputValue, fromUnit, toUnit);
        string formattedResult = result.ToString("F10").TrimEnd('0').TrimEnd('.');

        ResultLabel.Text = $"{formattedResult} {toUnit.LengthName}";

        // บันทึกประวัติการแปลง
        viewModel.SaveConversionHistory(inputValue, fromUnit.LengthName, toUnit.LengthName, result);

    }

    // ฟังก์ชันแปลงค่าจากหน่วยหนึ่งไปยังอีกหน่วยหนึ่ง
    private double ConvertLength(double value, Length fromUnit, Length toUnit)
    {
        // แปลงค่าจากหน่วยต้นทางเป็นมิลลิเมตร
        double valueInMillimeters = value * fromUnit.ConversionFactor;

        // แปลงค่าจากมิลลิเมตรเป็นหน่วยปลายทาง
        return valueInMillimeters / toUnit.ConversionFactor;
    }
    private async void OnSwapClicked(object sender, EventArgs e)
    {
        // ดึง ViewModel จาก BindingContext
        var viewModel = (LengthPageViewModel)BindingContext;

        // ตรวจสอบว่ามีการเลือกหน่วยแล้วหรือไม่
        if (viewModel.SelectedFromLength != null && viewModel.SelectedToLength != null)
        {
            // สลับค่า SelectedFromLength และ SelectedToLength
            var temp = viewModel.SelectedFromLength;
            viewModel.SelectedFromLength = viewModel.SelectedToLength;
            viewModel.SelectedToLength = temp;

            // ใช้ Animation หมุน 180 องศา
            var button = (Image)sender; // สมมติว่าปุ่มเป็นตัวที่เรียก Event
            await button.RotateTo(180, 500, Easing.SinOut); // หมุน 180 องศาในเวลา 500ms
            button.Rotation = 0; // รีเซ็ตการหมุนกลับเพื่อให้เริ่มที่ 0 องศาในครั้งถัดไป
        }
    }
    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

}