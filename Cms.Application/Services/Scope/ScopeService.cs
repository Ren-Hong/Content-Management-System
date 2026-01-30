using Cms.Contract.Repositories.Scope.Interfaces;
using Cms.Contract.Services.Scope.Dtos;
using Cms.Contract.Services.Scope.Interfaces;
using Cms.Contract.Services.UnitOfWork.Interfaces;

namespace Cms.Application.Services.Scope
{
    public class ScopeService : IScopeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IScopeRepository _scopeRepository;

        public ScopeService
        (
            IUnitOfWork unitOfWork,
            IScopeRepository scopeRepository
        )
        {
            _unitOfWork = unitOfWork;
            _scopeRepository = scopeRepository;
        }

        public async Task<List<GetScopeOptionsResponseDto>> GetScopeOptionsAsync()
        {
            var rows = await _scopeRepository.GetScopeOptionsAsync();

            return rows
                .Select(x => new GetScopeOptionsResponseDto
                {
                    ScopeId = x.ScopeId,
                    ScopeCode = x.ScopeCode,
                    ScopeName = x.ScopeName
                })
                .ToList();
        }

    }
}
