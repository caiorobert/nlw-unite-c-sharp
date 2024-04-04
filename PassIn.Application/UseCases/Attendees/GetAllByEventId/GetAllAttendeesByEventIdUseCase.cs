using Microsoft.EntityFrameworkCore;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;

namespace PassIn.Application.UseCases.Attendees.GetAllByEventId
{
    public class GetAllAttendeesByEventIdUseCase
    {
        private readonly PassInDbContext _dbContext;
        public GetAllAttendeesByEventIdUseCase()
        {
            _dbContext = new PassInDbContext();
        }

        public ResponseAllAttendeesJson Execute(Guid eventId)
        {
            //var entity = _dbContext.Attendees.Where(attendee => attendee.Event_Id.Equals(eventId)).ToList();
            var entity = _dbContext.Events.Include(e => e.Attendees).ThenInclude(at => at.CheckIn).FirstOrDefault(ev => ev.Id.Equals(eventId));
            if (entity is null)
                throw new NotFoundException("An event with this id don't exist.");

            return new ResponseAllAttendeesJson
            {
                Attendees = entity.Attendees.Select(at => new ResponseAttendeeJson
                {
                    Id = at.Id,
                    Name = at.Name,
                    Email = at.Email,
                    CreatedAt = at.Created_At,
                    CheckedInAt = at.CheckIn?.Created_at
                }).ToList()
            };
        }
    }
}
