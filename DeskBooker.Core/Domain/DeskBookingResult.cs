using DeskBooker.Core.Processor;

namespace DeskBooker.Core.Domain
{
    public class DeskBookingResult : DeskBookingBase
    {
        public DeskBookingResultCode Code { get; set; }
    }
}