using Mid_Project.ViewModels;
using Mid_Project.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Mid_Project;

public partial class TimePage : ContentPage
{
    public TimePage()
    {
        InitializeComponent();
        this.BindingContext = new TimePageViewModel();
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

        var viewModel = (TimePageViewModel)BindingContext;

        // Set default values for the Pickers
        if (viewModel.Times != null && viewModel.Times.Count > 0)
        {
            viewModel.SelectedFromTime = viewModel.Times.FirstOrDefault();
            viewModel.SelectedToTime = viewModel.Times.ElementAtOrDefault(1); // Default "Minute"
        }
    }

    private async void OnConvertClicked(object sender, EventArgs e)
    {
        var viewModel = (TimePageViewModel)BindingContext;

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

        if (viewModel.SelectedFromTime == null || viewModel.SelectedToTime == null)
        {
            message = "Please select both 'From' and 'To' units";
            await Toast.Make(message, ToastDuration.Short, 14).Show(cancellationTokenSource.Token);
            return;
        }

        var fromUnit = viewModel.SelectedFromTime;
        var toUnit = viewModel.SelectedToTime;

        double result = viewModel.ConvertTime(inputValue);
        string formattedResult = result.ToString("F10").TrimEnd('0').TrimEnd('.');

        ResultLabel.Text = $"{formattedResult} {toUnit.TimeName}";

        // Save conversion history
        viewModel.SaveConversionHistory(inputValue, fromUnit.TimeName, toUnit.TimeName, result);
    }

    private async void OnSwapClicked(object sender, EventArgs e)
    {
        var viewModel = (TimePageViewModel)BindingContext;

        if (viewModel.SelectedFromTime != null && viewModel.SelectedToTime != null)
        {
            var temp = viewModel.SelectedFromTime;
            viewModel.SelectedFromTime = viewModel.SelectedToTime;
            viewModel.SelectedToTime = temp;

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
