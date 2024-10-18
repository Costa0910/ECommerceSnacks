using MobileECommerce.Pages;
using MobileECommerce.Services;

namespace MobileECommerce;

public partial class App : Application
{
    public App(ApiService apiService)
    {
        InitializeComponent();

        MainPage = new NavigationPage(new InscricaoPage(apiService));
    }
}
