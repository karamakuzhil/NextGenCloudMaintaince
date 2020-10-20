using Devices.Domain.AggregatesModel;
using Devices.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace Device.Persistance.Repositories
{
    public class DeviceRepository: IDeviceRespository
    {
        private readonly DeviceDbContext _context;

        public DeviceRepository(DeviceDbContext dbContext)
        {
            _context = dbContext;
        }

        //Add the book
        public async Task<string> AddAsync(Devices.Domain.AggregatesModel.Device device)
        {
            device.CreatedDate = DateTime.Now;

            _context.Set<Devices.Domain.AggregatesModel.Device>().Add(device);
            await _context.SaveChangesAsync();

            return device.DeviceId;
        }

        //Update The Booking
        public async Task<Devices.Domain.AggregatesModel.Device> UpdateAsync(Devices.Domain.AggregatesModel.Device device)
        {
            device.UpdatedDate = DateTime.Now;

            //_context.Bookings.Update(bookingOrder);
            await _context.SaveChangesAsync();

            return device;
        }

        //Find Booking By ID
        public async Task<Devices.Domain.AggregatesModel.Device> FindByIdAsync(string deviceId)
        {
            var bookingOrder = await _context.Devices.FindAsync(deviceId);
           
            return bookingOrder;
        }
    }
}
