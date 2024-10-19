namespace MobileECommerce.Models;

public class ImagemPerfil
{
    public string? UrlImage { get; set; }
    public string? CaminhoImagem => AppConfig.BaseUrl + UrlImage;
}
