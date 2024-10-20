using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileECommerce.Services;

namespace MobileECommerce.Pages;

public partial class MinhaContaPage : ContentPage
{
    private readonly ApiService _apiService;

    private const string NomeUsuarioKey = "usuarionome";
    private const string EmailUsuarioKey = "usuarioemail";
    private const string TelefoneUsuarioKey = "usuariotelefone";

    public MinhaContaPage(ApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        CarregarInformacoesUsuario();
        ImgBtnPerfil.Source = await GetImagemPerfilAsync();
    }

    private void CarregarInformacoesUsuario()
    {
        LblNomeUsuario.Text = Preferences.Get(NomeUsuarioKey, string.Empty);
        EntNome.Text = LblNomeUsuario.Text;
        EntEmail.Text = Preferences.Get(EmailUsuarioKey, string.Empty);
        EntFone.Text = Preferences.Get(TelefoneUsuarioKey, string.Empty);
    }

    private async Task<string?> GetImagemPerfilAsync()
    {
        string imagemPadrao = AppConfig.PerfilImagemPadrao;

        var (response, errorMessage) = await _apiService.GetImagemPerfilUsuario();

        if (errorMessage is not null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    await DisplayAlert("Erro", "N o autorizado", "OK");
                    return imagemPadrao;
                default:
                    await DisplayAlert("Erro", errorMessage ?? "N o foi poss vel obter a imagem.", "OK");
                    return imagemPadrao;
            }
        }

        if (response?.UrlImage is not null)
        {
            return response.CaminhoImagem;
        }
        return imagemPadrao;
    }

    private async void BtnSalvar_Clicked(object sender, EventArgs e)
    {
        // Salva as informa  es alteradas pelo usu rio nas prefer ncias
        Preferences.Set(NomeUsuarioKey, EntNome.Text);
        Preferences.Set(EmailUsuarioKey, EntEmail.Text);
        Preferences.Set(TelefoneUsuarioKey, EntFone.Text);
        await DisplayAlert("Informaes Salvas", "Suas informaes foram salvas com sucesso!", "OK");
    }
}
