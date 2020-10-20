using System;

namespace Devices.Domain.AggregatesModel
{
    public class Device
    {
        public string DeviceId { get; private set; }
        public string Brand { get; set; }
        public string Type { get; set; }
        public string HardwareId { get; set; }
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
        public DeviceRegistrationState DeviceRegistrationState { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public Device(string tenantId, string brand, string type, string hardwareId, DeviceRegistrationState deviceRegistrationState)
        {
            DeviceId = Guid.NewGuid().ToString();
            TenantId = tenantId;
            Brand = brand;
            Type = type;
            HardwareId = hardwareId;
            DeviceRegistrationState = deviceRegistrationState;
        }

        public void ActivateSubscription(string subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }
    }
}
