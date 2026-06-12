//using EmployeeManagement.ApplicationContext;
//using EmployeeManagement.Model;
//using EmployeeManagement.RequestModal;
//using EmployeeManagement.ResponseModal;
//using Microsoft.EntityFrameworkCore;
//using Org.BouncyCastle.Crypto.Generators;
//using RestAPI.Exceptions;
//using RestAPI.Service;
//using System.Data;

//namespace EmployeeManagement.Service
//{
//    public interface IEmployeeService
//    {
//        Task<bool> CreateRegister(AdminRm req);
//        Task<bool> Login(LoginReq req);
//        Task<bool> CreateEmployeeAsync(EmployeeManagementRm req);
//        Task<CommonPaginatedResponse<EmployeeDto>> GetOverviewEmployees(string? search,string? department,int page = 1,int pageSize = 4);
//        Task<byte[]> GenerateEmployeeCsvReport(string? category = null, string? search = null);
//        Task<Employee> GetEmployeeByIdAsync(string employeeId);
//        Task<bool> UpdateEmployeeAsync(string employeeId, EmployeeManagementRm req);
//        Task<AttendanceSummaryResponse> AttendenceDashboardSummary();
//        Task<CommonPaginatedResponse<AttendanceRm>> GetAttendanceDashboard(string? search,string? department,string? status,DateTime? date,int page = 1,int pageSize = 4);
//        Task<bool> UpdateBulkAttendanceAsync(BulkUpdateAttendanceRm req);
//        Task<CommonPaginatedResponse<PayrollRm>> GetPayrollDashboardAsync(string? search,string? status, int page = 1,int pageSize = 10);
//        Task<MainDashboardDto> GetMainDashboardDataAsync();
//        Task<byte[]> GeneratePaymentCsvReport(string? category = null, string? search = null);
//    }
//    public class EmployeeService : IEmployeeService
//    {
//        private readonly AppDbContext _context;
    
//        public EmployeeService(AppDbContext context)
//        {
//            _context = context;
           
//        }
//        public async Task<bool> CreateRegister(AdminRm req)
//        {
//            if (req.Password != req.ConfirmPassword)
//            {
//                return false; 
//            }
//            var existingAdmin = await _context.Admins
//                .AnyAsync(p => p.Email == req.Email && !p.IsDeleted);

//            if (existingAdmin)
//                throw new AppExceptions("Email already registered.", 409);



//            var admin = new Admin
//            {
//                FullName = req.FullName,
//                Email = req.Email,
//                Password = BCrypt.Net.BCrypt.HashPassword(req.Password)
//            };

//            _context.Admins.Add(admin);

//             await _context.SaveChangesAsync();
//            return true;
//        }

//        public async Task<bool> Login(LoginReq req)
//        {
//            var admin = await _context.Admins
//                .FirstOrDefaultAsync(a => a.Email == req.Email);

//            if (admin == null)
//            {
//                return false;
//            }
//            if (admin.Password != req.Password)
//            {
//                return false;
//            }
//            return true;
//        }



//        public async Task<bool> CreateEmployeeAsync(EmployeeManagementRm req)
//        {


//            bool exists = await _context.Employees
//                .AnyAsync(u => u.Email == req.Email);

//            if (exists)
//                throw new AppExceptions("Employee with this Email already exists.", 409);

//            bool eexists = await _context.Employees
//                .AnyAsync(u => u.EmployeeId == req.EmployeeId);

//            if (eexists)
//                throw new AppExceptions("Employee with EmployeeId already exists.", 409);

//            bool pexists = await _context.Employees
//               .AnyAsync(u => u.PhoneNumber == req.PhoneNumber);

//            if (pexists)
//                throw new AppExceptions("Employee with this PhoneNo already exists.", 409);


//            var entity = new Employee
//            {
//                FirstName = req.FirstName,
//                LastName = req.LastName,
//                EmployeeId = req.EmployeeId,
//                Email = req.Email,
//                DateofBirth = req.DateofBirth,
//                Department = req.Departiment,
//                PhoneNumber = req.PhoneNumber,
//                Gender = req.Gender,
//                MonthlySalary = req.MonthlySalary,
//                JoiningDate = req.JoiningDate,
//                CreatedAt = DateTime.UtcNow
//            };

