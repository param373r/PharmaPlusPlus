
namespace PharmaPlusPlus.Models.Contracts
{
    public record LoginRequest
    {
        public string UserEmail { get; set; }
        public string Password { get; set; }
    }

    public record LoginResponse : Response
    {
        public string Token { get; set; }
    }
}
