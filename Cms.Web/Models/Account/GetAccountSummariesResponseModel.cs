namespace Cms.Web.Models.Account
{
    public class GetAccountSummariesResponseModel
    {
        public string Username { get; set; }

        public string Status { get; set; }

        public List<string> Roles { get; set; }
    }
}
