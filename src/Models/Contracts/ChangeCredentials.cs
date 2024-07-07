namespace PharmaPlusPlus.Models.Contracts;

public record ChangePasswordRequest
{
    public string UserEmail { get; init; }
    public string OldPassword { get; init; }
    public string NewPassword { get; init; }
}

public record ChangeEmailRequest
{
    public string NewEmail { get; init; }
}