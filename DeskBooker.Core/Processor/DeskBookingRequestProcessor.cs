using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using System;
using System.Linq;

namespace DeskBooker.Core.Processor
{
    //Some general principles - a class or method should have only a single responsibility
    //in other words, a class or method should have only one reason to change.
    //so we should not add the save to DB feature to the processor, thats a separate responsibility
    //that is where dependency injection comes in.
    //components must depend on abstractions and not on implementations. The dependency should be modeled correctly via
    //an interface - not via a specific implementation - this is so we can replace one implementation for another.
    //so this means that DeskBookingRequestProcesser should depend on IdeskBookingRepository of which DeskBookingRepository
    //is an implementation of that interface. One of many implementations.
    //Processor > Interface > Implementation of the interface > DB
    public class DeskBookingRequestProcessor
    {
        private readonly IDeskBookingRepository _deskBookingRepository;
        private readonly IDeskRepository _deskRepository;

        public DeskBookingRequestProcessor(IDeskBookingRepository deskBookingRepository, IDeskRepository deskRepository)
        {
            _deskBookingRepository = deskBookingRepository;
            _deskRepository = deskRepository;
        }

        public DeskBookingResult BookDesk(DeskBookingRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var result = Create<DeskBookingResult>(request);

            var availableDesks = _deskRepository.GetAvailableDesks(request.Date);
            
            if (availableDesks.Count() > 0)
            {
                var availableDesk = availableDesks.First();
                var deskBooking = Create<DeskBooking>(request);
                deskBooking.DeskId = availableDesk.Id;
                _deskBookingRepository.Save(deskBooking);
                result.Code = DeskBookingResultCode.Success;
            }
            else
            {
                result.Code = DeskBookingResultCode.NoDeskAvailable;
            }

            return result;
        }

        private static T Create<T>(DeskBookingRequest request) where T:DeskBookingBase, new()
        {
            return new T
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Date = request.Date
            };
        }
    }
}