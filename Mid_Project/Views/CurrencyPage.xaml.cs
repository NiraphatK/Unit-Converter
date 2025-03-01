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

        // ���¡�� FetchCurrencyDataAsync ������Ŵ�����Ũҡ API
        var viewModel = (CurrencyPageViewModel)BindingContext;
        await viewModel.FetchCurrencyDataAsync();  // ���������Ũҡ API �١��Ŵ

        // ��Ǩ�ͺ��� InputValue ������ӧҹ
        if (InputValue.IsEnabled && InputValue.IsVisible)
        {
            await Task.Delay(100); // ˹�ǧ�������˹����Ŵ
            InputValue.Focus();    // �Դ�������
        }

        // �礡�õ�駤�Ңͧ Picker �����˹����Ŵ
        if (viewModel.Currencies != null && viewModel.Currencies.Count > 0)
        {
            // ��駤�� default ���Ѻ Picker
            viewModel.SelectedFromCurrency = viewModel.Currencies.FirstOrDefault(c => c.CurrencyName == "THB");
            viewModel.SelectedToCurrency = viewModel.Currencies.FirstOrDefault(c => c.CurrencyName == "USD");

        }
        else
        {
            // �óշ�� Currencies �ѧ�������
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

        // �ѹ�֡����ѵԡ���ŧ
        viewModel.SaveConversionHistory(inputValue, fromUnit.CurrencyName, toUnit.CurrencyName, result);
    }


    private double ConvertCurrency(double value, Currency fromUnit, Currency toUnit)
    {
        // �ŧ��Ҩҡ˹��µ鹷ҧ�繰ҹ
        return value * (double)toUnit.ConversionRate / (double)fromUnit.ConversionRate;
    }

    private async void OnSwapClicked(object sender, EventArgs e)
    {
        // �֧ ViewModel �ҡ BindingContext
        var viewModel = (CurrencyPageViewModel)BindingContext;

        // ��Ǩ�ͺ����ա�����͡˹��������������
        if (viewModel.SelectedFromCurrency != null && viewModel.SelectedToCurrency != null)
        {
            // ��Ѻ��� SelectedFromCurrency ��� SelectedToCurrency
            var temp = viewModel.SelectedFromCurrency;
            viewModel.SelectedFromCurrency = viewModel.SelectedToCurrency;
            viewModel.SelectedToCurrency = temp;

            // �� Animation ��ع 180 ͧ��
            var button = (Image)sender; // �������һ����繵�Ƿ�����¡ Event
            await button.RotateTo(180, 500, Easing.SinOut); // ��ع 180 ͧ������� 500ms
            button.Rotation = 0; // ���絡����ع��Ѻ��������������� 0 ͧ��㹤��駶Ѵ�
        }
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
