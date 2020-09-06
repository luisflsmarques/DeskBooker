using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace DeskBooker.Core.Processor
{
    public class DeskBookingRequestProcessorTests
    {
        private readonly DeskBookingRequestProcessor _processor;
        private readonly DeskBookingRequest _request;
        private readonly List<Desk> _availableDesks;
        private readonly Mock<IDeskBookingRepository> _deskBookingRepositoryMock;
        private readonly Mock<IDeskRepository> _deskRepositoryMock;

        public DeskBookingRequestProcessorTests()
        {
            _request = new DeskBookingRequest
            {
                FirstName = "Luis",
                LastName = "Marques",
                Email = "luis.fls.marques@gmail.com",
                Date = new DateTime(2020, 9, 05)
            };

            _availableDesks = new List<Desk> { new Desk()};

            //Arrange or Setup or Given
            ////this is to create a mock implementation of the repository
            _deskBookingRepositoryMock = new Mock<IDeskBookingRepository>();
            _deskRepositoryMock = new Mock<IDeskRepository>();
            _deskRepositoryMock.Setup(x => x.GetAvailableDesks(_request.Date))
                .Returns(_availableDesks);
            
            _processor = new DeskBookingRequestProcessor(_deskBookingRepositoryMock.Object, _deskRepositoryMock.Object);
            
            
        }

        [Fact]
        public void ShouldReturnDeskBookingResultWithRequestValues()
        {


            //Act or When
            DeskBookingResult result = _processor.BookDesk(_request);

            //Assert or Then
            Assert.NotNull(result);
            Assert.Equal(_request.FirstName, result.FirstName);
            Assert.Equal(_request.LastName, result.LastName);
            Assert.Equal(_request.Email, result.Email);
            Assert.Equal(_request.Date, result.Date);
        }

        [Fact]
        public void ShouldThrowExceptionIfRequestIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => _processor.BookDesk(null));

            // checking that the argument null exception is relative to the request parameter and not some other param
            Assert.Equal("request", exception.ParamName);
        }

        [Fact]
        public void ShouldSaveDeskBooking()
        {
            //GIVEN
            DeskBooking savedDeskBooking = null;
            
            // this is basically to say that when Save gets called with a DeskBooking object, it should save it in 
            // the variable savedDeskBooking.
            _deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))
                .Callback<DeskBooking>(deskBooking =>
                {
                    savedDeskBooking = deskBooking;
                });

            //WHEN
            _processor.BookDesk(_request);

            //THEN
            
            //verify that the processor called the save method once when booking a desk.
            _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Once);

            Assert.NotNull(savedDeskBooking);
            Assert.Equal(_request.FirstName, savedDeskBooking.FirstName);
            Assert.Equal(_request.LastName, savedDeskBooking.LastName);
            Assert.Equal(_request.Email, savedDeskBooking.Email);
            Assert.Equal(_request.Date, savedDeskBooking.Date);
        }

        [Fact]
        public void ShouldNotSaveDeskBookingIfNoDeskIsAvailable()
        {
            //GIVEN
            _availableDesks.Clear();
            
            //WHEN
            _processor.BookDesk(_request);

            //THEN

            //verify that the processor called the save method once when booking a desk.
            _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Never);
        }
    }
}
