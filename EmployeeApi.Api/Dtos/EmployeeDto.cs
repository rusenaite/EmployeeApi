using System.ComponentModel.DataAnnotations;
using EmployeeApi.Api.Entities;
using EmployeeApi.Api.Validators;
using ExpressiveAnnotations.Attributes;

namespace EmployeeApi.Api.Dtos
{
    public record EmployeeDto
    {
        //private const string todaysDate = DateTime.Today.ToString();
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^([A-Z][a-z]+[,.]?[ ]?|[a-z]+['-]?)+$")]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^([A-Z][a-z]+[,.]?[ ]?|[a-z]+['-]?)+$")]
        public string? LastName { get; set; }

        [DataType(DataType.Date)]
        [ValidAgeAttribute]
        public DateTime BirthDate { get; set; }
        
        [DataType(DataType.Date)]
        [AssertThat("'1/1/2000' <= EmploymentDate")]
        [PastDate]
        public DateTime EmploymentDate { get; set; }

        [Required]
        public Boss? Boss { get; set; }

        [Required]
        [RegularExpression(@"(\d{1,}) [a-zA-Z0-9\s]+.+[a-zA-Z]")]
        public string? HomeAddress { get; set; }

        [Range(0, double.MaxValue)]
        public double CurrentSalary { get; set; }

        [Required]
        public string? Role { get; set; }
    }
}
