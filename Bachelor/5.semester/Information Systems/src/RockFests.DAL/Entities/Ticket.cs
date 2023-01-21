using System;

namespace RockFests.DAL.Entities
{
    public class Ticket : BaseEntity
    {
        public bool IsPaid { get; set; }
        public DateTime PaymentDue { get; set; }
        public long VariableSymbol { get; set; }
        /// <summary>For how many people the ticket is</summary>
        public int Count { get; set; }

        public int FestivalId { get; set; }
        public Festival Festival { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
