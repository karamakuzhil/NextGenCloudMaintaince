using Device.Application.IntegrationEvents.Events;
using Devices.Domain.AggregatesModel;
using Devices.Domain.Interfaces;
using EventDrivers.Abstractions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Device.Application.Device.Commands.RegisterDevice
{
    public class RegisterDeviceCommandHandler : IRequestHandler<RegisterDeviceCommand, string>
    {
        private readonly IEventBus _eventBus;
        private readonly IDeviceRespository _deviceContext;

        public RegisterDeviceCommandHandler(IEventBus eventBus,IDeviceRespository context)
        {
            _deviceContext = context;
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            // _bookingIntegrationEventService = bookingIntegrationEventService ?? throw new ArgumentNullException(nameof(bookingIntegrationEventService));
        }

        public async Task<string> Handle(RegisterDeviceCommand request, CancellationToken cancellationToken)
        {
            //Create Agreegate Root
            var deviceInfo = new Devices.Domain.AggregatesModel.Device(request.TenantId, request.Brand, request.Type, request.HardwareId
                , DeviceRegistrationState.Pending);

            //Create Integration Event
            var deviceAddIntegrationEvent = new DeviceRegistrationIntegrationEvent(deviceInfo.DeviceId);


            //Save the Data in local DB
            var deviceId = await _deviceContext.AddAsync(deviceInfo);

            //Publish Event to Service Bus Topic
            _eventBus.Publish(deviceAddIntegrationEvent);

            //Return Booking Ref
            return deviceId;
        }
    }
}
