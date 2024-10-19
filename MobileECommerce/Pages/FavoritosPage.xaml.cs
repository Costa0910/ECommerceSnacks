using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileECommerce.Models;
using MobileECommerce.Services;
using MobileECommerce.Validations;

namespace MobileECommerce.Pages;

public partial class FavoritosPage : ContentPage
{
    private readonly FavoritosService _favoritosService;
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    public FavoritosPage(ApiService apiService, IValidator validator)

    {
        InitializeComponent();
        _favoritosService = new FavoritosService();
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetProdutosFavoritos();
    }

    private async Task GetProdutosFavoritos()
    {
        try
        {
            var produtosFavoritos = await _favoritosService.ReadAllAsync();

            if (produtosFavoritos is null || produtosFavoritos.Count == 0)
            {
                CvProdutos.ItemsSource = null;//limpa a lista atual
                LblAviso.IsVisible = true; //mostra o aviso
            }
            else
            {
                CvProdutos.ItemsSource = produtosFavoritos;
                LblAviso.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
        }
    }


    private void CvProdutos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as ProdutoFavorito;

        if (currentSelection == null) return;

        Navigation.PushAsync(new ProdutoDetalhesPage(currentSelection.ProdutoId,
            currentSelection.Nome!,
            _apiService, _validator));

        ((CollectionView)sender).SelectedItem = null;
    }
}
