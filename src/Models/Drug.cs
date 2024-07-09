namespace PharmaPlusPlus.Models
{
    public class Drug
    {
        public Guid Id { get; set; }
        public string DrugName { get; set; }
        public string DrugDescription { get; set; }
        public double DrugPrice { get; set; }
        public long DrugQuantityAvailable { get; set; }
    }
}
