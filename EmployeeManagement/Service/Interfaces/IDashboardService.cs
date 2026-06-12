using EmployeeManagement.RequestModal;

namespace EmployeeManagement.Service.Interfaces
{
    public interface IDashboardService
    {
        Task<MainDashboardDto> GetDashboardAsync();
    }
}
