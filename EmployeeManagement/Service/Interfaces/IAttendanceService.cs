using EmployeeManagement.RequestModal;
using EmployeeManagement.ResponseModal;

namespace EmployeeManagement.Service.Interfaces
{
    public interface IAttendanceService
    {
        Task<AttendanceStatCardRes> GetStatCardsAsync();
        Task<CommonPaginatedResponse<AttendanceRes>> GetListAsync(AttendanceListReq req);
        Task<bool> BulkAttendanceAsync(BulkAttendanceReq req);
        Task<byte[]> ExportAsync(AttendanceListReq req);
    }
}
