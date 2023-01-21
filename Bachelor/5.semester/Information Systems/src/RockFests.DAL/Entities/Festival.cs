using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RockFests.DAL.Entities
{
    public class Festival : BaseEntity
    {
        public string Name { get; set; }
        public string Location { get; set; }
        [Column("Start date")]
        public DateTime StartDate { get; set; }
        [Column("End date")]
        public DateTime EndDate { get; set; }
        [Column("Maximum tickets")]
        public int MaxTickets { get; set; }
        [Column("Maximum tickets for user")]
        public int MaxTicketsForUser { get; set; }
        [Column("Reservation Days")]
        public int ReservationDays { get; set; }
        public int Price { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }

        public List<Stage> Stages { get; set; }
        public List<Ticket> Tickets { get; set; }
    }
}
