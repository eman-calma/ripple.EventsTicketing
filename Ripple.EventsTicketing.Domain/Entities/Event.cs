using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple.EventsTicketing.Domain.Entities
{
    public class Event
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Venue { get; set; } = string.Empty;

        public DateTimeOffset StartsAt { get; set; }

        public int TotalCapacity { get; set; }

        public DateTimeOffset CreatedAtUtc { get; set; }

        public DateTimeOffset? UpdatedAtUtc { get; set; }

        public byte[] RowVersion { get; set; } = [];

        public ICollection<PricingTier> PricingTiers { get; set; }  = new List<PricingTier>();
    }
}
