namespace Device.Application.Device.Queries.DTO
{
    public class DeviceDTO
    {
        public string DeviceId { get; private set; }
        public string Brand { get; set; }
        public string Type { get; set; }
        public string HardwareId { get; set; }
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
    }
}
