namespace Cms.Contract.Repositories.Scope.Entities
{
    public class ScopeOptionEntity
    {
        public required Guid ScopeId { get; set; }

        public required string ScopeName { get; set; }

        public required string ScopeCode { get; set; }
    }
}
