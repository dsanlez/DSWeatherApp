using DSWeatherApp.Models;
using DSWeatherApp.Pages;

namespace DSWeatherApp
{
    public partial class App : Application
    {
        

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
           
        }
    }
}
