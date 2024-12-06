using DSWeatherApp.Models;
using DSWeatherApp.Pages;
using System.Globalization;

namespace DSWeatherApp
{
    public partial class App : Application
    {  
        public App()
        {
            InitializeComponent();
            var language = Preferences.Get("AppLanguage", "en-US");
            ChangeLanguage(language);
            MainPage = new AppShell();
            MainPage = new NavigationPage(new LoginPage());
        }

        public void ChangeLanguage(string language)
        {
            CultureInfo cultureInfo = language switch
            {
                "Portuguese" => new CultureInfo("pt-PT"),
                "English" => new CultureInfo("en-US"),
                _ => new CultureInfo("en-US")
            };

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            OnLanguageChanged();
        }

        private void OnLanguageChanged()
        {
            MainPage = new NavigationPage(new SettingsPage());
        }
    }
}
