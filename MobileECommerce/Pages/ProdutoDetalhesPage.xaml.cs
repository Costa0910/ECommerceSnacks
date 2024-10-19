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
        throw new NotImplementedException();
    }

    private void BtnAdiciona_OnClicked(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void BtnIncluirNoCarrinho_OnClicked(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void ImagemBtnFavorito_OnClicked(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;

        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}
