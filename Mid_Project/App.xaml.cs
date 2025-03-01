using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Application = Microsoft.Maui.Controls.Application;
using TabbedPage = Microsoft.Maui.Controls.TabbedPage;

namespace Mid_Project
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Application.Current.UserAppTheme = AppTheme.Light;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {

            var tabbedPage = new TabbedPage();

            tabbedPage.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>()
                .SetToolbarPlacement(ToolbarPlacement.Bottom);

            tabbedPage.Children.Add(new MainPage
            {
                Title = "Home",
                IconImageSource = "home.png",
            });
            tabbedPage.Children.Add(new HistoryPage
            {
                Title = "History",
                IconImageSource = "history.png",
            });

            return new Window(new NavigationPage(tabbedPage));
        }
    }
}