namespace PharmaPlusPlus.Models.Contracts;

public record Response
{
    public bool IsSuccess { get; set; } = true;
    public string Message { get; set; }
}