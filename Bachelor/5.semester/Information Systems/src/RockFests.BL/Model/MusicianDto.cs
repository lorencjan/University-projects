using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RockFests.BL.Resources;


namespace RockFests.BL.Model
{
    public class MusicianDto : BaseDto, IValidatableObject
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Born { get; set; }
        public byte[] Photo { get; set; }
        public string Country { get; set; }
        public string Role { get; set; }
        public string Biography { get; set; }
        public List<BandLightDto> Bands { get; set; }
        public List<RatingDto> Ratings { get; set; }
        public List<PerformanceDto> Performances { get; set; }

        public MusicianDto Copy() => new MusicianDto
        {
            Id = Id,
            FirstName = FirstName,
            LastName = LastName,
            Born = Born,
            Photo = Photo,
            Country = Country,
            Role = Role,
            Biography = Biography
        };
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(FirstName))
                results.Add(new ValidationResult(Errors.Required, new[] { nameof(FirstName) }));
            if (string.IsNullOrWhiteSpace(LastName))
                results.Add(new ValidationResult(Errors.Required, new[] { nameof(LastName) }));
            return results;
        }
    }
}
