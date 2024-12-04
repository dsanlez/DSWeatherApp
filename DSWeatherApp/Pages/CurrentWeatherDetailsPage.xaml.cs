namespace DSWeatherApp.Pages;

public partial class CurrentWeatherDetailsPage : ContentPage
{
	public CurrentWeatherDetailsPage()
	{
		InitializeComponent();

        MessagingCenter.Subscribe<SettingsPage>(this, "LanguageChanged", (sender) =>
        {
            RefreshPage();
        });
    }

    private void RefreshPage()
    {
        InitializeComponent();
    }

}