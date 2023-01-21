using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RockFests.BL.Resources;

namespace RockFests.Model
{
    public class PasswordModel : IValidatableObject
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool DontValidate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (DontValidate)
                return results;

            if (string.IsNullOrWhiteSpace(Password))
                results.Add(new ValidationResult(Errors.Required, new[] { nameof(Password) }));
            if (string.IsNullOrWhiteSpace(ConfirmPassword))
                results.Add(new ValidationResult(Errors.Required, new[] { nameof(ConfirmPassword) }));
            if (!string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(ConfirmPassword) && Password != ConfirmPassword)
                results.Add(new ValidationResult(Errors.PasswordsDontMatch, new[] { nameof(Password) }));
            return results;
        }
    }
}
