using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ripple.EventsTicketing.Application.Events.Commands;
using Ripple.EventsTicketing.Application.Events.Queries;

namespace Ripple.EventsTicketing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EventsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Gets all events.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<EventDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyCollection<EventDto>>> GetAll(CancellationToken cancellationToken)
        {
            var events = await _mediator.Send(new GetAllEventsQuery(), cancellationToken);

            return Ok(events);
        }

        /// <summary>Gets a single event by id.</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EventDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var eventDto = await _mediator.Send(new GetEventQuery(id), cancellationToken);

            return Ok(eventDto);
        }

        /// <summary>Creates a new event.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(EventDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EventDto>> Create(CreateEventCommand command, CancellationToken cancellationToken)
        {
            var eventDto = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = eventDto.Id }, eventDto);
        }

        /// <summary>Updates an existing event.</summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(Guid id, UpdateEventRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateEventCommand(
                id,
                request.Name,
                request.Description,
                request.Venue,
                request.StartsAt,
                request.TotalCapacity,
                request.PricingTiers);

            await _mediator.Send(command, cancellationToken);

            return NoContent();
        }

        /// <summary>Deletes an event.</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteEventCommand(id), cancellationToken);

            return NoContent();
        }
    }

    public record UpdateEventRequest(
        string Name,
        string Description,
        string Venue,
        DateTimeOffset StartsAt,
        int TotalCapacity,
        IReadOnlyCollection<UpdatePricingTierCommand> PricingTiers);
}
