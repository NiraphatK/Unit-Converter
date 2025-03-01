using Mid_Project.ViewModels;
using Mid_Project.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Mid_Project;

public partial class AreaPage : ContentPage
{
    public AreaPage()
    {
        InitializeComponent();
        this.BindingContext = new AreaPageViewModel();
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
        var viewModel = (AreaPageViewModel)BindingContext;

        // ตรวจสอบว่า Areas มีข้อมูลหรือไม่ก่อนตั้งค่า
        if (viewModel.Areas != null && viewModel.Areas.Count > 0)
        {
            // ตั้งค่า default ให้กับ Picker
            viewModel.SelectedFromArea = viewModel.Areas.FirstOrDefault();  // หน่วยเริ่มต้น "Square Millimeter"
            viewModel.SelectedToArea = viewModel.Areas.ElementAtOrDefault(1);    // หน่วยเริ่มต้น "Square Centimeter"
        }
    }

    private async void OnConvertClicked(object sender, EventArgs e)
    {
        var viewModel = (AreaPageViewModel)BindingContext;

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

        if (viewModel.SelectedFromArea == null || viewModel.SelectedToArea == null)
        {
            message = "Please select both 'From' and 'To' units";
            await Toast.Make(message, ToastDuration.Short, 14).Show(cancellationTokenSource.Token);
            return;
        }

        var fromUnit = viewModel.SelectedFromArea;
        var toUnit = viewModel.SelectedToArea;

        double result = ConvertArea(inputValue, fromUnit, toUnit);
        string formattedResult = result.ToString("F10").TrimEnd('0').TrimEnd('.');

        ResultLabel.Text = $"{formattedResult} {toUnit.AreaName}";

        // บันทึกประวัติการแปลง
        viewModel.SaveConversionHistory(inputValue, fromUnit.AreaName, toUnit.AreaName, result);
    }

    // ฟังก์ชันแปลงค่าจากหน่วยหนึ่งไปยังอีกหน่วยหนึ่ง
    private double ConvertArea(double value, Area fromUnit, Area toUnit)
    {
        // แปลงค่าจากหน่วยต้นทางเป็นตารางมิลลิเมตร
        double valueInSquareMillimeters = value * fromUnit.ConversionFactor;

        // แปลงค่าจากตารางมิลลิเมตรเป็นหน่วยปลายทาง
        return valueInSquareMillimeters / toUnit.ConversionFactor;
    }

    private async void OnSwapClicked(object sender, EventArgs e)
    {
        // ดึง ViewModel จาก BindingContext
        var viewModel = (AreaPageViewModel)BindingContext;

        // ตรวจสอบว่ามีการเลือกหน่วยแล้วหรือไม่
        if (viewModel.SelectedFromArea != null && viewModel.SelectedToArea != null)
        {
            // สลับค่า SelectedFromArea และ SelectedToArea
            var temp = viewModel.SelectedFromArea;
            viewModel.SelectedFromArea = viewModel.SelectedToArea;
            viewModel.SelectedToArea = temp;

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
