using MongoDB.Bson.Serialization.Attributes;

namespace EmployeeApi.Api.Entities
{
    [BsonIgnoreExtraElements]
    public class Boss : Employee
    {
        public Boss() {}

        // default CEO constructor
        public Boss(string firstName, string lastName, DateTime employmentDate) {
            Id = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
            BirthDate = new DateTime(1988, 1, 9).Date;
            EmploymentDate = employmentDate;
            HomeAddress = "88 Greenland St., Liverpool";
            CurrentSalary = 2226.30;
            Boss = null;
        }

        // default Boss constructor
        public Boss(string firstName, string lastName, DateTime employmentDate, Boss ceo) {
            Id = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
            BirthDate = new DateTime(1988, 1, 9).Date;
            EmploymentDate = employmentDate;
            HomeAddress = "88 Greenland St., Liverpool";
            CurrentSalary = 2226.30;
            Boss = ceo;
        }

        public override string Role 
        { 
            get { return Position.Boss.ToString(); }
        }

    }
}