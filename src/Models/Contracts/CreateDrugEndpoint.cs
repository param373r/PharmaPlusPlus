namespace PharmaPlusPlus.Models.Contracts;

public record CreateDrugRequest
{
    public string DrugName { get; set; }
    public string DrugDescription { get; set; }
    public double DrugPrice { get; set; }
    public long DrugQuantityAvailable { get; set; }
}

public record CreateDrugResponse
{
    public Guid DrugId { get; set; }
}