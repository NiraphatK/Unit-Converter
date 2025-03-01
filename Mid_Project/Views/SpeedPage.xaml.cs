using Mid_Project.ViewModels;
using Mid_Project.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Mid_Project;

public partial class SpeedPage : ContentPage
{
    public SpeedPage()
    {
        InitializeComponent();
        this.BindingContext = new SpeedPageViewModel();
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

        var viewModel = (SpeedPageViewModel)BindingContext;

        // Set default values for the Pickers
        if (viewModel.Speeds != null && viewModel.Speeds.Count > 0)
        {
            viewModel.SelectedFromSpeed = viewModel.Speeds.FirstOrDefault();
            viewModel.SelectedToSpeed = viewModel.Speeds.ElementAtOrDefault(1); // Default "Kilometer per hour"
        }
    }

    private async void OnConvertClicked(object sender, EventArgs e)
    {
        var viewModel = (SpeedPageViewModel)BindingContext;

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

        if (viewModel.SelectedFromSpeed == null || viewModel.SelectedToSpeed == null)
        {
            message = "Please select both 'From' and 'To' units";
            await Toast.Make(message, ToastDuration.Short, 14).Show(cancellationTokenSource.Token);
            return;
        }

        var fromUnit = viewModel.SelectedFromSpeed;
        var toUnit = viewModel.SelectedToSpeed;

        double result = viewModel.ConvertSpeed(inputValue);
        string formattedResult = result.ToString("F10").TrimEnd('0').TrimEnd('.');

        ResultLabel.Text = $"{formattedResult} {toUnit.SpeedName}";

        // Save conversion history
        viewModel.SaveConversionHistory(inputValue, fromUnit.SpeedName, toUnit.SpeedName, result);
    }

    private async void OnSwapClicked(object sender, EventArgs e)
    {
        var viewModel = (SpeedPageViewModel)BindingContext;

        if (viewModel.SelectedFromSpeed != null && viewModel.SelectedToSpeed != null)
        {
            var temp = viewModel.SelectedFromSpeed;
            viewModel.SelectedFromSpeed = viewModel.SelectedToSpeed;
            viewModel.SelectedToSpeed = temp;

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
