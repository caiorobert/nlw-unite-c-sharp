using PassIn.Communication.Requests;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;

namespace PassIn.Application.UseCases.Events.Register
{
    public class RegisterEventUseCase
    {
        private readonly PassInDbContext _context;
        public RegisterEventUseCase()
        {
            _context = new PassInDbContext();
        }

        public ResponseRegisteredJson Execute(RequestEventJson request)
        {
            Validate(request);

            var entity = new Infrastructure.Entities.Event
            {
                Title = request.Title,
                Details = request.Details,
                Maximum_Attendees = request.MaximumAttendees,
                Slug = request.Title.ToLower().Replace(" ", "-"),
            };

            _context.Events.Add(entity);
            _context.SaveChanges();

            return new ResponseRegisteredJson
            {
                Id = entity.Id
            };
        }

        private void Validate(RequestEventJson request)
        {
            if (request.MaximumAttendees <= 0)
            {
                throw new ErrorOnValidationException("The Maximum attends is invalid");
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ErrorOnValidationException("The title is invalid");
            }

            if (string.IsNullOrWhiteSpace(request.Details))
            {
                throw new ErrorOnValidationException("The details are invalid");
            }
        }
    }
}
