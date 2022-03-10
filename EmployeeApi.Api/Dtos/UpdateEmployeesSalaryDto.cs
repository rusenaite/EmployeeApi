using System.ComponentModel.DataAnnotations;

namespace EmployeeApi.Api.Dtos
{
    public record UpdateEmployeeSalaryDto
    {
        [Required]
        [Range(0, 4000)] 
        public double CurrentSalary { get; set; }
    }
}
