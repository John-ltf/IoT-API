using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using System.Text;
using Newtonsoft.Json;

namespace IoTLib
{
    public interface IiotServiceClient{
        Task<string> sendDirectMethod(string deviceId, DeviceDirectMethod cloudToDeviceMethod);
    }

    public class IotServiceClient : IiotServiceClient
    {
        private readonly ServiceClient _serviceClient;

        public IotServiceClient(string connectionString)
        {
            _serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
        }

        public async Task<string> sendDirectMethod(string deviceId, DeviceDirectMethod cloudToDeviceMethodBuilder){
            try
            {
                var cloudToDeviceMethod = DeviceDirectMethodBuilder.NewcloudToDeviceMethod(cloudToDeviceMethodBuilder);
                cloudToDeviceMethod.SetPayloadJson(cloudToDeviceMethodBuilder.Payload);
                var response = await _serviceClient.InvokeDeviceMethodAsync(deviceId, cloudToDeviceMethod);
                return response.GetPayloadAsJson();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}