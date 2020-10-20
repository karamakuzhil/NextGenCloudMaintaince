using MediatR;

namespace Device.Application.Device.Queries.FindDevice
{
    public class FindDeviceQuery : IRequest<Devices.Domain.AggregatesModel.Device>
    {
        public string DeviceId { get; set; }
    }
}