//            await _context.Employees.AddAsync(entity);
//            await _context.SaveChangesAsync();
//            return true;

//        }


//        //GetallEmployee

//        public async Task<CommonPaginatedResponse<EmployeeDto>> GetOverviewEmployees(
//    string? search,
//    string? department,
//    int page = 1,
//    int pageSize = 4)
//        {
//            var query = _context.Employees.AsQueryable();

//            if (!string.IsNullOrWhiteSpace(search))
//            {
//                query = query.Where(e => e.FirstName.Contains(search) || e.Email.Contains(search));
//            }

//            if (!string.IsNullOrWhiteSpace(department) && department != "All Departments")
//            {
//                query = query.Where(e => e.Department.ToString() == department);
//            }

//            int totalCount = await query.CountAsync();

//            var employees = await query
//                .Skip((page - 1) * pageSize)
//                .Take(pageSize)
//                .Select(e => new EmployeeDto
//                {
//                    EmployeeId = e.EmployeeId,
//                    Name = $"{e.FirstName} {e.LastName}",
//                    Department = e.Department.ToString(),
//                    Email = e.Email
//                })
//                .ToListAsync();

//            return new CommonPaginatedResponse<EmployeeDto>(employees, totalCount, page, pageSize);
//        }

//        public async Task<bool> SoftDeleteAsync(List<Guid> ids)
//        {
//            if (ids == null || !ids.Any())
//                throw new AppExceptions("Not Found", 400);

//            var affectedRows = await _context.Employees
//                .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
//                .ExecuteUpdateAsync(s => s
//                    .SetProperty(x => x.IsDeleted, true)
//                    .SetProperty(x => x.DeletedAt, DateTime.UtcNow));

//            return true;
//        }

//        public async Task<byte[]> GenerateEmployeeCsvReport(string? category = null, string? search = null)
//        {
//            var indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

//            var resultWrapper = await GetOverviewEmployees(search, category, 1, int.MaxValue);

//            var finaldata = resultWrapper.Data.Select(s => new
//            {
//                s.EmployeeId,
//                s.Name,
//                s.Department, 
//                s.Email
//            }).ToList();

//            return CsvExporter.ToCsv(finaldata);
//        }


//        public async Task<Employee> GetEmployeeByIdAsync(Guid employeeId)
//        {
//            var employee = await _context.Employees
//                .FirstOrDefaultAsync(e => e.Id == employeeId);


//            if (employee == null)
//                throw new AppExceptions("Employee not found.", 404);

//            return employee;
//        }

//        public async Task<bool> UpdateEmployeeAsync(Guid employeeId, EmployeeManagementRm req)
//        {
//            var employee = await _context.Employees
//                .FirstOrDefaultAsync(e => e.Id == employeeId);

//            if (employee == null)
//                throw new AppExceptions("Employee not found.", 404);

//            bool emailExists = await _context.Employees
//                .AnyAsync(e => e.Email == req.Email);
//            if (emailExists)
//                throw new AppExceptions("Employee with this Email already exists.", 409);

//            bool phoneExists = await _context.Employees
//                .AnyAsync(e => e.PhoneNumber == req.PhoneNumber );
//            if (phoneExists)
//                throw new AppExceptions("Employee with this PhoneNo already exists.", 409);

//            employee.FirstName = req.FirstName;
//            employee.LastName = req.LastName;
//            employee.Email = req.Email;
//            employee.DateofBirth = req.DateofBirth;
//            employee.Department = req.Departiment; 
//            employee.PhoneNumber = req.PhoneNumber;
//            employee.Gender = req.Gender;
//            employee.MonthlySalary = req.MonthlySalary;
//            employee.JoiningDate = req.JoiningDate;
//            employee.UpdatedAt = DateTime.UtcNow; 

//            _context.Employees.Update(employee);
//            await _context.SaveChangesAsync();
//            return true;
//        }


//        //  Attendence


//        public async Task<AttendanceSummaryResponse> AttendenceDashboardSummary()
//        {
//            var filterDate = DateTime.UtcNow.Date;
//            var oneMonthAgo = filterDate.AddMonths(-1);

