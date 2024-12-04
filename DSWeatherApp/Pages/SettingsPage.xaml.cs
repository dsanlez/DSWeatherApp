using System.Globalization;

namespace DSWeatherApp.Pages;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
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
        bool isDarkMode = e.Value;
    }
}
