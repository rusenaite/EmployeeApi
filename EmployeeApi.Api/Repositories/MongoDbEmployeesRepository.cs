using System.Linq;
using EmployeeApi.Api.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace EmployeeApi.Api.Repositories
{
    public class MongoDbEmployeesRepository : IEmployeesRepository
    {
         private const string databaseName = "employeesApi";
        private const string collectionName = "employees";
        private readonly FilterDefinitionBuilder<Employee> filterBuilder = Builders<Employee>.Filter;
        private readonly IMongoCollection<Employee> employeesCollection;

        public MongoDbEmployeesRepository(IMongoClient mongoClient)
        {
            // reference to the database and to the collection
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            employeesCollection = database.GetCollection<Employee>(collectionName);
        }

        public async Task AddNewEmployeeAsync(Employee employee)
        {
            await employeesCollection.InsertOneAsync(employee);
        }

        public async Task DeleteEmployeeAsync(Guid id)
        {
            var filter = filterBuilder.Eq(employee => employee.Id, id);
            await employeesCollection.DeleteOneAsync(filter);
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await employeesCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesByBossIdAsync(Guid bossId)
        {
            var filter = filterBuilder.Eq(employee => employee.Boss!.Id, bossId);
            return await employeesCollection.Find(filter).ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(Guid id)
        {
            var filter = filterBuilder.Eq(employee => employee.Id, id);
            return await employeesCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<double[]> GetEmployeeCountAndAverageSalaryForRoleAsync(Position role)
        {
            var filter = filterBuilder.Eq(employee => employee.Role, role.ToString());

            Func<double[]> function = new Func<double[]>(() =>
            {
                return CalculateAndCount(filter);
            });

            return await Task.Run<double[]>(function);
        }

        private double[] CalculateAndCount(FilterDefinition<Employee> filter)
        {
            var salary = employeesCollection.Find(filter).ToList().Select(employee => employee.CurrentSalary).Sum();
            double count = employeesCollection.CountDocuments(filter);
            var averageSalary = salary / count;

            return new double[] { count, averageSalary};
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            var filter = filterBuilder.Eq(existingEmployee => existingEmployee.Id, employee.Id);
            await employeesCollection.ReplaceOneAsync(filter, employee);
        }

        public async Task UpdateEmployeeSalaryAsync(Employee employee)
        {
            var filter = filterBuilder.Eq(existingEmployee => existingEmployee.Id, employee.Id);
            await employeesCollection.ReplaceOneAsync(filter, employee);
        }
    }
}