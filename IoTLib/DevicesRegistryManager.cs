using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Shared;
using System.Text.Json;
using Microsoft.Azure.Devices;
using IoTLib.Devices;
using Microsoft.Azure.Devices.Client;

namespace IoTLib
{
    public static class DevicesRegistryManager {
        private static RegistryManager _registryManager;
        public static void setRegistryManager(RegistryManager registryManager) => DevicesRegistryManager._registryManager = registryManager;
        public async static Task<IoTDevice> registerDeviceOnIotHub(string deviceId, IoTDeviceProperties props, string connectionString){
            string deviceIdMac = $"{deviceId}_{props.Mac}";
            var device = await registerDevice(deviceIdMac);

            Dictionary<string, dynamic> deviceReportedProperties = new Dictionary<string, dynamic>{};
            deviceReportedProperties.Add("Mac", props.Mac);
            deviceReportedProperties.Add("AutoRegistered", props.AutoRegistered.ToString());
            deviceReportedProperties.Add("DeviceType", props.DeviceType);
            await updateReportedProperties(deviceIdMac, deviceReportedProperties, connectionString);

            Dictionary<string, dynamic> deviceDesiredProperties = new Dictionary<string, dynamic>{};
            deviceDesiredProperties.Add("TelemetryData", props.TelemetryData.ToString());
            deviceDesiredProperties.Add("Controller", props.Controller.ToString());
            if (props.TelemetryData)
                IoTDevice.defaultTelemetryTwinProperties.ToList().ForEach(x => deviceDesiredProperties.Add(x.Key, x.Value));
            if (props.Controller)
                IoTDevice.defaultControllerTwinProperties.ToList().ForEach(x => deviceDesiredProperties.Add(x.Key, x.Value));
            await DevicesRegistryManager.updateDesiredProperties(deviceIdMac, deviceDesiredProperties);

            ArbitraryTags tags = new ArbitraryTags();
            tags.Tags = new Dictionary<string, dynamic>
            {
                { "NickName", props.NickName },
                { "User", props.User }
            };
            Twin twin = await DevicesRegistryManager.updateDeviceTags(deviceIdMac, tags);
            return IoTDeviceBuilder.newDeviceFromTwin(twin);
        }
        private async static Task<Device> registerDevice(string deviceId){
            var device = new Microsoft.Azure.Devices.Device(deviceId);
            return await _registryManager.AddDeviceAsync(device);
        }
        public async static Task<Twin> updateDesiredProperties(string deviceId, Dictionary<string, dynamic> twinProperties){
            var twin = await _registryManager.GetTwinAsync(deviceId);
            foreach (KeyValuePair<string, dynamic> property in twinProperties)
            {
                twin.Properties.Desired[property.Key] = property.Value;
            }
            twin = await _registryManager.UpdateTwinAsync(twin.DeviceId, twin, twin.ETag);
            return twin;
        }
        public async static Task<Twin> updateReportedProperties(string deviceId, Dictionary<string, dynamic> twinProperties, string connectionString){
            string DeviceConnectionString = $"{connectionString};DeviceId={deviceId}";
            DeviceClient Client = DeviceClient.CreateFromConnectionString(DeviceConnectionString, Microsoft.Azure.Devices.Client.TransportType.Mqtt);
            TwinCollection reportedProperties = new TwinCollection();
            //should we get the existing ones and add them before update?
            foreach (KeyValuePair<string, dynamic> property in twinProperties)
            {
                reportedProperties[property.Key] = property.Value;
            }
            await Client.UpdateReportedPropertiesAsync(reportedProperties);
            return await _registryManager.GetTwinAsync(deviceId);
        }
        public async static Task<Twin> updateDeviceTags(string deviceId, Tags deviceTags){
            var twin = await _registryManager.GetTwinAsync(deviceId);
            twin = await _registryManager.UpdateTwinAsync(twin.DeviceId, deviceTags.patch(), twin.ETag);
            return twin;
        }
        public async static Task removeDevice(string deviceId) => await _registryManager.RemoveDeviceAsync(deviceId);
        public async static Task<Device> updateDeviceStatus(string deviceId, DeviceStatus status){
            var device = await _registryManager.GetDeviceAsync(deviceId);
            device.Status = status;
            device = await _registryManager.UpdateDeviceAsync(device);
            return device;
        }
    }
}