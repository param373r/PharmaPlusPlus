namespace PharmaPlusPlus.Models.Contracts
{
    public record UpdateUserRequest
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}