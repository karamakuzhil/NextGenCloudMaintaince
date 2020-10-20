using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Device.Application.Device.Commands.RegisterDevice
{
    public class RegisterDeviceCommand : IRequest<string>
    {
        [Required]
        public string Brand { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string HardwareId { get; set; }
        [Required]
        public string TenantId { get; set; }
    }
}
