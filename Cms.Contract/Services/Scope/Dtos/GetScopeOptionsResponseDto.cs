namespace Cms.Contract.Services.Scope.Dtos
{
    public class GetScopeOptionsResponseDto
    {
        public required Guid ScopeId { get; set; }

        public required string ScopeName { get; set; }

        public required string ScopeCode { get; set; }
    }
}
