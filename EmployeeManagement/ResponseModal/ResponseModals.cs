using EmployeeManagement.Model;

namespace EmployeeManagement.ResponseModal
{
    public class SalaryDto
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }

        public decimal Amount { get; set; }

        public DateTime? PaidDate { get; set; }

        public PaymentStatus Status { get; set; }
    }
}
