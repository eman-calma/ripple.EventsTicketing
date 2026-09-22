using Ripple.EventsTicketing.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.EventsTicketing.Domain.Entities
{
    public class TicketOrder
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid EventId { get; set; }

        public string UserObjectId { get; set; } = string.Empty;

        public string IdempotencyKey { get; set; } = string.Empty;

        public DateTimeOffset PurchasedAtUtc { get; set; }

        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; }

        public Event Event { get; set; } = null!;

        public ICollection<TicketOrderItem> Items { get; set; } = new List<TicketOrderItem>();
    }
}
