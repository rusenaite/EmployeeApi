using EmployeeApi.Api.Entities;

namespace EmployeeApi.Api.Repositories
{
    public interface IEmployeesRepository
    {
        Task<Employee?> GetEmployeeByIdAsync(Guid id);
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<IEnumerable<Employee>> GetAllEmployeesByBossIdAsync(Guid bossId);
        Task<double[]> GetEmployeeCountAndAverageSalaryForRoleAsync(Position role);
        Task AddNewEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(Employee employee);
        Task UpdateEmployeeSalaryAsync(Employee employee);
        Task DeleteEmployeeAsync(Guid id);
    }

}