using DSWeatherApp.Models;
using Newtonsoft.Json;
using System.Text;

namespace DSWeatherApp.Pages;

public partial class LoginPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();

    public LoginPage()
    {
        InitializeComponent();

    }

    private async void TapRegister_Tapped(object sender, TappedEventArgs e)
    {
        var url = "https://4k33qgqk-44351.uks1.devtunnels.ms";
        
        await Launcher.OpenAsync(url);
    }

    private async void BtnSignIn_Clicked(object sender, EventArgs e)
    {
        string email = EntEmail.Text;
        string password = EntPassword.Text;


        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Error", "Please fill in both fields.", "OK");
            return;
        }


        var loginRequest = new Login
        {
            Email = email,
            Password = password
        };

        var jsonContent = JsonConvert.SerializeObject(loginRequest);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        try
        {

            var response = await _httpClient.PostAsync("https://4k33qgqk-44351.uks1.devtunnels.ms/api/account/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<dynamic>(responseBody);


                string token = responseData?.token;

                if (!string.IsNullOrEmpty(token))
                {

                    Preferences.Set("AuthToken", token);


                    await Navigation.PushAsync(new HomePage());
                }
                else
                {
                    await DisplayAlert("Error", "Login failed, no token received.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "Invalid email or password.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Login failed: {ex.Message}", "OK");
        }
    }
}
