using System.Globalization;
using EmployeeApi.Api.Entities;

namespace EmployeeApi.Api.Repositories
{
    public class InMemEmployeeRepository : IEmployeesRepository
    {
        private static readonly Boss ceo = new(){
            Id = Guid.NewGuid(), 
            FirstName = "Tom", 
            LastName = "Calmin", 
            BirthDate = new DateTime(1988, 1, 9).Date,
            EmploymentDate = new DateTime(2016, 8, 1),
            HomeAddress = "88 Greenland St., Liverpool",
            CurrentSalary = 2226.30,
            Boss = null
        };

        private static readonly Boss boss = new(){
            Id = Guid.NewGuid(), 
            FirstName = "Agness", 
            LastName = "Litt", 
            BirthDate = new DateTime(1988, 1, 9).Date,
            EmploymentDate = new DateTime(2019, 8, 9).Date,
            Boss = new Boss("Jess", "Calmin", new DateTime(2001, 1, 9)),
            HomeAddress = "68 Yorker St., Liverpool",
            CurrentSalary = 1022.30,
            Role = Position.Boss.ToString()
        };

        private readonly List<Employee> employees = new()
        {
            new Employee { 
                Id = Guid.NewGuid(), 
                FirstName = "John", 
                LastName = "Lee", 
                BirthDate = new DateTime(1992, 10, 9).Date,
                EmploymentDate = new DateTime(2021, 10, 22).Date,
                Boss = boss,
                HomeAddress = "88 Journal Square, Jersey City",
                CurrentSalary = 1026.30,
                Role = Position.Product_Manager.ToString()
            },
            new Employee { 
                Id = Guid.NewGuid(), 
                FirstName = "Casey", 
                LastName = "Kinderly", 
                BirthDate = new DateTime(1990, 4, 16).Date,
                EmploymentDate = new DateTime(2020, 11, 2).Date,
                Boss = boss,
                HomeAddress = "65 Garnel St., Liverpool",
                CurrentSalary = 1005.60,
                Role = Position.Software_Developer.ToString()
            },
            new Employee { 
                Id = Guid.NewGuid(), 
                FirstName = "Harry", 
                LastName = "Lonecy", 
                BirthDate = new DateTime(1989, 9, 1).Date,
                EmploymentDate = new DateTime(2019, 11, 2).Date,
                Boss = boss,
                HomeAddress = "96 Lightbull St., London",
                CurrentSalary = 1015.99,
                Role = Position.Software_Developer.ToString()
            }
        };

        public async Task<Employee?> GetEmployeeByIdAsync(Guid id)
        {
            var employee = employees.Where(employee => employee.Id == id).SingleOrDefault();
            return await Task.FromResult(employee);
        }

         public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await Task.FromResult(employees);
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesByBossIdAsync(Guid bossId)
        {
            var foundEmployees = employees.Where(employee => employee.Boss is not null && employee.Boss.Id == bossId);
            return await Task.FromResult(foundEmployees);
        }

        public async Task<double[]> GetEmployeeCountAndAverageSalaryForRoleAsync(Position role)
        {
            return await Task.FromResult(CalculateAndCount(role));
        }

        private double[] CalculateAndCount(Position role)
        {
            var salary = employees.Where(employee => employee.Role == role.ToString()).Sum(employee => employee.CurrentSalary);
            double count = employees.Count();
            var averageSalary = salary / count;

            return new double[] { count, averageSalary};
        }

        public async Task AddNewEmployeeAsync(Employee employee)
        {
            employees.Add(employee);
            await Task.CompletedTask;
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            var index = employees.FindIndex(existingEmployee => existingEmployee.Id == employee.Id);
            employees[index] = employee;
            await Task.CompletedTask;
        }

        public async Task UpdateEmployeeSalaryAsync(Employee employee)
        {
            var index = employees.FindIndex(existingEmployee => existingEmployee.Id == employee.Id);
            employees[index] = employee;
            await Task.CompletedTask;
        }

         public async Task DeleteEmployeeAsync(Guid id)
        {
            var index = employees.FindIndex(existingEmployee => existingEmployee.Id == id);
            employees.RemoveAt(index);
            await Task.CompletedTask;
        }    
    }
}