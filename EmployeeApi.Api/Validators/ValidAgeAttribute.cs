using System.ComponentModel.DataAnnotations;

namespace EmployeeApi.Api.Validators
{
    public class ValidAgeAttribute : ValidationAttribute
    {
        public override string FormatErrorMessage(string name)
        {
            return "Employee has has to be in the interval [ 18 ; 70 ].";
        }

        protected override ValidationResult IsValid(object? objValue, ValidationContext validationContext)
        {
            var dateValue = objValue as DateTime? ?? new DateTime();

            if (dateValue.Date.AddYears(18) >= DateTime.Now.Date || dateValue.Date.AddYears(70) <= DateTime.Now.Date)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            return ValidationResult.Success!;
        }
    }
}