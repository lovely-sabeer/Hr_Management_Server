using EmployeeManagement.ApplicationContext;
using EmployeeManagement.Model;
using EmployeeManagement.RequestModal;
using EmployeeManagement.ResponseModal;
using EmployeeManagement.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using RestAPI.Exceptions;
using RestAPI.Service;
using SQLitePCL;

namespace EmployeeManagement.Service.Implementations
{
    public class SalaryService : ISalaryService
    {
        AppDbContext _context;
        public SalaryService(AppDbContext context) 
        {
            _context = context;
        }
        public async Task<CommonPaginatedResponse<SalaryDto>> GetSalaryListAsync(string? search,string? department,int month,int year,int page,int pageSize)
        {
            var query = _context.Employees
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.FirstName.Contains(search) ||
                    x.LastName.Contains(search) ||
                    x.EmployeeId.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(department) && department != "All Departments")
            {
                query = query.Where(x => x.Department.ToString() == department);
            }

            var totalCount = await query.CountAsync();

            var employees = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new List<SalaryDto>();

            foreach (var employee in employees)
            {
                var payment = await _context.Payments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.EmployeeId == employee.Id &&
                        x.PaymentDate.Month == month &&
                        x.PaymentDate.Year == year);

                var previousMonth = new DateTime(year, month, 1)
                    .AddMonths(-1);

                var leaveDays = await _context.Attendences
                    .CountAsync(x =>
                        x.EmployeeId == employee.Id &&
                        x.AttendenceUpdateTime.Month == previousMonth.Month &&
                        x.AttendenceUpdateTime.Year == previousMonth.Year &&
                        x.Status == AttendenceStatus.Absent);

                decimal perDaySalary = employee.MonthlySalary / 30;

                decimal deduction = perDaySalary * leaveDays;

                decimal finalSalary =
                    employee.MonthlySalary - deduction;

                result.Add(new SalaryDto
                {
                    EmployeeId = employee.Id,
                    EmployeeCode = employee.EmployeeId,
                    EmployeeName = $"{employee.FirstName} {employee.LastName}",
                    Department = employee.Department.ToString(),
                    Amount = finalSalary,
                    PaidDate = payment?.PaymentDate,
                    Status = payment?.Status ?? PaymentStatus.Unpaid
                });
            }

            return new CommonPaginatedResponse<SalaryDto>(
                result,
                totalCount,
                page,
                pageSize);
        }
        public async Task<bool> CreatePaymentAsync(SalaryReq req)
        {
            if (req.EmployeeIds == null || !req.EmployeeIds.Any())
                throw new AppExceptions("Employee list required.", 400);

            var payments = new List<Payment>();

            foreach (var employeeId in req.EmployeeIds)
            {
                bool alreadyExists = await _context.Payments
                    .AnyAsync(x =>
                        x.EmployeeId == employeeId &&
                        x.PaymentDate.Month == req.PaymentDate.Month &&
                        x.PaymentDate.Year == req.PaymentDate.Year);

                if (alreadyExists)
                    continue;

                var employee = await _context.Employees
                    .FirstOrDefaultAsync(x => x.Id == employeeId);

                if (employee == null)
                    continue;

                var previousMonth =
                    req.PaymentDate.AddMonths(-1);

                var leaveDays = await _context.Attendences
                    .CountAsync(x =>
                        x.EmployeeId == employeeId &&
                        x.AttendenceUpdateTime.Month == previousMonth.Month &&
                        x.AttendenceUpdateTime.Year == previousMonth.Year &&
                        x.Status == AttendenceStatus.Absent);

                decimal deduction =
                    (employee.MonthlySalary / 30) * leaveDays;

                decimal finalSalary =
                    employee.MonthlySalary - deduction;

                payments.Add(new Payment
                {
                    EmployeeId = employeeId,
                    Amount = finalSalary,
                    PaymentDate = req.PaymentDate,
                    Status = req.Status,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (payments.Any())
            {
                await _context.Payments.AddRangeAsync(payments);
                await _context.SaveChangesAsync();
            }

            return true;
        }
        public async Task<byte[]> ExportAsync(string? search,string? department,int month,int year)
        {
            var data = await GetSalaryListAsync(
                search,
                department,
                month,
                year,
                1,
                int.MaxValue);

            var exportData = data.Data.Select(x => new
            {
                x.EmployeeCode,
                x.EmployeeName,
                x.Department,
                x.Amount,
                x.Status,
                x.PaidDate
            });

            return CsvExporter.ToCsv(exportData);
        }
    }
}
