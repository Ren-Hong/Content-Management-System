namespace Cms.Web.Controllers.Scope.Models
{
    public class GetScopeOptionsResponseModel
    {
        public required Guid ScopeId { get; set; }

        public required string ScopeName { get; set; }

        public required string ScopeCode { get; set; }
    }
}
