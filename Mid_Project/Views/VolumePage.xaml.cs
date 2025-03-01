using Mid_Project.ViewModels;
using Mid_Project.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Mid_Project;

public partial class VolumePage : ContentPage
{
    public VolumePage()
    {
        InitializeComponent();
        this.BindingContext = new VolumePageViewModel();
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

        var viewModel = (VolumePageViewModel)BindingContext;

        // Set default values for the Pickers
        if (viewModel.Volumes != null && viewModel.Volumes.Count > 0)
        {
            viewModel.SelectedFromVolume = viewModel.Volumes.FirstOrDefault();
            viewModel.SelectedToVolume = viewModel.Volumes.ElementAtOrDefault(1); // Default "Liter"
        }
    }

    private async void OnConvertClicked(object sender, EventArgs e)
    {
        var viewModel = (VolumePageViewModel)BindingContext;

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

        if (viewModel.SelectedFromVolume == null || viewModel.SelectedToVolume == null)
        {
            message = "Please select both 'From' and 'To' units";
            await Toast.Make(message, ToastDuration.Short, 14).Show(cancellationTokenSource.Token);
            return;
        }

        var fromUnit = viewModel.SelectedFromVolume;
        var toUnit = viewModel.SelectedToVolume;

        double result = viewModel.ConvertVolume(inputValue);
        string formattedResult = result.ToString("F10").TrimEnd('0').TrimEnd('.');

        ResultLabel.Text = $"{formattedResult} {toUnit.VolumeName}";

        // Save conversion history
        viewModel.SaveConversionHistory(inputValue, fromUnit.VolumeName, toUnit.VolumeName, result);
    }

    private async void OnSwapClicked(object sender, EventArgs e)
    {
        var viewModel = (VolumePageViewModel)BindingContext;

        if (viewModel.SelectedFromVolume != null && viewModel.SelectedToVolume != null)
        {
            var temp = viewModel.SelectedFromVolume;
            viewModel.SelectedFromVolume = viewModel.SelectedToVolume;
            viewModel.SelectedToVolume = temp;

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
