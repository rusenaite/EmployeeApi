using MongoDB.Bson.Serialization.Attributes;

namespace EmployeeApi.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class Employee
    {
        public Guid Id { get; init; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime EmploymentDate { get; set; }
        public string? HomeAddress { get; set; }
        public double CurrentSalary { get; set; }
        public virtual string? Role { get; set; }
        public Boss? Boss { get; set; }
    }
}