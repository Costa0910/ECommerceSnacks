using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MobileECommerce.Models;
using MobileECommerce.Services;
using MobileECommerce.Validations;

namespace MobileECommerce.Pages;

public partial class HomePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    public HomePage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService ??
                      throw new ArgumentNullException(nameof(apiService));
        _validator = validator;
        LblNomeUsuario.Text
            = "Olá, " + Preferences.Get("usuarionome", string.Empty);
        Title = AppConfig.HomePageTitle;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetListaCategorias();
        await GetMaisVendidos();
        await GetPopulares();
    }

    private async Task<IEnumerable<Category>> GetListaCategorias()
    {
        try
        {
            var (categorias, errorMessage) = await _apiService.GetCategorias();

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Category>();
            }

            if (categorias == null)
            {
                await DisplayAlert("Erro",
                    errorMessage ?? "Não foi possível obter as categorias.",
                    "OK");
                return Enumerable.Empty<Category>();
            }

            CvCategorias.ItemsSource = categorias;
            return categorias;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro",
                $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return Enumerable.Empty<Category>();
        }
    }

    private async Task<IEnumerable<Product>> GetMaisVendidos()
    {
        try
        {
            var (produtos, errorMessage)
                = await _apiService.GetProdutos("maisvendido", string.Empty);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Product>();
            }

            if (produtos == null)
            {
                await DisplayAlert("Erro",
                    errorMessage ?? "Não foi possível obter as categorias.",
                    "OK");
                return Enumerable.Empty<Product>();
            }

            CvMaisVendidos.ItemsSource = produtos;
            return produtos;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro",
                $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return Enumerable.Empty<Product>();
        }
    }

    //-----------------

    private async Task<IEnumerable<Product>> GetPopulares()
    {
        try
        {
            var (produtos, errorMessage)
                = await _apiService.GetProdutos("popular", string.Empty);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Product>();
            }

            if (produtos == null)
            {
                await DisplayAlert("Erro",
                    errorMessage ?? "Não foi possível obter as categorias.",
                    "OK");
                return Enumerable.Empty<Product>();
            }

            CvPopulares.ItemsSource = produtos;
            return produtos;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro",
                $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return Enumerable.Empty<Product>();
        }
    }

    //-------------------

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private void CvCategorias_OnSelectionChanged(object sender,
        SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Category;

        if (currentSelection == null) return;


        Navigation.PushAsync(new ListaProdutosPage(currentSelection.Id,
            currentSelection.Name!,
            _apiService,
            _validator));

        ((CollectionView)sender).SelectedItem = null;
    }
}
