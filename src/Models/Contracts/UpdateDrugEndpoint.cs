namespace PharmaPlusPlus.Models.Contracts;

public record UpdateDrugRequest
{
    public string DrugName { get; set; }
    public string DrugDescription { get; set; }
    public double? DrugPrice { get; set; }
    public long? DrugQuantityAvailable { get; set; }

}