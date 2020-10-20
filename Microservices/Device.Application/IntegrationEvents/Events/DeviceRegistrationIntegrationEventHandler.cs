using EventDrivers.Abstractions;
using System;
using System.Threading.Tasks;

namespace Device.Application.IntegrationEvents.Events
{
    public class DeviceRegistrationIntegrationEventHandler : IIntegrationEventHandler<DeviceRegistrationIntegrationEvent>
    {
        public DeviceRegistrationIntegrationEventHandler()
        {

        }

        public async Task Handle(DeviceRegistrationIntegrationEvent eventMsg)
        {
            if (eventMsg.Id != Guid.Empty)
            {
                try
                {
                    var deviceId = eventMsg.DeviceId;
                }
                catch
                {                   
                    throw; //Throw the message so message queue abandons it
                }
            }
        }
    }
}
