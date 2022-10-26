using System;
using Microsoft.Azure.Devices;

namespace IoTLib
{
    public class DeviceDirectMethodBuilder
    {
        public static CloudToDeviceMethod NewcloudToDeviceMethod(DeviceDirectMethod deviceDirectMethod) =>
            new CloudToDeviceMethod(deviceDirectMethod.MethodName,
                                    TimeSpan.FromSeconds(deviceDirectMethod.ResponseTimeout),
                                    TimeSpan.FromSeconds(deviceDirectMethod.ConnectionTimeout));
    }

    public class DeviceDirectMethod
    {
        public string User{ get; set; }
        public string Payload{ get; set; }
        public string MethodName { get; set; }
        public double ResponseTimeout { get; set; }
        public double ConnectionTimeout { get; set; }
    }
}