//            var totalEmployees = await _context.Employees.CountAsync();

//            var presentCount = await _context.Attendences
//                .CountAsync(a => a.AttendenceUpdateTime.Date == filterDate && a.Status == AttendenceStatus.Present);

//            var absentCount = await _context.Attendences
//                .CountAsync(a => a.AttendenceUpdateTime.Date == filterDate && a.Status == AttendenceStatus.Absent);

//            var continuousLeaveCount = await _context.Employees
//                .CountAsync(e =>
//                    _context.Attendences.Any(a => a.EmployeeId == e.Id && a.AttendenceUpdateTime.Date >= oneMonthAgo && a.Status == AttendenceStatus.Leave)
//                );

//            return new AttendanceSummaryResponse
//            {
//                TotalEmployees = totalEmployees,
//                PresentCount = presentCount,
//                AbsentCount = absentCount,
//                ContinuousLeaveCount = continuousLeaveCount
//            };
//        }


//        public async Task<CommonPaginatedResponse<AttendanceRm>> GetAttendanceDashboard(
//    string? search,
//    string? department,
//    string? status,
   
//    int page = 1,
//    int pageSize = 4)
//        {
//            var filterDate =  DateTime.UtcNow.Date;

//            var query = _context.Employees
//                .Include(e => e.Attendence)
//                .AsQueryable();

//            if (!string.IsNullOrWhiteSpace(search))
//            {
//                query = query.Where(e => e.FirstName.Contains(search) || e.LastName.Contains(search) || e.EmployeeId.Contains(search));
//            }

//            if (!string.IsNullOrWhiteSpace(department) && department != "All Departments")
//            {
//                query = query.Where(e => e.Department.ToString() == department);
//            }

//            if (!string.IsNullOrWhiteSpace(status) && status != "All")
//            {
//                query = query.Where(e => e.Attendence != null && e.Attendence.Status.ToString() == status);
//            }

           
//            int totalFilteredCount = await query.CountAsync();

//            var rows = await query
//                .Skip((page - 1) * pageSize)
//                .Take(pageSize)
//                .Select(e => new AttendanceRm
//                {
//                    EmployeeId = e.EmployeeId,
//                    Name = $"{e.FirstName} {e.LastName}",
//                    Department = e.Department.ToString(),
//                    Date = filterDate,
//                    Status = e.Attendence != null && e.Attendence.AttendenceUpdateTime.Date == filterDate
//                        ? e.Attendence.Status.ToString()
//                        : "Absent" 
//                })
//                .ToListAsync();

//            var response = new CommonPaginatedResponse<AttendanceRm>(rows, totalFilteredCount, page, pageSize);

           


//            return response;
//        }

//        public async Task<bool> UpdateBulkAttendanceAsync(BulkUpdateAttendanceRm req)
//        {
//            var targetDate = req.Date.Date;

//            foreach (var empId in req.EmployeeIds)
//            {
//                var employee = await _context.Employees
//                    .Include(e => e.Attendence)
//                    .FirstOrDefaultAsync(e => e.EmployeeId == empId);

//                if (employee == null) continue;


//                var attendance = await _context.Attendences
//                    .FirstOrDefaultAsync(a => a.EmployeeId == employee.Id && a.AttendenceUpdateTime.Date == targetDate);

//                if (attendance != null)
//                {
                   
//                    var newAttendance = new Attendence
//                    {
//                        EmployeeId = employee.Id,
//                        Status = req.Status,
//                        AttendenceUpdateTime = DateTime.Now
//                    };
//                    await _context.Attendences.AddAsync(newAttendance);
//                }
//            }

//            await _context.SaveChangesAsync();
//            return true;
//        }


//        //dashboard

//        public async Task<MainDashboardDto> GetMainDashboardDataAsync()
//        {
//            var today = DateTime.UtcNow.Date;
//            var currentYear = today.Year;
//            var threeMonthsAgo = today.AddMonths(-3);

//            var totalEmployees = await _context.Employees.CountAsync();

