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
        bool enderecoSalvo = Preferences.ContainsKey("endereco");
        if (enderecoSalvo)
        {
            string name = Preferences.Get("nome", string.Empty);
            string address = Preferences.Get("endereco", string.Empty);
            string phone = Preferences.Get("telefone", string.Empty);

            LblEndereco.Text
                = $"{name}\n{address}\n{phone}";
        }
        else
        {
            LblEndereco.Text = "Informe o endereco de entrega";
        }
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

    private async void BtnIncrementar_OnClicked(object sender, EventArgs e)
    {
        if (sender is Button button &&
            button.BindingContext is CarrinhoCompraItem itemCarrinho)
        {
            itemCarrinho.Quantity++;
            AtualizaPrecoTotal();
            await _apiService.AtualizaQuantidadeItemCarrinho(
                itemCarrinho.ProductId, "aumentar");
        }
    }

    private async void BtnDecrementar_OnClicked(object sender, EventArgs e)
    {
        if (sender is Button button &&
            button.BindingContext is CarrinhoCompraItem itemCarrinho)
        {
            if (itemCarrinho.Quantity == 1) return;
            else
            {
                itemCarrinho.Quantity--;
                AtualizaPrecoTotal();
                await _apiService.AtualizaQuantidadeItemCarrinho(
                    itemCarrinho.ProductId, "diminuir");
            }
        }
    }

    private async void BtnDeletar_Clicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button &&
            button.BindingContext is CarrinhoCompraItem itemCarrinho)
        {
            bool resposta = await DisplayAlert("Confirma  o",
                "Tem certeza que deseja excluir este item do carrinho?", "Sim",
                "N o");
            if (resposta)
            {
                ItensCarrinhoCompra.Remove(itemCarrinho);
                AtualizaPrecoTotal();
                await _apiService.AtualizaQuantidadeItemCarrinho(itemCarrinho
                    .ProductId, "apagar");
            }
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

    private async void BtnEditaEndereco_OnClicked(object sender, EventArgs e)
    {
        if (ItensCarrinhoCompra.Any())
        {
            await Navigation.PushAsync(new EnderecoPage());
        }
        else
        {
            await DisplayAlert("Erro",
                "Não é possível prosseguir sem itens no carrinho de compra.",
                "OK");
        }
    }
}
