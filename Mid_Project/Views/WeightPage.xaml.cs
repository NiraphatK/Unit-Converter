using Mid_Project.ViewModels;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Mid_Project.Models;

namespace Mid_Project;

public partial class WeightPage : ContentPage
{
    public WeightPage()
    {
        InitializeComponent();
        this.BindingContext = new WeightPageViewModel();
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
        var viewModel = (WeightPageViewModel)BindingContext;

        // ตรวจสอบว่า Weights มีข้อมูลหรือไม่ก่อนตั้งค่า
        if (viewModel.Weights != null && viewModel.Weights.Count > 0)
        {
            // ตั้งค่า default ให้กับ Picker
            viewModel.SelectedFromWeight = viewModel.Weights.FirstOrDefault();  // หน่วยเริ่มต้น
            viewModel.SelectedToWeight = viewModel.Weights.ElementAtOrDefault(1);    // หน่วยเริ่มต้น
        }
    }

    private async void OnConvertClicked(object sender, EventArgs e)
    {
        var viewModel = (WeightPageViewModel)BindingContext;

        string message;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        // ตรวจสอบให้แน่ใจว่าได้กรอกค่า InputValue
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

        // ตรวจสอบว่าเลือกหน่วยจากและหน่วยถึงหรือไม่
        if (viewModel.SelectedFromWeight == null || viewModel.SelectedToWeight == null)
        {
            message = "Please select both 'From' and 'To' units";
            await Toast.Make(message, ToastDuration.Short, 14).Show(cancellationTokenSource.Token);
            return;
        }

        var fromUnit = viewModel.SelectedFromWeight;
        var toUnit = viewModel.SelectedToWeight;

        double result = ConvertWeight(inputValue, fromUnit, toUnit);
        string formattedResult = result.ToString("F10").TrimEnd('0').TrimEnd('.');

        ResultLabel.Text = $"{formattedResult} {toUnit.WeightName}";

        // บันทึกประวัติการแปลง
        viewModel.SaveConversionHistory(inputValue, fromUnit.WeightName, toUnit.WeightName, result);
    }

    private double ConvertWeight(double value, Weight fromUnit, Weight toUnit)
    {
        // แปลงค่าจากหน่วยต้นทางเป็นกรัม
        double valueInGrams = value * fromUnit.ConversionFactor;

        // แปลงค่าจากกรัมไปยังหน่วยปลายทาง
        return valueInGrams / toUnit.ConversionFactor;
    }

    private async void OnSwapClicked(object sender, EventArgs e)
    {
        // ดึง ViewModel จาก BindingContext
        var viewModel = (WeightPageViewModel)BindingContext;

        // ตรวจสอบว่ามีการเลือกหน่วยแล้วหรือไม่

        if (viewModel.SelectedFromWeight != null && viewModel.SelectedToWeight != null)
        {
            // สลับค่า SelectedFromWeight และ SelectedToWeight
            var temp = viewModel.SelectedFromWeight;
            viewModel.SelectedFromWeight = viewModel.SelectedToWeight;
            viewModel.SelectedToWeight = temp;

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
