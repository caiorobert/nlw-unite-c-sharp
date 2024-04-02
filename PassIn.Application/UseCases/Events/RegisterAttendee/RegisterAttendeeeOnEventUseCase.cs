using PassIn.Communication.Requests;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using System.Net.Mail;

namespace PassIn.Application.UseCases.Events.RegisterAttendee
{
    public class RegisterAttendeeeOnEventUseCase
    {
        private readonly PassInDbContext _context;
        public RegisterAttendeeeOnEventUseCase()
        {
            _context = new PassInDbContext();
        }

        public ResponseRegisteredJson Execute(Guid eventId, RequestRegisterEventJson request)
        {
            Validate(eventId, request);

            var entity = new Infrastructure.Entities.Attendee
            {
                Email = request.Email,
                Name = request.Name,
                Event_Id = eventId,
                Created_At = DateTime.UtcNow,
            };

            _context.Attendees.Add(entity);
            _context.SaveChanges();

            return new ResponseRegisteredJson
            {
                Id = entity.Id,
            };
        }

        private void Validate(Guid eventId, RequestRegisterEventJson request)
        {
            var eventEntity = _context.Events.Find(eventId);
            if (eventEntity is null)
                throw new NotFoundException("An event with this id don't exist.");

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ErrorOnValidationException("The name is invalid");
            }

            if (!EmailIsValid(request.Email))
            {
                throw new ErrorOnValidationException("The e-mail is invalid");
            }

            var attendeeAlredyRegistered = _context.
                Attendees
                .Any(at => at.Email.Equals(request.Email) && at.Event_Id.Equals(eventId));

            if (attendeeAlredyRegistered)
            {
                throw new ConflictException("You can not register twice on the same event.");
            }

            var attendeeForEvent = _context.Attendees.Count(at => at.Event_Id.Equals(eventId));
            if(attendeeForEvent >= eventEntity.Maximum_Attendees)
            {
                throw new ConflictException("There is no room for this event.");
            }
        }

        private bool EmailIsValid(string email)
        {
            try
            {
                new MailAddress(email);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
