using Microsoft.AspNetCore.Mvc;
using PassIn.Application.UseCases.Events.RegisterAttendee;
using PassIn.Communication.Requests;
using PassIn.Communication.Responses;

namespace PassIn.Api.Controllers
{
    public class AteendeesController : ControllerBase
    {
        [HttpPost]
        [Route("{eventId}/register")]
        [ProducesResponseType(typeof(ResponseRegisteredJson), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status409Conflict)]
        public IActionResult Register([FromRoute] Guid eventId, [FromBody] RequestRegisterEventJson request)
        {
            var useCase = new RegisterAttendeeeOnEventUseCase();

            var response = useCase.Execute(eventId, request);
            return Created(string.Empty, response);
        }
    }
}
