namespace MobileECommerce.Models;

public class PedidoDetalhe
{
    public int Id { get; set; }

    public int Quantity { get; set; }

    public decimal SubTotal { get; set; }

    public string? ProductName { get; set; }

    public string? ProductImage { get; set; }

    public string CaminhoImagem => AppConfig.BaseUrl + ProductImage;

    public decimal Price { get; set; }
}
