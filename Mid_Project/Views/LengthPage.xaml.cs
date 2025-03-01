using Mid_Project.ViewModels;
using Mid_Project.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Mid_Project;

public partial class LengthPage : ContentPage
{
    public LengthPage()
    {
        InitializeComponent();
        this.BindingContext = new LengthPageViewModel();
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
        var viewModel = (LengthPageViewModel)BindingContext;

        // ��Ǩ�ͺ��� Lengths �բ�������������͹��駤��
        if (viewModel.Lengths != null && viewModel.Lengths.Count > 0)
        {
            // ��駤�� default ���Ѻ Picker
            viewModel.SelectedFromLength = viewModel.Lengths.FirstOrDefault();  // ˹���������� "Millimeters"
            viewModel.SelectedToLength = viewModel.Lengths.ElementAtOrDefault(1);    // ˹���������� "Millimeters"
        }
    }

    private async void OnConvertClicked(object sender, EventArgs e)
    {
        var viewModel = (LengthPageViewModel)BindingContext;

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

        if (viewModel.SelectedFromLength == null || viewModel.SelectedToLength == null)
        {
            message = "Please select both 'From' and 'To' units";
            await Toast.Make(message, ToastDuration.Short, 14).Show(cancellationTokenSource.Token);
            return;
        }

        var fromUnit = viewModel.SelectedFromLength;
        var toUnit = viewModel.SelectedToLength;

        double result = ConvertLength(inputValue, fromUnit, toUnit);
        string formattedResult = result.ToString("F10").TrimEnd('0').TrimEnd('.');

        ResultLabel.Text = $"{formattedResult} {toUnit.LengthName}";

        // �ѹ�֡����ѵԡ���ŧ
        viewModel.SaveConversionHistory(inputValue, fromUnit.LengthName, toUnit.LengthName, result);

    }

    // �ѧ��ѹ�ŧ��Ҩҡ˹���˹����ѧ�ա˹���˹��
    private double ConvertLength(double value, Length fromUnit, Length toUnit)
    {
        // �ŧ��Ҩҡ˹��µ鹷ҧ�����������
        double valueInMillimeters = value * fromUnit.ConversionFactor;

        // �ŧ��Ҩҡ�����������˹��»��·ҧ
        return valueInMillimeters / toUnit.ConversionFactor;
    }
    private async void OnSwapClicked(object sender, EventArgs e)
    {
        // �֧ ViewModel �ҡ BindingContext
        var viewModel = (LengthPageViewModel)BindingContext;

        // ��Ǩ�ͺ����ա�����͡˹��������������
        if (viewModel.SelectedFromLength != null && viewModel.SelectedToLength != null)
        {
            // ��Ѻ��� SelectedFromLength ��� SelectedToLength
            var temp = viewModel.SelectedFromLength;
            viewModel.SelectedFromLength = viewModel.SelectedToLength;
            viewModel.SelectedToLength = temp;

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