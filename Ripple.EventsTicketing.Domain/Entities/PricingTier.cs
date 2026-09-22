using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.EventsTicketing.Domain.Entities
{
    public class PricingTier
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid EventId { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Capacity { get; set; }

        public int RemainingQuantity { get; set; }

        public byte[] RowVersion { get; set; } = [];

        public Event Event { get; set; } = null!;
    }
}
