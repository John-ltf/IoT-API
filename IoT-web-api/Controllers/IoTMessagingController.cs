using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IoTLib;
using System.Net;

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
        public async Task<ActionResult<string>> newDevice(string deviceId, DeviceDirectMethod method)
        {
            if(Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"] == method.User)
            {
                return await _iotServiceClient.sendDirectMethod(deviceId, method);
            }
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return null;
        }
    }
}
