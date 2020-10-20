using EventDrivers.Events;

namespace Device.Application.IntegrationEvents.Events
{
    public class DeviceRegistrationIntegrationEvent : IntegrationEvent
    {        
        public string DeviceId { get; set; }

        public DeviceRegistrationIntegrationEvent(string deviceId)
        {
            DeviceId = deviceId;
        }
    }
}
