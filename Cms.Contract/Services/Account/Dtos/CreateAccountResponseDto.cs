namespace Cms.Contract.Services.Account.Dtos
{
    public class CreateAccountResponseDto
    {
        public CreateAccountResult Result { get; set; }
    }

    public enum CreateAccountResult
    {
        Success,
        UsernameRequired,
        PasswordRequired,
        UsernameDuplicated,
        RoleAssignmentsRequired,
        RoleNotFound,
        DepartmentNotFound,
    }
}
