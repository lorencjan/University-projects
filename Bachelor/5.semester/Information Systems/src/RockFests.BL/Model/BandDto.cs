using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RockFests.BL.Resources;

namespace RockFests.BL.Model
{
    public class BandDto : BaseDto, IValidatableObject
    {
        public string Name { get; set; }
        public string Genre { get; set; }
        public byte[] Photo { get; set; }
        public string Country { get; set; }
        public int FormationYear { get; set; }
        public string Description { get; set; }

        public List<MusicianLightDto> Members { get; set; }
        public List<RatingDto> Ratings { get; set; }
        public List<PerformanceDto> Performances { get; set; }
        public BandDto Copy() => new BandDto
        {
            Id = Id,
            Name = Name,
            Genre = Genre,
            Photo = Photo,
            Country = Country,
            FormationYear = FormationYear,
            Description = Description,
            Members = Members
        };

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(Name))
                results.Add(new ValidationResult(Errors.Required, new[] { nameof(Name) }));
            if (string.IsNullOrWhiteSpace(Genre))
                results.Add(new ValidationResult(Errors.Required, new[] { nameof(Genre) }));
            if ( (Members==null) || (Members.Count==0))
                results.Add(new ValidationResult(Errors.MembersEmpty, new[] { nameof(Members) }));
            if (FormationYear == 0)
                results.Add(new ValidationResult(Errors.Required, new[] { nameof(FormationYear) }));
            return results;
        }
    }
}
