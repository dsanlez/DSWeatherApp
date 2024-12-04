using DSWeatherApp.Models;
using DSWeatherApp.Pages;


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
                    new ShellContent {Title = "Home", Icon = "home", Content = homePage},
                    new ShellContent {Title = "Current Weather", Icon = "cloudy", Content = currentPage},
                    new ShellContent {Title = "Forecast Weather", Icon = "cloudy", Content = forecastPage},
                    new ShellContent {Title = "Settings", Icon = "settings", Content = settingsPage},
                    new ShellContent {Title = "About", Icon = "about", Content = aboutPage},
                }
            });
        }
    }
}
