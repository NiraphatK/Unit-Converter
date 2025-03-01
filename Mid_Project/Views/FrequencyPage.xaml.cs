using Mid_Project.ViewModels;
using Mid_Project.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Mid_Project;

public partial class FrequencyPage : ContentPage
{
    public FrequencyPage()
    {
        InitializeComponent();
        this.BindingContext = new FrequencyPageViewModel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (InputValue.IsEnabled && InputValue.IsVisible)
        {
            await Task.Delay(100);
            InputValue.Focus();
        }

        var viewModel = (FrequencyPageViewModel)BindingContext;

        if (viewModel.Frequencies != null && viewModel.Frequencies.Count > 0)
        {
            viewModel.SelectedFromFrequency = viewModel.Frequencies.FirstOrDefault();  // Default: Hertz
            viewModel.SelectedToFrequency = viewModel.Frequencies.ElementAtOrDefault(1);    // Default: Kilohertz
        }
    }

    private async void OnConvertClicked(object sender, EventArgs e)
    {
        var viewModel = (FrequencyPageViewModel)BindingContext;
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

        if (viewModel.SelectedFromFrequency == null || viewModel.SelectedToFrequency == null)
        {
            message = "Please select both 'From' and 'To' units";
            await Toast.Make(message, ToastDuration.Short, 14).Show(cancellationTokenSource.Token);
            return;
        }

        var fromUnit = viewModel.SelectedFromFrequency;
        var toUnit = viewModel.SelectedToFrequency;

        double result = ConvertFrequency(inputValue, fromUnit, toUnit);
        string formattedResult = result.ToString("F10").TrimEnd('0').TrimEnd('.');

        ResultLabel.Text = $"{formattedResult} {toUnit.FrequencyName}";

        // Save conversion history
        viewModel.SaveConversionHistory(inputValue, fromUnit.FrequencyName, toUnit.FrequencyName, result);
    }

    private double ConvertFrequency(double value, Frequency fromUnit, Frequency toUnit)
    {
        // Convert value from 'fromUnit' to Hertz (base unit)
        double valueInHertz = value * fromUnit.ConversionFactor;

        // Convert from Hertz to the 'toUnit'
        return valueInHertz / toUnit.ConversionFactor;
    }

    private async void OnSwapClicked(object sender, EventArgs e)
    {
        var viewModel = (FrequencyPageViewModel)BindingContext;

        if (viewModel.SelectedFromFrequency != null && viewModel.SelectedToFrequency != null)
        {
            var temp = viewModel.SelectedFromFrequency;
            viewModel.SelectedFromFrequency = viewModel.SelectedToFrequency;
            viewModel.SelectedToFrequency = temp;

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
