using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeApi.Api.Controllers;
using EmployeeApi.Api.Dtos;
using EmployeeApi.Api.Entities;
using EmployeeApi.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Moq;
using Xunit;

namespace EmployeeApi.UnitTests;

public class EmployeesControllerTests
{
    private readonly Mock<IEmployeesRepository> repositoryStub = new();
    private readonly Mock<ILogger<EmployeesController>> loggerStub = new();
    private readonly Random rand = new();

    [Fact]
    public async Task GetEmployeeByIdAsync_WithUnexistingEmployee_ReturnsNotFound()
    {
        // Arrange
        repositoryStub.Setup(repo => repo.GetEmployeeByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(null as Employee);

        var controller = new EmployeesController(repositoryStub.Object, loggerStub.Object);

        // Act
        var result = await controller.GetEmployeeByIdAsync(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetEmployeeByIdAsync_WithExistingEmployee_ReturnsExpectedEmployee()
    {
        // Arrange
        var expectedEmployee = CreateRandomEmployee();
        repositoryStub.Setup(repo => repo.GetEmployeeByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expectedEmployee);

        var controller = new EmployeesController(repositoryStub.Object, loggerStub.Object);

        // Act
        var result = await controller.GetEmployeeByIdAsync(Guid.NewGuid());

        // Assert
        result.Value.Should().BeEquivalentTo(expectedEmployee);
    }

    [Fact]
    public async Task GetAllEmployeesAsync_WithExistingEmployees_ReturnsAllEmployees()
    {
        // Arrange
        var expectedEmployees = new[] { CreateRandomEmployee(), CreateRandomEmployee(), CreateRandomEmployee() };

        repositoryStub.Setup(repo => repo.GetAllEmployeesAsync())
            .ReturnsAsync(expectedEmployees);

        var controller = new EmployeesController(repositoryStub.Object, loggerStub.Object);

        // Act
        var actualEmployees = await controller.GetAllEmployeesAsync();

        // Assert
        actualEmployees.Should().BeEquivalentTo(expectedEmployees);
    }

    [Fact]
    public async Task GetEmployeesByNameAndBirthdateIntervalAsync_WithMatchingEmployees_ReturnsMatchingEmployees()
    {
        // Arrange
        var allEmployees = new[]
        {
            new Employee(){ FirstName = "Garry", BirthDate = new DateTime(1999-02-02).Date },
            new Employee(){ FirstName = "Garry", BirthDate = new DateTime(1992-02-02).Date },
            new Employee(){ FirstName = "Garry", BirthDate = new DateTime(1960-02-02).Date },
            new Employee(){ FirstName = "Seff", BirthDate = new DateTime(1999-02-02).Date }
        };

        var nameToMatch = "Garry";

        repositoryStub.Setup(repo => repo.GetAllEmployeesAsync())
            .ReturnsAsync(allEmployees);

        var controller = new EmployeesController(repositoryStub.Object, loggerStub.Object);

        DateTime intervalStart = new DateTime(1990-02-02).Date;
        DateTime intervalEnd = new DateTime(1999-03-02).Date;

        // Act
        IEnumerable<EmployeeDto> foundItems = await controller.GetEmployeesByNameAndBirthdateIntervalAsync(nameToMatch, intervalStart, intervalEnd);

        // Assert
        foundItems.Should().OnlyContain(
            employee => (employee.FirstName == allEmployees[0].FirstName && intervalStart <= employee.BirthDate && employee.BirthDate <= intervalEnd) || 
                        (employee.FirstName == allEmployees[1].FirstName && intervalStart <= employee.BirthDate && employee.BirthDate <= intervalEnd)
        );
    }

    [Fact]
    public async Task GetAllEmployeesByBossIdAsync_WithMatchingEmployees_ReturnsMatchingEmployees()
    {
        // Arrange
        var allEmployees = new[]{ CreateRandomEmployee(), CreateRandomEmployee(), CreateRandomEmployee() };

        Boss boss = new Boss();

        if(allEmployees[0].Boss is not null){
            boss = allEmployees[0].Boss!;
        }

        repositoryStub.Setup(repo => repo.GetAllEmployeesAsync())
            .ReturnsAsync(allEmployees);

        var controller = new EmployeesController(repositoryStub.Object, loggerStub.Object);

        // Act
        IEnumerable<EmployeeDto> foundItems = await controller.GetAllEmployeesByBossIdAsync(boss!.Id);

        // Assert
        foundItems.Should().OnlyContain(employee => employee.Boss!.Id == boss.Id);
    }

    /*
    [Fact]
    public async Task GetEmployeeCountAndAverageSalaryByRoleAsync_WithExistingEmployees_ReturnsCountAndAverageSalary()
    {
        // Arrange
        var expectedEmployees = new[] { CreateRandomEmployee(), CreateRandomEmployee(), CreateRandomEmployee() };

        repositoryStub.Setup(repo => repo.GetAllEmployeesAsync())
            .ReturnsAsync(expectedEmployees);

        var controller = new EmployeesController(repositoryStub.Object, loggerStub.Object);

        var position = (Position)rand.Next(4);
        var role = position.ToString();
        var filteredEmployees = expectedEmployees.Where(employee => employee.Role == role.ToString());

        var salary = filteredEmployees.Sum(employee => employee.CurrentSalary);

        int averageSalary = (int)(salary / expectedEmployees!.Length);

        IEnumerable<int> response = new List<int>() 
        { 
            filteredEmployees.Count(),
            averageSalary 
        };

        // Act
        var actualEmployees = await controller.GetEmployeeCountAndAverageSalaryForRoleAsync(position);

        // Assert
        actualEmployees.Should().BeEquivalentTo(expectedEmployees);
    }
    */

    [Fact]
    public async Task AddNewEmployeeAsync_WithEmployeeToCreate_ReturnsCreatedEmployee()
    {
        // Arrange
        var employeeToCreate = new CreateEmployeeDto()
        {
            Id = Guid.NewGuid(),
            FirstName = Guid.NewGuid().ToString(),
            LastName = Guid.NewGuid().ToString(),
            BirthDate = DateTime.Now.AddYears(-(rand.Next(18, 70))),
            EmploymentDate = DateTime.Now,
            HomeAddress = "1 Random St., Randomness",
            CurrentSalary = rand.Next(1000),
            Role = ((Position)rand.Next(4)).ToString()
        };

        var controller = new EmployeesController(repositoryStub.Object, loggerStub.Object);

        // Act
        var result = await controller.AddNewEmployeeAsync(employeeToCreate);

        // Assert
        var createdEmployee = ((CreatedAtActionResult)result.Result!).Value as EmployeeDto;

        employeeToCreate.Should().BeEquivalentTo(
            result,
            options => options.ComparingByMembers<EmployeeDto>().ExcludingMissingMembers()
        );

        createdEmployee!.Id.Should().NotBeEmpty();
        createdEmployee.EmploymentDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task UpdateEmployeeAsync_WithExistingEmployee_ReturnsNoContent()
    {
        // Arrange
        Employee existingEmployee = CreateRandomEmployee();
        repositoryStub.Setup(repo => repo.GetEmployeeByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingEmployee);

        var employeeId = existingEmployee.Id;

        var employeeToUpdate = new UpdateEmployeeDto()
        {
            FirstName = Guid.NewGuid().ToString(),
            LastName = Guid.NewGuid().ToString(),
            BirthDate = DateTime.UtcNow.AddYears(-(rand.Next(18, 70))),
            EmploymentDate = DateTime.UtcNow.Date,
            HomeAddress = "1 Random St., Randomness",
            CurrentSalary = rand.Next(2000),
            Role = ((Position)rand.Next(4)).ToString()
        };

        var controller = new EmployeesController(repositoryStub.Object, loggerStub.Object);

        // Act
        var result = await controller.UpdateEmployeeAsync(employeeId, employeeToUpdate);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateEmployeeSalaryAsync_WithExistingEmployee_ReturnsNoContent()
    {
        // Arrange
        Employee existingEmployee = CreateRandomEmployee();
        repositoryStub.Setup(repo => repo.GetEmployeeByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingEmployee);

        var employeeId = existingEmployee.Id;

        var employeeToUpdate = new UpdateEmployeeSalaryDto()
        {
            CurrentSalary = rand.Next(2000)
        };

        var controller = new EmployeesController(repositoryStub.Object, loggerStub.Object);

        // Act
        var result = await controller.UpdateEmployeeSalaryAsync(employeeId, employeeToUpdate);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteEmployeeAsync_WithExistingEmployee_ReturnsNoContent()
    {
        // Arrange
        Employee existingEmployee = CreateRandomEmployee();
        repositoryStub.Setup(repo => repo.GetEmployeeByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingEmployee);

        var controller = new EmployeesController(repositoryStub.Object, loggerStub.Object);

        // Act
        var result = await controller.DeleteEmployeeAsync(existingEmployee.Id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    private Employee CreateRandomEmployee()
    {
        return new()
        {
            Id = Guid.NewGuid(),
            FirstName = Guid.NewGuid().ToString(),
            LastName = Guid.NewGuid().ToString(),
            BirthDate = DateTime.UtcNow.AddYears(-(rand.Next(18, 70))),
            EmploymentDate = DateTime.UtcNow.Date,
            HomeAddress = "1 Random St., Randomness",
            CurrentSalary = rand.Next(1000),
            Role = ((Position)rand.Next(4)).ToString(),
            Boss = new Boss("Harry", "Lake", new DateTime(2001, 1, 9).Date, new Boss())
        };
    }
}