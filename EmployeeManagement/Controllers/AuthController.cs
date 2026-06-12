using EmployeeManagement.RequestModal;
using EmployeeManagement.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(AdminRm req)
        {
            var result = await _authService.RegisterAsync(req);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginReq req)
        {
            var result = await _authService.LoginAsync(req);
            return Ok(result);
        }
    }
}
