using System.ComponentModel.DataAnnotations;

namespace GrpcAccountProfileService.Entities
{
    public class AccountProfile
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = null!;
        public string? FirstName { get; set; } = null!;
        public string? LastName { get; set; } = null!;
        public string? PhoneNumber { get; set; } = null!;

        public AccountProfileAddress? Address { get; set; }
    }
}
