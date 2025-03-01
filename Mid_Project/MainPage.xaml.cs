
namespace Mid_Project
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            Header.Text = "Unit" + Environment.NewLine + "Converter";
        }
        private async void ToggleThemeButton_Clicked(object sender, EventArgs e)
        {
            // ย่อขนาดปุ่มพร้อมลดความโปร่งใสของหน้าจอพร้อมกัน
            if (ToggleThemeButton != null)
            {
                await Task.WhenAll(
                    ToggleThemeButton.ScaleTo(0.8, 150, Easing.CubicIn),
                    this.FadeTo(0.6, 200)
                );
            }
            // เปลี่ยนธีมและอัปเดตไอคอน
            if (Application.Current != null && ToggleThemeButton != null)
            {
                Application.Current.UserAppTheme = Application.Current.UserAppTheme == AppTheme.Dark
                                                    ? AppTheme.Light
                                                    : AppTheme.Dark;

                ToggleThemeButton.Source = Application.Current.UserAppTheme == AppTheme.Dark
                                            ? "light.png"
                                            : "dark.png";
            }

            // เพิ่มความโปร่งใสกลับและขยายปุ่ม
            if (ToggleThemeButton != null)
            {
                await Task.WhenAll(
                    this.FadeTo(1, 200),
                    ToggleThemeButton.ScaleTo(1.1, 150, Easing.BounceOut)
                );
            }

            // รีเซ็ตขนาดปุ่ม
            if (ToggleThemeButton != null)
            {
                await ToggleThemeButton.ScaleTo(1, 100, Easing.BounceOut);
            }
        }


        private async void LengthTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new LengthPage());
        }
        private async void WeightTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new WeightPage());
        }
        private async void TempTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new TempPage());
        }
        private async void AreaTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new AreaPage());
        }
        private async void VolumeTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new VolumePage());
        }
        private async void TimeTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new TimePage());
        }
        private async void SpeedTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new SpeedPage());
        }
        private async void EnergyTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new EnergyPage());
        }
        private async void PressureTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new PressurePage());
        }
        private async void DataTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new DataPage());
        }
        private async void CurrencyTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new CurrencyPage());
        }
        private async void FreqTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new FrequencyPage());
        }
    }

}
