namespace PharmaPlusPlus.Models.Contracts
{
    public class AddCartRequest
    {
        public Guid DrugId { get; set; }
        public int RequiredQuantity { get; set; }
    }
}
