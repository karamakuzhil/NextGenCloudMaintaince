using Devices.Domain.AggregatesModel;
using System.Threading.Tasks;

namespace Devices.Domain.Interfaces
{
    public interface IDeviceRespository
    {
        Task<string> AddAsync(Device device);
        Task<Device> UpdateAsync(Device device);
        Task<Device> FindByIdAsync(string deviceId);
    }
}
