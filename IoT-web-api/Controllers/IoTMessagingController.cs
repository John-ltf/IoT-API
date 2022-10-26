using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IoTLib;

namespace IoT_API.Controllers
{
    public class IoTMessagingController : BaseApiController
    {
        //private readonly Iiothub _iotHub;
        private readonly IiotServiceClient _iotServiceClient;
        public IoTMessagingController(IiotServiceClient iotServiceClient)
        {
            _iotServiceClient = iotServiceClient;
        }

        [HttpPost("direct-method/{deviceId}/")]
        public async Task<ActionResult<string>> newDevice(string deviceId, DeviceDirectMethod method) => await _iotServiceClient.sendDirectMethod(deviceId, method);
    }
}
