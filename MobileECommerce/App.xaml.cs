using MobileECommerce.Pages;
using MobileECommerce.Services;
using MobileECommerce.Validations;

namespace MobileECommerce;

public partial class App : Application
{
    public App(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        MainPage
            = new NavigationPage(new InscricaoPage(apiService, validator));
    }
}
