using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IoTLib;
using IoTLib.Devices;
using Microsoft.Azure.Devices;
using System.Net;

namespace IoT_API.Controllers
{
    public class IoTHubController : BaseApiController
    {
        private readonly Iiothub _iotHub;
        public IoTHubController(Iiothub iotHub)
        {
            _iotHub = iotHub;
        }

        [HttpGet("devices/list/")]
        public async Task<ActionResult<List<IoTDeviceEntry>>> getUserDevicesList([FromQuery(Name = "user")]  string user)
        {
            if(Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"] == user)
            {
                return await _iotHub.getUserDevicesList(user);
            }
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return null;
        }

        [HttpGet("devices/")]
        public async Task<ActionResult<List<IoTDevice>>> getUserDevices([FromQuery(Name = "user")]  string user)
        {
            if(Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"] == user)
            {
                return await _iotHub.getUserDevices(user);
            }
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return null;
        }

        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult<List<IoTDevice>>> getDevice(  string deviceId, [FromQuery(Name = "user")] string user)
        {
            if(Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"] == user)
            {
                return await _iotHub.getDevice(deviceId, user);
            }
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return null;
        }

        [HttpPost("device/{deviceId}")]
        public async Task<ActionResult<List<IoTDevice>>> newDevice(string deviceId, IoTDeviceProperties props)
        {
            if(Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"] == props.User)
            {
                return await _iotHub.registerDevice(deviceId, props);
            }
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return null;

        }

        [HttpPut("device/{deviceId}")]
        public async Task<ActionResult<List<IoTDevice>>> updateDevice(string deviceId,
                                                                        [FromQuery(Name = "user")] string user,
                                                                        [FromQuery(Name = "updateType")] string updateType,
                                                                        [FromQuery(Name = "updateName")] string updateName,
                                                                        [FromQuery(Name = "updateValue")] string updateValue )
        {
            if(Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"] == user)
            {
                return await _iotHub.updateDevice(deviceId, user, updateType, updateName, updateValue);
            }
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return null;
        }

        [HttpDelete("device/{deviceId}")]
        public Task<Boolean> deleteDevice(string deviceId, [FromQuery(Name = "user")] string user)
        {
            if(Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"] == user)
            {
                return _iotHub.deleteDevice(deviceId, user);
            }
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return null;
        }

        [HttpPut("device/{deviceId}/status/{statusVal}")]
        public async Task<Boolean> updateDeviceStatus(string deviceId, string statusVal, [FromQuery(Name = "user")] string user)
        {
            if(Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"] == user)
            {
                return await _iotHub.updateStatusDevice(deviceId, user, statusVal);
            }
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return false;
        }
    }
}
