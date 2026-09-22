using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.EventsTicketing.Domain.Entities
{
    public class TicketOrderItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid TicketOrderId { get; set; }

        public Guid PricingTierId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public TicketOrder TicketOrder { get; set; } = null!;

        public PricingTier PricingTier { get; set; } = null!;
    }
}
