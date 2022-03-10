using Microsoft.AspNetCore.Mvc;
using EmployeeApi.Api.Repositories;
using EmployeeApi.Api.Dtos;
using EmployeeApi.Api.Entities;

namespace EmployeeApi.Api.Controllers
{
    [ApiController]
    [Route("employees")]
    public class EmployeesController : ControllerBase
    {
        private Boss defaultCeo;
        private Boss defaultBoss;

        private readonly IEmployeesRepository repository;
        private readonly ILogger<EmployeesController> logger;

        public EmployeesController(IEmployeesRepository repository, ILogger<EmployeesController> logger)
        {
            this.repository = repository;
            this.logger = logger;

            defaultCeo = new Boss("Nick", "Hamilton", new DateTime(2000, 1, 30));
            defaultBoss = new Boss("Tom", "Rogers", new DateTime(2012, 6, 15), defaultCeo);
        }

        // GET /employees
        [HttpGet]
        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            var employees = (await repository.GetAllEmployeesAsync())
                        .Select(employee => employee.AsDto());

            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved {employees.Count()} items");

            return employees;
        }

        // GET /employees/byid/{id}
        [HttpGet("byid/{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await repository.GetEmployeeByIdAsync(id);

            if(employee is null)
            {
                return NotFound();
            }

            return employee.AsDto();
        }

        // GET /employees/nameAndBirthDate/{nameToMatch}{intervalStart}{intervalEnd}
        [HttpGet("nameAndBirthDate/{nameToMatch}/{intervalStart}/{intervalEnd}")]
        public async Task<IEnumerable<EmployeeDto>> GetEmployeesByNameAndBirthdateIntervalAsync(string nameToMatch, DateTime intervalStart, DateTime intervalEnd)
        {
            var employees = (await repository.GetAllEmployeesAsync())
                        .Select(employee => employee.AsDto());

            if(!string.IsNullOrWhiteSpace(nameToMatch))
            {
                employees = employees.Where(employee => employee.FirstName is not null && employee.FirstName.Contains(nameToMatch, StringComparison.OrdinalIgnoreCase));
            }

            if(intervalStart != intervalEnd && intervalEnd > intervalStart)
            {
                 employees = employees.Where(employee => intervalStart <= employee.BirthDate && employee.BirthDate <= intervalEnd);
            }

            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved {employees.Count()} items");

            return employees;
        }

        // GET /employees/boss/{id}
        [HttpGet("boss/{id}")]
        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesByBossIdAsync(Guid bossId)
        {
            var employees = (await repository.GetAllEmployeesAsync())
                        .Select(employee => employee.AsDto());

            employees = employees.Where(employee => employee.Boss is not null && employee.Boss.Id == bossId);

            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved {employees.Count()} items");

            return employees;
        }

        // GET /employees/countAndAverage/{role}
        [HttpGet("countAndAverage/{role}")]
        public async Task<double[]> GetEmployeeCountAndAverageSalaryForRoleAsync(Position role)
        {
            var response = (await repository.GetEmployeeCountAndAverageSalaryForRoleAsync(role));

            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved {response.ElementAt(0)} items, salary : {response.ElementAt(1)}");

            return response;
        }

        // POST /employees
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> AddNewEmployeeAsync(CreateEmployeeDto employeeDto)
        {
            Employee employee;

            if(employeeDto.Role == Position.Boss.ToString()){
                employee = new(){
                    Id = Guid.NewGuid(),
                    FirstName = employeeDto.FirstName,
                    LastName = employeeDto.LastName,
                    BirthDate = employeeDto.BirthDate,
                    EmploymentDate = DateTime.Now,
                    Boss = defaultCeo,
                    HomeAddress = employeeDto.HomeAddress,
                    CurrentSalary = employeeDto.CurrentSalary,
                    Role = employeeDto.Role,
                };
            } else if(employeeDto.Role == Position.Ceo.ToString()){
                employee = new(){
                    Id = Guid.NewGuid(),
                    FirstName = employeeDto.FirstName,
                    LastName = employeeDto.LastName,
                    BirthDate = employeeDto.BirthDate,
                    EmploymentDate = DateTime.Now,
                    HomeAddress = employeeDto.HomeAddress,
                    CurrentSalary = employeeDto.CurrentSalary,
                    Role = employeeDto.Role,
                    Boss = null
                };

                defaultCeo = new(){
                    Id = Guid.NewGuid(),
                    FirstName = employeeDto.FirstName,
                    LastName = employeeDto.LastName,
                    BirthDate = employeeDto.BirthDate,
                    EmploymentDate = DateTime.Now,
                    HomeAddress = employeeDto.HomeAddress,
                    CurrentSalary = employeeDto.CurrentSalary,
                    Role = employeeDto.Role,
                    Boss = null
                };
            } else {
                employee = new(){
                    Id = Guid.NewGuid(),
                    FirstName = employeeDto.FirstName,
                    LastName = employeeDto.LastName,
                    BirthDate = employeeDto.BirthDate,
                    EmploymentDate = DateTime.Now,
                    Boss = defaultBoss,
                    HomeAddress = employeeDto.HomeAddress,
                    CurrentSalary = employeeDto.CurrentSalary,
                    Role = employeeDto.Role
                };
            }

            await repository.AddNewEmployeeAsync(employee);

            return CreatedAtAction(nameof(GetEmployeeByIdAsync), new { id = employee.Id }, employee.AsDto());
        }

        // PUT /employees/update/{id}
        [HttpPut("update/{id}")]
        public async Task<ActionResult> UpdateEmployeeAsync(Guid id, UpdateEmployeeDto employeeDto)
        {
            var existingEmployee = await repository.GetEmployeeByIdAsync(id);

            if(existingEmployee is null)
            {
                return NotFound();
            }

            existingEmployee.FirstName = employeeDto.FirstName;
            existingEmployee.LastName = employeeDto.LastName;
            existingEmployee.BirthDate = employeeDto.BirthDate;
            existingEmployee.EmploymentDate = employeeDto.EmploymentDate;
            existingEmployee.HomeAddress = employeeDto.HomeAddress;
            existingEmployee.CurrentSalary = employeeDto.CurrentSalary;
            existingEmployee.Role = employeeDto.Role;

            await repository.UpdateEmployeeAsync(existingEmployee);

            return NoContent();
        }

        // PUT /employees/update/salary/{id}
        [HttpPut("update/salary/{id}")]
        public async Task<ActionResult> UpdateEmployeeSalaryAsync(Guid id, UpdateEmployeeSalaryDto employeeDto)
        {
            var existingEmployee = await repository.GetEmployeeByIdAsync(id);

            if(existingEmployee is null)
            {
                return NotFound();
            }

            existingEmployee.CurrentSalary = employeeDto.CurrentSalary;

            await repository.UpdateEmployeeSalaryAsync(existingEmployee);

            return NoContent();
        }

        // DELETE /employees/delete/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEmployeeAsync(Guid id)
        {
            var existingEmployee = await repository.GetEmployeeByIdAsync(id);

            if(existingEmployee is null)
            {
                return NotFound();
            }

            await repository.DeleteEmployeeAsync(id);

            return NoContent();
        }

    }
}