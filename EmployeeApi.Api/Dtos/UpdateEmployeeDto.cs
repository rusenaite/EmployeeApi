using System.ComponentModel.DataAnnotations;
using EmployeeApi.Api.Validators;
using ExpressiveAnnotations.Attributes;

namespace EmployeeApi.Api.Dtos
{
    public record UpdateEmployeeDto
    {
        [Required]
        [StringLength(50)]
        [RegularExpression(@"^([A-Z][a-z]+[,.]?[ ]?|[a-z]+['-]?)+$")]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^([A-Z][a-z]+[,.]?[ ]?|[a-z]+['-]?)+$")]
        public string? LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [ValidAgeAttribute]
        public DateTime BirthDate { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        [AssertThat("ToDate('2000/1/1') <= EmploymentDate")]
        [PastDate]
        public DateTime EmploymentDate { get; set; }

        [Required]
        [RegularExpression(@"(\d{1,}) [a-zA-Z0-9\s]+.+[a-zA-Z]")]
        public string? HomeAddress { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double CurrentSalary { get; set; }

        [Required]
        public string? Role { get; set; }
    }
}