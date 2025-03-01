using Mid_Project.ViewModels;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Mid_Project.Models;

namespace Mid_Project;

public partial class WeightPage : ContentPage
{
    public WeightPage()
    {
        InitializeComponent();
        this.BindingContext = new WeightPageViewModel();
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
        var viewModel = (WeightPageViewModel)BindingContext;

        // ��Ǩ�ͺ��� Weights �բ�������������͹��駤��
        if (viewModel.Weights != null && viewModel.Weights.Count > 0)
        {
            // ��駤�� default ���Ѻ Picker
            viewModel.SelectedFromWeight = viewModel.Weights.FirstOrDefault();  // ˹����������
            viewModel.SelectedToWeight = viewModel.Weights.ElementAtOrDefault(1);    // ˹����������
        }
    }

    private async void OnConvertClicked(object sender, EventArgs e)
    {
        var viewModel = (WeightPageViewModel)BindingContext;

        string message;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        // ��Ǩ�ͺ������������͡��� InputValue
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

        // ��Ǩ�ͺ������͡˹��¨ҡ���˹��¶֧�������
        if (viewModel.SelectedFromWeight == null || viewModel.SelectedToWeight == null)
        {
            message = "Please select both 'From' and 'To' units";
            await Toast.Make(message, ToastDuration.Short, 14).Show(cancellationTokenSource.Token);
            return;
        }

        var fromUnit = viewModel.SelectedFromWeight;
        var toUnit = viewModel.SelectedToWeight;

        double result = ConvertWeight(inputValue, fromUnit, toUnit);
        string formattedResult = result.ToString("F10").TrimEnd('0').TrimEnd('.');

        ResultLabel.Text = $"{formattedResult} {toUnit.WeightName}";

        // �ѹ�֡����ѵԡ���ŧ
        viewModel.SaveConversionHistory(inputValue, fromUnit.WeightName, toUnit.WeightName, result);
    }

    private double ConvertWeight(double value, Weight fromUnit, Weight toUnit)
    {
        // �ŧ��Ҩҡ˹��µ鹷ҧ�繡���
        double valueInGrams = value * fromUnit.ConversionFactor;

        // �ŧ��Ҩҡ������ѧ˹��»��·ҧ
        return valueInGrams / toUnit.ConversionFactor;
    }

    private async void OnSwapClicked(object sender, EventArgs e)
    {
        // �֧ ViewModel �ҡ BindingContext
        var viewModel = (WeightPageViewModel)BindingContext;

        // ��Ǩ�ͺ����ա�����͡˹��������������

        if (viewModel.SelectedFromWeight != null && viewModel.SelectedToWeight != null)
        {
            // ��Ѻ��� SelectedFromWeight ��� SelectedToWeight
            var temp = viewModel.SelectedFromWeight;
            viewModel.SelectedFromWeight = viewModel.SelectedToWeight;
            viewModel.SelectedToWeight = temp;

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
