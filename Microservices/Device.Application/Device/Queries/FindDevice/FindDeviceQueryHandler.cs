using Devices.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Device.Application.Device.Queries.FindDevice
{
    public class FindDeviceQueryHandler : IRequestHandler<FindDeviceQuery, Devices.Domain.AggregatesModel.Device>
    {
        private readonly IDeviceRespository _deviceContext;

        public FindDeviceQueryHandler(IDeviceRespository context)
        {
            _deviceContext = context;
        }

        public async Task<Devices.Domain.AggregatesModel.Device> Handle(FindDeviceQuery request, CancellationToken cancellationToken)
        {
            return await _deviceContext.FindByIdAsync(request.DeviceId);
        }
    }
}
