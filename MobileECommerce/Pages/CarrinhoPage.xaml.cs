using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileECommerce.Models;
using MobileECommerce.Services;
using MobileECommerce.Validations;

namespace MobileECommerce.Pages;

public partial class CarrinhoPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;
    private bool _isNavigatingToEmptyCartPage = false;

    private ObservableCollection<CarrinhoCompraItem>
        ItensCarrinhoCompra = new ObservableCollection<CarrinhoCompraItem>();

    public CarrinhoPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetItensCarrinhoCompra();
    }

    private async Task<bool> GetItensCarrinhoCompra()
    {
        try
        {
            var usuarioId = Preferences.Get("usuarioid", 0);
            var (itensCarrinhoCompra, errorMessage) = await
                _apiService.GetItensCarrinhoCompra(usuarioId);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                // Redirecionar para a p?gina de login
                await DisplayLoginPage();
                return false;
            }

            if (itensCarrinhoCompra == null)
            {
                await DisplayAlert("Erro",
                    errorMessage ??
                    "Não foi possivel obter os itens do carrinho de compra.",
                    "OK");
                return false;
            }

            ItensCarrinhoCompra.Clear();
            foreach (var item in itensCarrinhoCompra)
            {
                ItensCarrinhoCompra.Add(item);
            }

            CvCarrinho.ItemsSource = ItensCarrinhoCompra;
            AtualizaPrecoTotal(); // Atualizar o preco total ap?s atualizar os itens do carrinho

            if (!ItensCarrinhoCompra.Any())
            {
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro",
                $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return false;
        }
    }

    private void AtualizaPrecoTotal()
    {
        try
        {
            var precoTotal
                = ItensCarrinhoCompra.Sum(item => item.Price * item.Quantity);
            LblPrecoTotal.Text = precoTotal.ToString();
        }
        catch (Exception ex)
        {
            DisplayAlert("Erro",
                $"Ocorreu um erro ao atualizar o pre?o total: {ex.Message}",
                "OK");
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;

        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}
