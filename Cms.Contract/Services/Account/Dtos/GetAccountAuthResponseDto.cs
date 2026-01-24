namespace Cms.Contract.Services.Account.Dtos
{
    public class GetAccountAuthResponseDto
    {
        public Guid AccountId { get; set; }

        public required string Username { get; set; }

        public required string PasswordHash { get; set; } 

        public required List<string> RoleCodes { get; set; } // ToList() 鐵定不為NULL

        public required List<string> PermissionCodes { get; set; }  // ToList() 鐵定不為NULL
    }
}
