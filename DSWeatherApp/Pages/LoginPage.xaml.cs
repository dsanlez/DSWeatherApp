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

    private async void BtnSignIn_Clicked(object sender, EventArgs e)
    {
        string email = EntEmail.Text;
        string password = EntPassword.Text;

        // Validação básica de campos
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Error", "Please fill in both fields.", "OK");
            return;
        }

        // Criando o objeto para enviar ao backend
        var loginRequest = new Login
        {
            Email = email,
            Password = password
        };

        var jsonContent = JsonConvert.SerializeObject(loginRequest);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        try
        {
            // Mudar para link do meu projeto
            var response = await _httpClient.PostAsync("https://seu-backend-api.com/api/account/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<dynamic>(responseBody);

                // Armazenar o token, por exemplo, no Preferences ou SecureStorage
                string token = responseData?.token;

                if (!string.IsNullOrEmpty(token))
                {
                    // Armazenar o token (exemplo, usando Preferences)
                    Preferences.Set("AuthToken", token);

                    // Navegar para a página principal ou próxima tela
                    await Navigation.PushAsync(new MainPage());
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
            // Em caso de erro, exibir uma mensagem de erro
            await DisplayAlert("Error", $"Login failed: {ex.Message}", "OK");
        }
    }

    private async void TapRegister_Tapped(object sender, EventArgs e)
    {
        // Navegar para a página de registro
        await Navigation.PushAsync(new LoginPage());
    }
}
