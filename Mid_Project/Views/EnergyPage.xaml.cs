using Mid_Project.ViewModels;
using Mid_Project.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Mid_Project;

public partial class EnergyPage : ContentPage
{
    public EnergyPage()
    {
        InitializeComponent();
        this.BindingContext = new EnergyPageViewModel();
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

        var viewModel = (EnergyPageViewModel)BindingContext;

        // Set default values for the Pickers
        if (viewModel.Energies != null && viewModel.Energies.Count > 0)
        {
            viewModel.SelectedFromEnergy = viewModel.Energies.FirstOrDefault();
            viewModel.SelectedToEnergy = viewModel.Energies.ElementAtOrDefault(1); // Default to Kilojoule (kJ)
        }
    }

    private async void OnConvertClicked(object sender, EventArgs e)
    {
        var viewModel = (EnergyPageViewModel)BindingContext;

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

        if (viewModel.SelectedFromEnergy == null || viewModel.SelectedToEnergy == null)
        {
            message = "Please select both 'From' and 'To' units";
            await Toast.Make(message, ToastDuration.Short, 14).Show(cancellationTokenSource.Token);
            return;
        }

        var fromUnit = viewModel.SelectedFromEnergy;
        var toUnit = viewModel.SelectedToEnergy;

        double result = viewModel.ConvertEnergy(inputValue);
        string formattedResult = result.ToString("F10").TrimEnd('0').TrimEnd('.');

        ResultLabel.Text = $"{formattedResult} {toUnit.EnergyName}";

        // Save conversion history
        viewModel.SaveConversionHistory(inputValue, fromUnit.EnergyName, toUnit.EnergyName, result);
    }

    private async void OnSwapClicked(object sender, EventArgs e)
    {
        var viewModel = (EnergyPageViewModel)BindingContext;

        if (viewModel.SelectedFromEnergy != null && viewModel.SelectedToEnergy != null)
        {
            var temp = viewModel.SelectedFromEnergy;
            viewModel.SelectedFromEnergy = viewModel.SelectedToEnergy;
            viewModel.SelectedToEnergy = temp;

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
