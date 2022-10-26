using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IoTLib;
using IoTLib.Devices;
using Microsoft.Azure.Devices;

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
        public async Task<ActionResult<List<IoTDeviceEntry>>> getUserDevicesList([FromQuery(Name = "user")]  string user) => await _iotHub.getUserDevicesList(user);

        [HttpGet("devices/")]
        public async Task<ActionResult<List<IoTDevice>>> getUserDevices([FromQuery(Name = "user")]  string user) => await _iotHub.getUserDevices(user);

        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult<List<IoTDevice>>> getDevice(  string deviceId, [FromQuery(Name = "user")] string user) => await _iotHub.getDevice(deviceId, user);

        [HttpPost("device/{deviceId}")]
        public async Task<ActionResult<List<IoTDevice>>> newDevice(string deviceId, IoTDeviceProperties props) => await _iotHub.registerDevice(deviceId, props);

        [HttpPut("device/{deviceId}")]
        public async Task<ActionResult<List<IoTDevice>>> updateDevice(string deviceId,
                                                                        [FromQuery(Name = "user")] string user,
                                                                        [FromQuery(Name = "updateType")] string updateType,
                                                                        [FromQuery(Name = "updateName")] string updateName,
                                                                        [FromQuery(Name = "updateValue")] string updateValue )
                                                                        => await _iotHub.updateDevice(deviceId, user, updateType, updateName, updateValue);

        [HttpDelete("device/{deviceId}")]
        public Task<Boolean> deleteDevice(string deviceId, [FromQuery(Name = "user")] string user) => _iotHub.deleteDevice(deviceId, user);

        [HttpPut("device/{deviceId}/status/{statusVal}")]
        public async Task<Boolean> updateDeviceStatus(string deviceId, string statusVal, [FromQuery(Name = "user")] string user) => await _iotHub.updateStatusDevice(deviceId, user, statusVal);
    }
}
