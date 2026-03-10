using Cms.Contract.Repositories.Department.Interfaces;
using Cms.Contract.Services.Department.Dtos;
using Cms.Contract.Services.Department.Interfaces;
using Cms.Contract.Services.UnitOfWork.Interfaces;

namespace Cms.Application.Services.Department
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(
            IUnitOfWork unitOfWork,
            IDepartmentRepository departmentRepository
        )
        {
            _unitOfWork = unitOfWork;
            _departmentRepository = departmentRepository;
        }

        public async Task<List<GetDepartmentOptionsResponseDto>> GetDepartmentOptionsAsync()
        {
            var rows = await _departmentRepository.GetDepartmentOptionsAsync();

            return rows
                .Select(x => new GetDepartmentOptionsResponseDto
                {
                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.DepartmentName
                })
                .ToList();
        }

        public async Task<List<GetDepartmentsForSidebarResponseDto>> GetDepartmentsForSidebarAsync()
        {
            var rows = await _departmentRepository.GetDepartmentsForSidebarAsync();

            return rows
                .Select(x => new GetDepartmentsForSidebarResponseDto
                {
                    DepartmentId = x.DepartmentId,
                    DepartmentCode = x.DepartmentCode,
                    DepartmentName = x.DepartmentName
                })
                .ToList();
        }
    }
}
