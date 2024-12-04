using System.Globalization;

namespace DSWeatherApp.Pages;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();

        bool isDarkMode = Preferences.Get("IsDarkMode", false);
        Application.Current.UserAppTheme = isDarkMode ? AppTheme.Dark : AppTheme.Light;
        ThemeSwitch.IsToggled = isDarkMode;
    }

    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = sender as Picker;
        var selectedLanguage = picker.SelectedItem.ToString();

        Preferences.Set("AppLanguage", selectedLanguage);
        ChangeLanguage(selectedLanguage);
    }

    private void ChangeLanguage(string language)
    {
        CultureInfo cultureInfo = language switch
        {
            "Portuguese" => new CultureInfo("pt-PT"),
            "English" => new CultureInfo("en-US"),          
            _ => new CultureInfo("en-US")
        };

        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        MessagingCenter.Send(this, "LanguageChanged");
        App.Current.MainPage = new NavigationPage(new SettingsPage());
    }

    private void ThemeSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            Application.Current.UserAppTheme = AppTheme.Dark;
            Preferences.Set("IsDarkMode", true); 
        }
        else
        {
            Application.Current.UserAppTheme = AppTheme.Light;
            Preferences.Set("IsDarkMode", false);
        }
    }
}
