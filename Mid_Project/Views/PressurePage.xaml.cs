using Mid_Project.ViewModels;
using Mid_Project.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Mid_Project;

public partial class PressurePage : ContentPage
{
    public PressurePage()
    {
        InitializeComponent();
        this.BindingContext = new PressurePageViewModel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Focus on the input field when the page appears
        if (InputValue.IsEnabled && InputValue.IsVisible)
        {
            await Task.Delay(100); // Wait for the page to load
            InputValue.Focus();    // Show the keyboard
        }

        var viewModel = (PressurePageViewModel)BindingContext;

        // Set default values for the Pickers
        if (viewModel.Pressures != null && viewModel.Pressures.Count > 0)
        {
            viewModel.SelectedFromPressure = viewModel.Pressures.FirstOrDefault();
            viewModel.SelectedToPressure = viewModel.Pressures.ElementAtOrDefault(1); // Default "Bar"
        }
    }

    private async void OnConvertClicked(object sender, EventArgs e)
    {
        var viewModel = (PressurePageViewModel)BindingContext;

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

        if (viewModel.SelectedFromPressure == null || viewModel.SelectedToPressure == null)
        {
            message = "Please select both 'From' and 'To' units";
            await Toast.Make(message, ToastDuration.Short, 14).Show(cancellationTokenSource.Token);
            return;
        }

        var fromUnit = viewModel.SelectedFromPressure;
        var toUnit = viewModel.SelectedToPressure;

        double result = viewModel.ConvertPressure(inputValue);
        string formattedResult = result.ToString("F10").TrimEnd('0').TrimEnd('.');

        ResultLabel.Text = $"{formattedResult} {toUnit.PressureName}";

        // Save conversion history
        viewModel.SaveConversionHistory(inputValue, fromUnit.PressureName, toUnit.PressureName, result);
    }

    private async void OnSwapClicked(object sender, EventArgs e)
    {
        var viewModel = (PressurePageViewModel)BindingContext;

        if (viewModel.SelectedFromPressure != null && viewModel.SelectedToPressure != null)
        {
            var temp = viewModel.SelectedFromPressure;
            viewModel.SelectedFromPressure = viewModel.SelectedToPressure;
            viewModel.SelectedToPressure = temp;

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
