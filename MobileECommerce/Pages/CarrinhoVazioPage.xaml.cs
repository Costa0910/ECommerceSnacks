using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileECommerce.Pages;

public partial class CarrinhoVazioPage : ContentPage
{
    public CarrinhoVazioPage()
    {
        InitializeComponent();
    }

    private async void BtnRetornar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
