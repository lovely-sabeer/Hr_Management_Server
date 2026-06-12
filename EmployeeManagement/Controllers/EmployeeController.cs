using EmployeeManagement.RequestModal;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Service.Interfaces;

namespace EmployeeManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] EmployeeManagementRm request)
        {
            var result = await _employeeService.CreateAsync(request);

            return Ok(new
            {
                Success = result,
                Message = "Employee created successfully."
            });
        }

        [HttpGet("get-by-id/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            return Ok(employee);
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetList([FromQuery] string? search,string? department,int page = 1,int pageSize = 10)
        {
            var result = await _employeeService.GetListAsync(search,department,page,pageSize);
            return Ok(result);
        }

        [HttpPost("update/{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] EmployeeManagementRm request)
        {
            var result = await _employeeService.UpdateAsync(id, request);

            return Ok(new
            {
                Success = result,
                Message = "Employee updated successfully."
            });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] List<Guid> ids)
        {
            var result = await _employeeService.DeleteListAsync(ids);

            return Ok(new
            {
                Success = result,
                Message = "Employees deleted successfully."
            });
        }

        [HttpGet("export")]
        public async Task<IActionResult> Export([FromQuery] string? department, string? search)
        {
            var fileBytes = await _employeeService.ExportAsync(department, search);
            return File(fileBytes,"text/csv","report.csv");
        }
    }
}
