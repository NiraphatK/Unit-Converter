using Mid_Project.ViewModels;
using Mid_Project.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Mid_Project;

public partial class DataPage : ContentPage
{
    public DataPage()
    {
        InitializeComponent();
        this.BindingContext = new DataPageViewModel();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // ��Ǩ�ͺ��� InputValue ������ӧҹ
        if (InputValue.IsEnabled && InputValue.IsVisible)
        {
            await Task.Delay(100); // ˹�ǧ�������˹����Ŵ
            InputValue.Focus();    // �Դ�������
        }

        // �礡�õ�駤�Ңͧ Picker �����˹����Ŵ
        var viewModel = (DataPageViewModel)BindingContext;

        // ��Ǩ�ͺ��� Lengths �բ�������������͹��駤��
        if (viewModel.Datas != null && viewModel.Datas.Count > 0)
        {
            // ��駤�� default ���Ѻ Picker
            viewModel.SelectedFromData = viewModel.Datas.FirstOrDefault();  // ˹���������� "Millimeters"
            viewModel.SelectedToData = viewModel.Datas.ElementAtOrDefault(1);    // ˹���������� "Millimeters"
        }
    }

    private async void OnConvertClicked(object sender, EventArgs e)
    {
        var viewModel = (DataPageViewModel)BindingContext;

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

        if (viewModel.SelectedFromData == null || viewModel.SelectedToData == null)
        {
            message = "Please select both 'From' and 'To' units";
            await Toast.Make(message, ToastDuration.Short, 14).Show(cancellationTokenSource.Token);
            return;
        }

        var fromUnit = viewModel.SelectedFromData;
        var toUnit = viewModel.SelectedToData;

        double result = ConvertDataUnit(inputValue, fromUnit, toUnit);
        string formattedResult = result.ToString("F10").TrimEnd('0').TrimEnd('.');

        ResultLabel.Text = $"{formattedResult} {toUnit.DataName}";

        viewModel.SaveConversionHistory(inputValue, fromUnit.DataName, toUnit.DataName, result);
    }

    private double ConvertDataUnit(double value, Data fromUnit, Data toUnit)
    {
        // Convert value to bits (base unit)
        double valueInBits = value * fromUnit.ConversionFactor;

        // Convert from bits to the target unit
        return valueInBits / toUnit.ConversionFactor;
    }

    private async void OnSwapClicked(object sender, EventArgs e)
    {
        var viewModel = (DataPageViewModel)BindingContext;

        if (viewModel.SelectedFromData != null && viewModel.SelectedToData != null)
        {
            var temp = viewModel.SelectedFromData;
            viewModel.SelectedFromData = viewModel.SelectedToData;
            viewModel.SelectedToData = temp;

            var button = (Image)sender;
            await button.RotateTo(180, 500, Easing.SinOut);
            button.Rotation = 0;
        }
    }
    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
