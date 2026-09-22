using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ripple.EventsTicketing.Application.Reports.Queries;

namespace Ripple.EventsTicketing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Gets a ticket sales summary for every event.</summary>
        [HttpGet("sales")]
        [ProducesResponseType(typeof(IReadOnlyCollection<EventSalesSummaryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyCollection<EventSalesSummaryDto>>> GetAllSalesSummaries(CancellationToken cancellationToken)
        {
            var summaries = await _mediator.Send(new GetAllEventSalesSummariesQuery(), cancellationToken);

            return Ok(summaries);
        }

        /// <summary>Gets a ticket sales summary for a single event.</summary>
        [HttpGet("sales/{eventId:guid}")]
        [ProducesResponseType(typeof(EventSalesSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EventSalesSummaryDto>> GetSalesSummary(Guid eventId, CancellationToken cancellationToken)
        {
            var summary = await _mediator.Send(new GetEventSalesSummaryQuery(eventId), cancellationToken);

            return Ok(summary);
        }
    }
}
