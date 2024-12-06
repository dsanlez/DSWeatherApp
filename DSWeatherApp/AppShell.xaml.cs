using DSWeatherApp.Models;
using DSWeatherApp.Pages;
using DSWeatherApp.Resources.Languages;


namespace DSWeatherApp
{
    public partial class AppShell : Shell
    {

        public AppShell()
        {
            InitializeComponent();
            
            ConfigureShell();
        }

        private void ConfigureShell()
        {
            var homePage = new HomePage();
            var currentPage = new CurrentPage();
            var forecastPage = new ForecastPage();
            var settingsPage = new SettingsPage();
            var aboutPage = new AboutPage();

            Items.Add(new TabBar
            {
                Items =
                {
                    new ShellContent {Title = AppResources.Home, Icon = "home", Content = homePage},
                    new ShellContent {Title = AppResources.Current, Icon = "cloudy", Content = currentPage},
                    new ShellContent {Title = AppResources.Forecast, Icon = "cloudy", Content = forecastPage},
                    new ShellContent {Title = AppResources.Settings, Icon = "settings", Content = settingsPage},
                    new ShellContent {Title = AppResources.About, Icon = "about", Content = aboutPage},
                }
            });
        }
    }
}
