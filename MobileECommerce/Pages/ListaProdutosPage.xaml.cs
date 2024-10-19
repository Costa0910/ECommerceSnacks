using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileECommerce.Models;
using MobileECommerce.Services;
using MobileECommerce.Validations;

namespace MobileECommerce.Pages;

public partial class ListaProdutosPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private int _categoriaId;
    private bool _loginPageDisplayed = false;

    public ListaProdutosPage(int categoriaId, string categoriaNome,
        ApiService apiService, IValidator validador)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validador;
        _categoriaId = categoriaId;
        Title = categoriaNome ?? "Produtos"; // Definindo o título da página
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetListaProdutos(_categoriaId);
    }

    private async Task<IEnumerable<Product>> GetListaProdutos(int categoriaId)
    {
        try
        {
            var (produtos, errorMessage)
                = await _apiService.GetProdutos("categoria",
                    categoriaId.ToString());

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Product>();
            }

            if (produtos is null)
            {
                await DisplayAlert("Erro",
                    errorMessage ?? "Não foi possível obter as categorias.",
                    "OK");
                return Enumerable.Empty<Product>();
            }

            // controle (por exemplo, ListView) com as categorias obtidas
            CvProdutos.ItemsSource = produtos;
            return produtos;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro",
                $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return Enumerable.Empty<Product>();
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}
