using System;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using System.Collections.Generic;
using System.Threading.Tasks ;
using System.Linq;
using IoTLib.Devices;

namespace IoTLib
{
    public interface Iiothub{
        Task<List<IoTDeviceEntry>> getUserDevicesList(string user);
        Task<List<IoTDevice>> getUserDevices(string user);
        Task<List<IoTDevice>> getDevice(string deviceId, string user);
        Task<List<IoTDevice>> getDevice(string deviceId, string user, string mac);
        Task<List<IoTDevice>> registerDevice(string deviceId, IoTDeviceProperties tags);
        Task<List<IoTDevice>> updateDevice(string deviceId, string user, string updateType, string updateName, string updateValue);
        Task<Boolean> deleteDevice(string deviceId, string user);
        Task<Boolean> updateStatusDevice(string deviceId, string user, string statusDescription);
    }

    public class IotHub : Iiothub
    {
        //Not keeping the Registry Manager, use the class as a mediator?
        //private readonly RegistryManager _registryManager;

        private readonly string connectionString;
        public IotHub(string connectionString)
        {
            this.connectionString = connectionString;
            var _registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            DevicesFetch.setRegistryManager(_registryManager);
            DevicesRegistryManager.setRegistryManager(_registryManager);
        }
        public async Task<List<IoTDeviceEntry>> getUserDevicesList(string user) =>
            await DevicesFetch.getDevicesList(new List<(string, string)>
                    {
                        ("tags.User", user)
                    });
        public async Task<List<IoTDevice>> getUserDevices(string user) =>
            await DevicesFetch.getDevices(new List<(string, string)>
                    {
                        ("tags.User", user)
                    });
        public async Task<List<IoTDevice>> getDevice(string deviceId, string user) =>
            await DevicesFetch.getDevices(new List<(string, string)>
                    {
                        ("tags.User", user),
                        ("DeviceId", deviceId)
                    });
        public async Task<List<IoTDevice>> getDevice(string deviceId, string user, string mac) =>
            await DevicesFetch.getDevices(new List<(string, string)>
                    {
                        ("tags.User", user),
                        ("DeviceId", deviceId),
                        ("tags.Mac", mac)
                    });
        public async Task<List<IoTDevice>> registerDevice(string deviceId, IoTDeviceProperties props){
            var devices = await getDevice(deviceId, props.User, props.Mac);
            if ( devices.Any() )
            {
                return devices;
            }
            return new List<IoTDevice>()
                {
                    await DevicesRegistryManager.registerDeviceOnIotHub(deviceId, props, this.connectionString)
                };

        }
        public async Task<List<IoTDevice>> updateDevice(string deviceId, string user, string updateType, string updateName, string updateValue){
            var devices = await getDevice(deviceId, user);
            if ( devices.Any() && devices.Count == 1 )
            {
                IoTDevice device = devices[0];
                if(updateType == "tags")
                {
                    device.updateTags(updateName, updateValue);
                    return new List<IoTDevice>()
                    {
                        IoTDeviceBuilder.newDeviceFromTwin(await DevicesRegistryManager.updateDeviceTags(deviceId, device.Tags))
                    };
                }
                else if(updateType == "desiredProperty")
                {
                    device.cleanDesiredProperties();
                    device.addDesiredProperty(updateName, updateValue);
                    return new List<IoTDevice>()
                    {
                        IoTDeviceBuilder.newDeviceFromTwin(await DevicesRegistryManager.updateDesiredProperties(deviceId, device.DesiredProperties))
                    };
                }
                else if(updateType == "reportedProperty")
                {
                    device.cleanReportedProperties();
                    device.addReportedProperty(updateName, updateValue);
                    return new List<IoTDevice>()
                    {
                        IoTDeviceBuilder.newDeviceFromTwin(await DevicesRegistryManager.updateReportedProperties(deviceId, device.ReportedProperties, connectionString))
                    };
                }

            }
            return null;
        }

        public async Task<Boolean> deleteDevice(string deviceId, string user) {
            if ( getDevice(deviceId, user).Result.Any() ){
                await DevicesRegistryManager.removeDevice(deviceId);
                return true;
            }
            return false;
        }
        public async Task<Boolean> updateStatusDevice(string deviceId, string user, string statusDescription)
        {
            if ( getDevice(deviceId, user).Result.Any() )
            {
                var device = await DevicesRegistryManager.updateDeviceStatus(
                    deviceId,
                    statusDescription.Equals("Enabled") ? DeviceStatus.Enabled : DeviceStatus.Disabled);
                return true;
            }
            return false;
        }

    }
}