//            var paidThisMonth = await _context.Employees
//                .Where(e => e.JoiningDate.Month == today.Month && e.JoiningDate.Year == today.Year)
//                .SumAsync(e => e.MonthlySalary);

//            var newJoineesCount = await _context.Employees
//                .CountAsync(e => e.JoiningDate.Month == today.Month && e.JoiningDate.Year == today.Year);

//            var presentTodayCount = await _context.Attendences
//                .CountAsync(a => a.AttendenceUpdateTime.Date == today && a.Status == AttendenceStatus.Present);


//            var departmentHub = await _context.Employees
//                .GroupBy(e => e.Department)
//                .Select(g => new DepartmentCountDto
//                {
//                    DepartmentName = g.Key.ToString(),
//                    Headcount = g.Count() 
//                })
//                .ToListAsync();


//            var recentJoinees = await _context.Employees
//                .Where(e => e.JoiningDate >= threeMonthsAgo)
//                .OrderByDescending(e => e.JoiningDate)
//                .Take(5) 
//                .Select(e => new RecentJoineeDto
//                {
//                    Name = $"{e.FirstName} {e.LastName}",
//                    Email = e.Email,
//                    Department = e.Department.ToString()
//                })
//                .ToListAsync();

//            return new MainDashboardDto
//            {
//                TotalEmployees = totalEmployees,
//                PaidThisMonth = paidThisMonth,
//                NewJoinees = newJoineesCount,
//                PresentToday = presentTodayCount,
//                DepartmentHub = departmentHub,
//                RecentNewJoinees = recentJoinees,
//            };
//        }



//        //Salary

//        public async Task<CommonPaginatedResponse<PayrollRm>> GetPayrollDashboardAsync(string? search,string? status, int page = 1,int pageSize = 10)         {
//            var query = _context.Payments
//                .Include(p => p.Employee)
//                .AsQueryable();
            
//            if (!string.IsNullOrWhiteSpace(search))
//            {
//                query = query.Where(p => p.Employee.EmployeeId.Contains(search) ||
//                                         p.Employee.FirstName.Contains(search) ||
//                                         p.Employee.LastName.Contains(search));
//            }
            
//            if (!string.IsNullOrWhiteSpace(status) && status != "All")
//            {
//                if (Enum.TryParse<PaymentStatus>(status, true, out var parsedStatus))
//                {
//                    query = query.Where(p => p.Status == parsedStatus);
//                }
//            }

//            int totalCount = await query.CountAsync();
            
//            var rows = await query
//                .Skip((page - 1) * pageSize)
//                .Take(pageSize)
//                .Select(p => new PayrollRm
//                {
//                    EmployeeId = p.Employee.EmployeeId,
//                    FullName = $"{p.Employee.FirstName} {p.Employee.LastName}",
//                    Department = p.Employee.Department.ToString(),
//                    Amount = p.Amount,
//                    PaymentDate = p.Status == PaymentStatus.Paid ? p.PaymentDate : null, 
//                    Status = p.Status.ToString()
//                })
//                .ToListAsync();

//            return new CommonPaginatedResponse<PayrollRm>(rows, totalCount, page, pageSize);
//        }



//        public async Task<bool> SoftDeletePaymentAsync(List<Guid> ids)
//        {
//            if (ids == null || !ids.Any())
//                throw new AppExceptions("Not Found", 400);

//            var affectedRows = await _context.Payments
//                .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
//                .ExecuteUpdateAsync(s => s
//                    .SetProperty(x => x.IsDeleted, true)
//                    .SetProperty(x => x.DeletedAt, DateTime.UtcNow));

//            return true;
//        }

//        public async Task<byte[]> GeneratePaymentCsvReport(string? category = null, string? search = null)
//        {
//            var indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

//            var resultWrapper = await GetPayrollDashboardAsync(search, category, 1, int.MaxValue);

//            var finaldata = resultWrapper.Data.Select(s => new
//            {
//                s.EmployeeId,
//                s.FullName,
//                s.Department,
//                s.Amount,
//                s.PaymentDate,
//                s.Status
//            }).ToList();

//            return CsvExporter.ToCsv(finaldata);
//        }

//    }
//}
