using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ripple.EventsTicketing.Application.Tickets.Commands;
using Ripple.EventsTicketing.Application.Tickets.Queries;

namespace Ripple.EventsTicketing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TicketsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Purchases tickets for an event.</summary>
        [HttpPost("purchase")]
        [ProducesResponseType(typeof(TicketOrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<TicketOrderDto>> Purchase(PurchaseTicketsCommand command, CancellationToken cancellationToken)
        {
            var order = await _mediator.Send(command, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, order);
        }

        /// <summary>Gets ticket availability per pricing tier for an event.</summary>
        [HttpGet("availability/{eventId:guid}")]
        [ProducesResponseType(typeof(TicketAvailabilityDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TicketAvailabilityDto>> GetAvailability(Guid eventId, CancellationToken cancellationToken)
        {
            var availability = await _mediator.Send(new GetTicketAvailabilityQuery(eventId), cancellationToken);

            return Ok(availability);
        }
    }
}
