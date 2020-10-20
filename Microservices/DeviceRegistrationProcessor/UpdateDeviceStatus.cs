using EventDrivers.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DeviceRegistrationProcessor
{
    public static class UpdateDeviceStatus
    {

        public static void Run([ServiceBusTrigger("deviceregistrationtopic", "readprojection", Connection = "ServiceBus")] Message msg, ILogger log)
        {
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {msg}");

            try
            {
                if (msg.Label == "DeviceRegistered")
                {
                    var eventMsg = JsonConvert.DeserializeObject<DeviceRegistrationIntegrationEvent>(Encoding.UTF8.GetString(msg.Body));
                    //Check Message and Validate Tenant Id and Subscription Id
                    //Write Event to Bus after Validation
                }
            }
            catch(Exception ex)
            {
                log.LogError($"C# ServiceBus topic trigger function error: {ex.Message}");
                throw;
            }
         }

        public class DeviceRegistrationIntegrationEvent : IntegrationEvent
        {
            public string DeviceId { get; set; }

            public string TenantId { get; set; }

            public string SubscriptionId { get; set; }

            public DeviceRegistrationIntegrationEvent(string deviceId, string tenantId, string subscriptionId)
            {
                DeviceId = deviceId;
                TenantId = tenantId;
                SubscriptionId = subscriptionId;
            }
        }


    }
}
