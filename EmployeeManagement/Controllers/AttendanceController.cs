using EmployeeManagement.RequestModal;
using EmployeeManagement.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [ApiController]
    [Route("api/attendance")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpGet("stat-cards")]
        public async Task<IActionResult> GetCards()
        {
            var result = await _attendanceService.GetStatCardsAsync();
            return Ok(result);
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetList([FromBody] AttendanceListReq req)
        {
            var result = await _attendanceService.GetListAsync(req);
            return Ok(result);
        }

        [HttpPost("bulk-update")]
        public async Task<IActionResult> BulkUpdate([FromBody] BulkAttendanceReq req)
        {
            await _attendanceService.BulkAttendanceAsync(req);
            return Ok();
        }

        [HttpPost("export")]
        public async Task<IActionResult> Export([FromBody] AttendanceListReq req)
        {
            byte[] fileBytes = await _attendanceService.ExportAsync(req);
            string contentType = "text/csv";
            string fileName = "report.csv";
            return File(fileBytes, contentType, fileName);
        }
    }
}
