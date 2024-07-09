namespace PharmaPlusPlus.Models;

public class Order
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Dictionary<Guid, int> QuantityByDrugs { get; set; }
    public Dictionary<Guid, double> TotalPriceByDrugs { get; set; }
    public double TotalOrderPrice { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus OrderStatus { get; set; } = OrderStatus.ORDER_PLACED;
}
