using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RockFests.BL.Resources;

namespace RockFests.BL.Model
{
    public class TicketDto : BaseDto, IValidatableObject
    {
        public bool IsPaid { get; set; }
        public DateTime PaymentDue { get; set; }
        public long VariableSymbol { get; set; }
        public int Count { get; set; }
        public FestivalLightDto Festival { get; set; } = new FestivalLightDto();
        public UserDto User { get; set; } = new UserDto();

        public TicketDto CopyLight() => new TicketDto
        {
            Id = Id,
            IsPaid = IsPaid,
            PaymentDue = PaymentDue,
            VariableSymbol = VariableSymbol,
            Count = Count,
            Festival = new FestivalLightDto {Id = Festival.Id},
            User = new UserDto {Id = User.Id, EnableValidation = false}
        };

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (Count <= 0)
                results.Add(new ValidationResult(Errors.Required, new[] { nameof(Count) }));
            if (Festival == null || Festival.Id == 0)
                results.Add(new ValidationResult(Errors.Required, new[] { nameof(Festival) }));
            if (User == null || User.Id == 0)
                results.Add(new ValidationResult(Errors.Required, new[] { nameof(User) }));
            if (PaymentDue == new DateTime())
                results.Add(new ValidationResult(Errors.Required, new[] { nameof(PaymentDue) }));

            return results;
        }
    }
}
