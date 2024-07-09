
namespace PharmaPlusPlus.Models
{
    public class Cart
    {
        public Guid UserCartId { get; set; }
        public Dictionary<Guid, int> QuantityByDrugs { get; set; }
        public Dictionary<Guid, double> TotalPriceByDrugs { get; set; }
        public double TotalCartAmount { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
    }
}
