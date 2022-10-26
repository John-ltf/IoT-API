using System;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using System.Collections.Generic;
using System.Threading.Tasks ;
using System.Linq;
using IoTLib.Devices;

namespace IoTLib
{
    public static class DevicesFetch {
        private static RegistryManager _registryManager;
        public static void setRegistryManager(RegistryManager registryManager) => DevicesFetch._registryManager = registryManager;

        public async static Task<List<IoTDevice>> getDevices(List<(string,string)> conditions)
        {
            List<IoTDevice> devices = new List<IoTDevice>();
            string whereClause = string.Join(" and ", conditions.Select(t => string.Format("{0} = '{1}'", t.Item1, t.Item2)));
            var query = _registryManager.CreateQuery($"SELECT * FROM devices where {whereClause}");
            while (query.HasMoreResults)
            {
                var page = await query.GetNextAsTwinAsync();
                foreach (var twin in page)
                {
                    devices.Add(IoTDeviceBuilder.newDeviceFromTwin(twin));
                }
            }
            return devices;
        }

        public async static Task<List<IoTDeviceEntry>> getDevicesList(List<(string,string)> conditions)
        {
            List<IoTDeviceEntry> devices = new List<IoTDeviceEntry>();
            string whereClause = string.Join(" and ", conditions.Select(t => string.Format("{0} = '{1}'", t.Item1, t.Item2)));
            var query = _registryManager.CreateQuery($"SELECT * FROM devices where {whereClause}");
            while (query.HasMoreResults)
            {
                var page = await query.GetNextAsTwinAsync();
                foreach (var twin in page)
                {
                    devices.Add( new IoTDeviceEntry(twin.DeviceId, twin.Tags.Contains("NickName") ? twin.Tags["NickName"].ToString(): "" ));
                }
            }
            return devices;
        }
    }
}