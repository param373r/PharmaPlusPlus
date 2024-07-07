
namespace PharmaPlusPlus.Models.Contracts
{
    public record RegisterRequest
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string UserEmail { get; set; }
    }

    public record RegisterAdminRequest : RegisterRequest
    {
        public Guid? Id { get; set; }
        public Role? Role { get; set; }
    }

    public record RegisterResponse : Response
    {
        public Guid UserId { get; set; }
    }
}
