using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using PassIn.Infrastructure.Entities;

namespace PassIn.Application.UseCases.Checkins.DoCheckin
{
    public class DoAttendeeCheckinUseCase
    {
        private readonly PassInDbContext _dbContext;
        public DoAttendeeCheckinUseCase()
        {
            _dbContext = new PassInDbContext();
        }

        public ResponseRegisteredJson Execute(Guid attendeeId)
        {
            Validate(attendeeId);

            var entity = new CheckIn
            {
                Attendee_Id = attendeeId,
                Created_at = DateTime.UtcNow,
            };

            _dbContext.checkIns.Add(entity);
            _dbContext.SaveChanges();

            return new ResponseRegisteredJson
            {
                Id = entity.Id,
            };
        }

        private void Validate(Guid attendeeId)
        {
            var existAttendee = _dbContext.Attendees.Any(at => at.Id.Equals(attendeeId));
            if (!existAttendee)
                throw new NotFoundException("The attendee with this Id was not found.");

            var existCheckin = _dbContext.checkIns.Any(ch => ch.Attendee_Id.Equals(attendeeId));
            if(existCheckin)
                throw new ConflictException("Attendee can not do checking twice in the same event.");
        }
    }
}
