using EmployeeManagement.RequestModal;
using EmployeeManagement.ResponseModal;

namespace EmployeeManagement.Service.Interfaces
{
    public interface ISalaryService
    {
        Task<CommonPaginatedResponse<SalaryDto>> GetSalaryListAsync( string? search, string? department, int month, int year, int page, int pageSize);
        Task<bool> CreatePaymentAsync(SalaryReq req);
        Task<byte[]> ExportAsync( string? search, string? department, int month, int year);
    }
}
