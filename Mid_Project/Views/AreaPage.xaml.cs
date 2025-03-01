using Mid_Project.ViewModels;
using Mid_Project.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Mid_Project;

public partial class AreaPage : ContentPage
{
    public AreaPage()
    {
        InitializeComponent();
        this.BindingContext = new AreaPageViewModel();
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
        var viewModel = (AreaPageViewModel)BindingContext;

        // ��Ǩ�ͺ��� Areas �բ�������������͹��駤��
        if (viewModel.Areas != null && viewModel.Areas.Count > 0)
        {
            // ��駤�� default ���Ѻ Picker
            viewModel.SelectedFromArea = viewModel.Areas.FirstOrDefault();  // ˹���������� "Square Millimeter"
            viewModel.SelectedToArea = viewModel.Areas.ElementAtOrDefault(1);    // ˹���������� "Square Centimeter"
        }
    }

    private async void OnConvertClicked(object sender, EventArgs e)
    {
        var viewModel = (AreaPageViewModel)BindingContext;

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

        if (viewModel.SelectedFromArea == null || viewModel.SelectedToArea == null)
        {
            message = "Please select both 'From' and 'To' units";
            await Toast.Make(message, ToastDuration.Short, 14).Show(cancellationTokenSource.Token);
            return;
        }

        var fromUnit = viewModel.SelectedFromArea;
        var toUnit = viewModel.SelectedToArea;

        double result = ConvertArea(inputValue, fromUnit, toUnit);
        string formattedResult = result.ToString("F10").TrimEnd('0').TrimEnd('.');

        ResultLabel.Text = $"{formattedResult} {toUnit.AreaName}";

        // �ѹ�֡����ѵԡ���ŧ
        viewModel.SaveConversionHistory(inputValue, fromUnit.AreaName, toUnit.AreaName, result);
    }

    // �ѧ��ѹ�ŧ��Ҩҡ˹���˹����ѧ�ա˹���˹��
    private double ConvertArea(double value, Area fromUnit, Area toUnit)
    {
        // �ŧ��Ҩҡ˹��µ鹷ҧ�繵��ҧ���������
        double valueInSquareMillimeters = value * fromUnit.ConversionFactor;

        // �ŧ��Ҩҡ���ҧ�����������˹��»��·ҧ
        return valueInSquareMillimeters / toUnit.ConversionFactor;
    }

    private async void OnSwapClicked(object sender, EventArgs e)
    {
        // �֧ ViewModel �ҡ BindingContext
        var viewModel = (AreaPageViewModel)BindingContext;

        // ��Ǩ�ͺ����ա�����͡˹��������������
        if (viewModel.SelectedFromArea != null && viewModel.SelectedToArea != null)
        {
            // ��Ѻ��� SelectedFromArea ��� SelectedToArea
            var temp = viewModel.SelectedFromArea;
            viewModel.SelectedFromArea = viewModel.SelectedToArea;
            viewModel.SelectedToArea = temp;

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
