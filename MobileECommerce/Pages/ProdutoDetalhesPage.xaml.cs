using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileECommerce.Models;
using MobileECommerce.Services;
using MobileECommerce.Validations;

namespace MobileECommerce.Pages;

public partial class ProdutoDetalhesPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private int _produtoId;
    private bool _loginPageDisplayed = false;
    private FavoritosService _favoritosService = new FavoritosService();
    private string _imagemUrl;

    public ProdutoDetalhesPage(int produtoId,
        string produtoNome,
        ApiService apiService,
        IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _produtoId = produtoId;
        Title = produtoNome ?? "Detalhes do Produto";
    }

    // Método chamado quando a página aparece
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetProdutoDetalhes(_produtoId);
        AtualizaFavoritoButton();
    }

    private async Task<Product?> GetProdutoDetalhes(int produtoId)
    {
        var (produtoDetalhe, errorMessage)
            = await _apiService.GetProdutoDetalhe(produtoId);

        if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
        {
            await DisplayLoginPage();
            return null;
        }

        // Verificar se houve algum erro na obtenção das produtos
        if (produtoDetalhe == null)
        {
            // Lidar com o erro, exibir mensagem ou logar
            await DisplayAlert("Erro",
                errorMessage ?? "Não foi possível obter o produto.", "OK");
            return null;
        }

        if (produtoDetalhe != null)
        {
            // Atualizar as propriedades dos controles com os dados do produto
            ImagemProduto.Source = produtoDetalhe.CaminhoImagem;
            LblProdutoNome.Text = produtoDetalhe.Name;
            LblProdutoPreco.Text = produtoDetalhe.Price.ToString();
            LblProdutoDescricao.Text = produtoDetalhe.Details;
            LblPrecoTotal.Text = produtoDetalhe.Price.ToString();
            _imagemUrl = produtoDetalhe.CaminhoImagem;
        }
        else
        {
            await DisplayAlert("Erro",
                errorMessage ??
                "Não foi possível obter os detalhes do produto.", "OK");
            return null;
        }

        return produtoDetalhe;
    }

    private void BtnRemove_OnClicked(object? sender, EventArgs e)
    {
        if (int.TryParse(LblQuantidade.Text, out int quantidade) &&
            decimal.TryParse(LblProdutoPreco.Text, out decimal precoUnitario))
        {
            // Decrementa a quantidade, e n o permite que seja menor que 1
            quantidade = Math.Max(1, quantidade - 1);
            LblQuantidade.Text = quantidade.ToString();

            // Calcula o pre o total
            var precoTotal = quantidade * precoUnitario;
            LblPrecoTotal.Text = precoTotal.ToString();
        }
        else
        {
            // Tratar caso as convers es falhem
            DisplayAlert("Erro", "Valores inv lidos", "OK");
        }
    }

    private void BtnAdiciona_OnClicked(object? sender, EventArgs e)
    {
        if (int.TryParse(LblQuantidade.Text, out int quantidade) &&
            decimal.TryParse(LblProdutoPreco.Text, out decimal precoUnitario))
        {
            // Incrementa a quantidade
            quantidade++;
            LblQuantidade.Text = quantidade.ToString();

            // Calcula o pre o total
            var precoTotal = quantidade * precoUnitario;
            LblPrecoTotal.Text = precoTotal.ToString(); // Formata como moeda
        }
        else
        {
            // Tratar caso as convers es falhem
            DisplayAlert("Erro", "Valores inv lidos", "OK");
        }
    }

    private async void BtnIncluirNoCarrinho_OnClicked(object? sender,
        EventArgs e)
    {
        try
        {
            var carrinhoCompra = new CarrinhoCompra()
            {
                Quantity = Convert.ToInt32(LblQuantidade.Text),
                UnitPrice = Convert.ToDecimal(LblProdutoPreco.Text),
                Total = Convert.ToDecimal(LblPrecoTotal.Text),
                ProductId = _produtoId,
                ClientId = Preferences.Get("usuarioid", 0)
            };
            var response
                = await _apiService.AdicionaItemNoCarrinho(carrinhoCompra);
            if (response.Data)
            {
                await DisplayAlert("Sucesso", "Item adicionado ao carrinho !",
                    "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Erro",
                    $"Falha ao adicionar item: {response.ErrorMessage}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "OK");
        }
    }

    private async void ImagemBtnFavorito_OnClicked(object? sender, EventArgs e)
    {
        try
        {
            var existeFavorito = await _favoritosService.ReadAsync(_produtoId);
            if (existeFavorito is not null)
            {
                await _favoritosService.DeleteAsync(existeFavorito);
            }
            else
            {
                var produtoFavorito = new ProdutoFavorito()
                {
                    ProdutoId = _produtoId,
                    IsFavorito = true,
                    Detalhe = LblProdutoDescricao.Text,
                    Nome = LblProdutoNome.Text,
                    Preco = Convert.ToDecimal(LblProdutoPreco.Text),
                    ImagemUrl = _imagemUrl
                };

                await _favoritosService.CreateAsync(produtoFavorito);
            }

            AtualizaFavoritoButton();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "OK");
        }
    }

    private async void AtualizaFavoritoButton()
    {
        var existeFavorito = await
            _favoritosService.ReadAsync(_produtoId);

        if (existeFavorito is not null)
            ImagemBtnFavorito.Source = "heartfill";
        else
            ImagemBtnFavorito.Source = "heart";
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;

        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}
