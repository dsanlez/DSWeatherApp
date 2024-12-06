namespace DSWeatherApp.Pages;

public partial class AboutPage : ContentPage
{
	public AboutPage()
	{
		InitializeComponent();
	}

    private async void OnLinkedInClicked(object sender, EventArgs e)
    {
        var uri = new Uri("https://www.linkedin.com/in/diogo-sanlez-051915244/");
        await Launcher.Default.OpenAsync(uri);
    }
}