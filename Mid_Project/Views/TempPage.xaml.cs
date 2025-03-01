using Mid_Project.ViewModels;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Mid_Project;

public partial class TempPage : ContentPage
{
    public TempPage()
    {
        InitializeComponent();
        this.BindingContext = new TempPageViewModel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (InputValue.IsEnabled && InputValue.IsVisible)
        {
            await Task.Delay(100);
            InputValue.Focus();
        }

        var viewModel = (TempPageViewModel)BindingContext;

        if (viewModel.TemperatureUnits != null && viewModel.TemperatureUnits.Count > 0)
        {
            viewModel.SelectedFromTemp = viewModel.TemperatureUnits.FirstOrDefault();
            viewModel.SelectedToTemp = viewModel.TemperatureUnits.ElementAtOrDefault(1);
        }
    }

    private async void OnConvertClicked(object sender, EventArgs e)
    {
        var viewModel = (TempPageViewModel)BindingContext;

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

        if (viewModel.SelectedFromTemp == null || viewModel.SelectedToTemp == null)
        {
            message = "Please select both 'From' and 'To' units";
            await Toast.Make(message, ToastDuration.Short, 14).Show(cancellationTokenSource.Token);
            return;
        }

        var fromUnit = viewModel.SelectedFromTemp;
        var toUnit = viewModel.SelectedToTemp;

        double result = viewModel.ConvertTemperature(inputValue, fromUnit, toUnit);
        string formattedResult = result.ToString("F10").TrimEnd('0').TrimEnd('.');

        ResultLabel.Text = $"{formattedResult} {toUnit.TempName}";

        viewModel.SaveConversionHistory(inputValue, fromUnit.TempName, toUnit.TempName, result);
    }

    private async void OnSwapClicked(object sender, EventArgs e)
    {
        var viewModel = (TempPageViewModel)BindingContext;

        if (viewModel.SelectedFromTemp != null && viewModel.SelectedToTemp != null)
        {
            var temp = viewModel.SelectedFromTemp;
            viewModel.SelectedFromTemp = viewModel.SelectedToTemp;
            viewModel.SelectedToTemp = temp;

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
