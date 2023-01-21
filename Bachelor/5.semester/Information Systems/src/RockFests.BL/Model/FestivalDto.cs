using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RockFests.BL.Resources;
using RockFests.DAL.Types;

namespace RockFests.BL.Model
{
    public class FestivalDto : BaseDto, IValidatableObject
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTimeInterval Date { get; set; } = new DateTimeInterval();
        public int? MaxTickets { get; set; }
        public int? MaxTicketsForUser { get; set; }
        public int? ReservationDays { get; set; }
        public int? Price { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }

        public List<StageDto> Stages { get; set; } = new List<StageDto>();
        public List<TicketDto> Tickets { get; set; } = new List<TicketDto>();

        public FestivalDto Copy() => new FestivalDto
        {
            Id = Id,
            Name = Name,
            Location = Location,
            Date = new DateTimeInterval(Date.Start, Date.End),
            MaxTickets = MaxTickets,
            MaxTicketsForUser = MaxTicketsForUser,
            ReservationDays = ReservationDays,
            Price = Price,
            Currency = Currency,
            Description = Description
        };

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(Name))
                results.Add(new ValidationResult(Errors.Required, new[] { nameof(Name) }));
            if (string.IsNullOrWhiteSpace(Location))
                results.Add(new ValidationResult(Errors.Required, new[] { nameof(Location) }));
            if (string.IsNullOrWhiteSpace(Currency))
                results.Add(new ValidationResult(Errors.Required, new[] { nameof(Currency) }));

            if (!MaxTickets.HasValue || MaxTickets.Value <= 0)
                results.Add(new ValidationResult(Errors.Required, new[] { nameof(MaxTickets) }));
            if (!ReservationDays.HasValue || ReservationDays.Value <= 0)
                results.Add(new ValidationResult(Errors.Required, new[] { nameof(ReservationDays) }));
            if (!Price.HasValue || Price.Value <= 0)
                results.Add(new ValidationResult(Errors.Required, new[] { nameof(Price) }));

            if(Date.Start >= Date.End)
                results.Add(new ValidationResult(Errors.InvalidDateInterval, new[] { nameof(Date) }));

            return results;
        }
    }
}
