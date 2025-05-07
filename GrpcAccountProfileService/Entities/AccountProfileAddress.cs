using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GrpcAccountProfileService.Entities
{
    public class AccountProfileAddress
    {
        [Key, ForeignKey(nameof(AccountProfile))]
        public string AccountProfileId { get; set; } = null!;
        public AccountProfile? AccountProfile { get; set; } = null!;
        public string StreetName { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string City { get; set; } = null!;
    }
}
