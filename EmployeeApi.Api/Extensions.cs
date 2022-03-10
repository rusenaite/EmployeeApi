using EmployeeApi.Api.Dtos;
using EmployeeApi.Api.Entities;

namespace EmployeeApi.Api
{
    public static class Extensions
    {
        public static EmployeeDto AsDto(this Employee employee)
        {
            return new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                BirthDate = employee.BirthDate,
                EmploymentDate = employee.EmploymentDate,
                Boss = employee.Boss,
                HomeAddress = employee.HomeAddress,
                CurrentSalary = employee.CurrentSalary,
                Role = employee.Role
            };
        }
    }
}