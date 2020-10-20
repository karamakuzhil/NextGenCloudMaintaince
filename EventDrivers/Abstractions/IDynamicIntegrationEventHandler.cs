using System.Threading.Tasks;

namespace EventDrivers.Abstractions
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}
