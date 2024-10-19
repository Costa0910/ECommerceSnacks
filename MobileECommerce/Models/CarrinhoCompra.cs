namespace MobileECommerce.Models;

public class CarrinhoCompra
{
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Total { get; set; }
    public int ProductId { get; set; }
    public int ClientId { get; set; }
}
