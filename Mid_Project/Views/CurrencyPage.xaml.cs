using Mid_Project.ViewModels;
using Mid_Project.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Mid_Project;

public partial class CurrencyPage : ContentPage
{
    public CurrencyPage()
    {
        InitializeComponent();
        this.BindingContext = new CurrencyPageViewModel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // เรียกใช้ FetchCurrencyDataAsync เพื่อโหลดข้อมูลจาก API
        var viewModel = (CurrencyPageViewModel)BindingContext;
        await viewModel.FetchCurrencyDataAsync();  // รอให้ข้อมูลจาก API ถูกโหลด

        // ตรวจสอบให้ InputValue พร้อมทำงาน
        if (InputValue.IsEnabled && InputValue.IsVisible)
        {
            await Task.Delay(100); // หน่วงเวลาให้หน้าโหลด
            InputValue.Focus();    // เปิดคีย์บอร์ด
        }

        // เช็คการตั้งค่าของ Picker เมื่อหน้าโหลด
        if (viewModel.Currencies != null && viewModel.Currencies.Count > 0)
        {
            // ตั้งค่า default ให้กับ Picker
            viewModel.SelectedFromCurrency = viewModel.Currencies.FirstOrDefault(c => c.CurrencyName == "THB");
            viewModel.SelectedToCurrency = viewModel.Currencies.FirstOrDefault(c => c.CurrencyName == "USD");

        }
        else
        {
            // กรณีที่ Currencies ยังไม่พร้อม
            Console.WriteLine("Currencies data not yet loaded");
        }
    }



    private async void OnConvertClicked(object sender, EventArgs e)
    {
        var viewModel = (CurrencyPageViewModel)BindingContext;

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

        if (viewModel.SelectedFromCurrency == null || viewModel.SelectedToCurrency == null)
        {
            message = "Please select both 'From' and 'To' currencies";
            await Toast.Make(message, ToastDuration.Short, 14).Show(cancellationTokenSource.Token);
            return;
        }

        var fromUnit = viewModel.SelectedFromCurrency;
        var toUnit = viewModel.SelectedToCurrency;

        double result = ConvertCurrency(inputValue, fromUnit, toUnit);
        string formattedResult = result.ToString("F2").TrimEnd('0').TrimEnd('.');

        string formattedDate = DateTime.Now.ToString("dd MMMM yyyy HH:mm");

        ResultLabel.Text = $"{formattedResult} {toUnit.CurrencyName}\n" +
                           $"{formattedDate}";

        // บันทึกประวัติการแปลง
        viewModel.SaveConversionHistory(inputValue, fromUnit.CurrencyName, toUnit.CurrencyName, result);
    }


    private double ConvertCurrency(double value, Currency fromUnit, Currency toUnit)
    {
        // แปลงค่าจากหน่วยต้นทางเป็นฐาน
        return value * (double)toUnit.ConversionRate / (double)fromUnit.ConversionRate;
    }

    private async void OnSwapClicked(object sender, EventArgs e)
    {
        // ดึง ViewModel จาก BindingContext
        var viewModel = (CurrencyPageViewModel)BindingContext;

        // ตรวจสอบว่ามีการเลือกหน่วยแล้วหรือไม่
        if (viewModel.SelectedFromCurrency != null && viewModel.SelectedToCurrency != null)
        {
            // สลับค่า SelectedFromCurrency และ SelectedToCurrency
            var temp = viewModel.SelectedFromCurrency;
            viewModel.SelectedFromCurrency = viewModel.SelectedToCurrency;
            viewModel.SelectedToCurrency = temp;

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
