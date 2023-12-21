
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace VbApi.Controllers;

public class Employee 
{
    public string Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public double HourlySalary { get; set; }
}



[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    public EmployeeController()
    {
    }

    [HttpPost]
    public IActionResult Post([FromBody] Employee value)
    {
        EmployeeValidator employeeValidator = new EmployeeValidator();
        ValidationResult validationResult = employeeValidator.Validate(value);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        return Ok(value);
    }
}

public class EmployeeValidator : AbstractValidator<Employee>
{

    private readonly Regex phoneRegex = new Regex(@"^(\+\s?)?((?<!\+.*)\(\+?\d+([\s\-\.]?\d+)?\)|\d+)([\s\-\.]?(\(\d+([\s\-\.]?\d+)?\)|\d+))*(\s?(x|ext\.?)\s?\d+)?$",
    RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

    private bool ValidateHourlySalary(Employee employee, double hourlySalary)
    {
        var dateBeforeThirtyYears = DateTime.Today.AddYears(-30);
        var isOlderThanThirdyYears = employee.DateOfBirth <= dateBeforeThirtyYears;

        return isOlderThanThirdyYears ? hourlySalary >= 300: hourlySalary >= 50;
    }
    public EmployeeValidator()
    {
        RuleFor(employee => employee.Name).NotEmpty().Length(10,250).WithMessage("Invalid Name");

        RuleFor(employee => employee.DateOfBirth).NotEmpty();

        RuleFor(employee => employee.Email).EmailAddress().WithMessage("Email address is not valid.");
       
        RuleFor(employee => employee.Phone).
            Matches(phoneRegex).WithMessage("Phone is not valid.");

        //[Range(minimum: 50, maximum: 400, ErrorMessage = "Hourly salary does not fall within allowed range.")]
        RuleFor(employee => employee.HourlySalary).InclusiveBetween(50, 400).WithMessage("Hourly salary does not fall within allowed range.");

        RuleFor(employee => employee.HourlySalary)
            .Must((employee, hourlySalary) => ValidateHourlySalary(employee, hourlySalary))
            .WithMessage("Minimum hourly salary is not valid.");


    }
}