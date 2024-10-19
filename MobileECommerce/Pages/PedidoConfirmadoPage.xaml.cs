using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileECommerce.Pages;

public partial class PedidoConfirmadoPage : ContentPage
{
    public PedidoConfirmadoPage()
    {
        InitializeComponent();
    }

    private async void BtnRetornar_OnClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
