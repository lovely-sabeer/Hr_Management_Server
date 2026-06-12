using EmployeeManagement.RequestModal;

namespace EmployeeManagement.Service.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(AdminRm req);
        Task<LoginRes> LoginAsync(LoginReq req);
    }
}
