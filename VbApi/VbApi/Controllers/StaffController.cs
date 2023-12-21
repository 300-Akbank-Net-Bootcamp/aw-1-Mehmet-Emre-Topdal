using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using System.Text.RegularExpressions;

namespace VbApi.Controllers;

public class Staff
{
    
   
    public string? Name { get; set; }


    public string? Email { get; set; }

 
    public string? Phone { get; set; }

    
    public decimal? HourlySalary { get; set; }
}

[Route("api/[controller]")]
[ApiController]
public class StaffController : ControllerBase
{
    public StaffController()
    {
    }

    [HttpPost]
    public IActionResult Post([FromBody] Staff value)
    {
        StaffValidator staffValidator = new StaffValidator();
        ValidationResult validationResult = staffValidator.Validate(value);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        return Ok(value);
    }
}

public class StaffValidator : AbstractValidator<Staff>
{
    private readonly Regex phoneRegex = new Regex(@"^(\+\s?)?((?<!\+.*)\(\+?\d+([\s\-\.]?\d+)?\)|\d+)([\s\-\.]?(\(\d+([\s\-\.]?\d+)?\)|\d+))*(\s?(x|ext\.?)\s?\d+)?$", 
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
    public StaffValidator()
    {
        RuleFor(staff => staff.Name)
            .NotEmpty().Length(10, 250);

        RuleFor(staff => staff.Email)
            .EmailAddress().WithMessage("Email address is not valid.");

   
        RuleFor(staff => staff.Phone).
            Matches(phoneRegex).WithMessage("Phone is not valid.");

        RuleFor(staff => staff.HourlySalary)
            .InclusiveBetween(30, 400).WithMessage("Hourly salary does not fall within allowed range.");
    }
}   