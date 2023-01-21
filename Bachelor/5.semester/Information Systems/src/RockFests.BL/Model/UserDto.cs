using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RockFests.BL.Resources;
using RockFests.DAL.Enums;

namespace RockFests.BL.Model
{
    public class UserDto : BaseDto, IValidatableObject
    {
        public string Login { get; set; }
        public AccessRole AccessRole { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

        public bool EnableValidation { get; set; } = true;

        public UserDto(){}

        public UserDto(UserDto user)
        {
            Id = user.Id;
            Login = user.Login;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Phone = user.Phone;
            AccessRole = user.AccessRole;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (!EnableValidation)
                return results;

            if (string.IsNullOrWhiteSpace(FirstName))
                results.Add(new ValidationResult(Errors.Required, new[] { nameof(FirstName) }));
            if (string.IsNullOrWhiteSpace(LastName))
                results.Add(new ValidationResult(Errors.Required, new[] { nameof(LastName) }));
            if (string.IsNullOrWhiteSpace(Login))
                results.Add(new ValidationResult(Errors.Required, new[] { nameof(Login) }));
            if (string.IsNullOrWhiteSpace(Email))
                results.Add(new ValidationResult(Errors.Required, new[] { nameof(Email) }));
            if (!string.IsNullOrWhiteSpace(Email) && !Email.Contains("@"))
                results.Add(new ValidationResult(Errors.InvalidEmailFormat, new[] { nameof(Email) }));
            return results;
        }
    }
}
