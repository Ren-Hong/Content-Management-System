namespace Cms.Contract.Services.Account.Dtos
{
    public class CreateAccountRequestDto
    {
        public required string Username { get; set; }

        public required string Password { get; set; }

        public required List<AccountRoleAssignmentRequestDto> RoleAssignments { get; set; }
    }

    public class AccountRoleAssignmentRequestDto
    {
        public required Guid RoleId { get; set; }

        public required List<Guid> DepartmentIds { get; set; }
    }

}
