using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Shared;
using System.Text.Json;
using Microsoft.Azure.Devices;

namespace IoTLib.Devices
{
    public class IoTDeviceBuilder
    {
        protected IoTDevice ioTDevice;
        public IoTDeviceBuilder() => ioTDevice = new IoTDevice();
        public IoTDevice Build() => ioTDevice;
        public static IoTDeviceBuilder newIoTDevice => new IoTDeviceBuilder();
        public static IoTDevice newDeviceFromTwin(Twin twin) =>
            IoTDeviceBuilder
            .newIoTDevice
            .hasDeviceId(twin.DeviceId)
            .isConnected(twin.ConnectionState.ToString())
            .withStatus(twin.Status.ToString())
            .withTags(twin.Tags)
            .withDesiredPrperties(twin.Properties.Desired)
            .withReportedPrperties(twin.Properties.Reported)
            .Build();
        public IoTDeviceBuilder hasDeviceId(string deviceId)
        {
            ioTDevice.DeviceId = deviceId;
            return this;
        }
        public IoTDeviceBuilder isConnected(string connectionState)
        {
            ioTDevice.ConnectionState = connectionState;
            return this;
        }
        public IoTDeviceBuilder withStatus(string status)
        {
            ioTDevice.Status = status;
            return this;
        }
        public IoTDeviceBuilder withTags(TwinCollection tags)
        {
            ioTDevice.Tags = new ArbitraryTags();
            ioTDevice.Tags.Tags = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(tags.ToString());
            return this;
        }
        public IoTDeviceBuilder withDesiredPrperties(TwinCollection properties)
        {
            //you may use MaxDepth to avoid the metadata copy
            ioTDevice.DesiredProperties = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(properties.ToString());
            return this;
        }
        public IoTDeviceBuilder withReportedPrperties(TwinCollection properties)
        {
            ioTDevice.ReportedProperties = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(properties.ToString());
            return this;
        }
    }

    public class IoTDevice
    {
        public static Dictionary<string, dynamic> defaultTelemetryTwinProperties = new Dictionary<string, dynamic>
        {
                    { "Interval", "60" },
                    { "RetentionPolicyData", "60" }
        };
        public static Dictionary<string, dynamic> defaultControllerTwinProperties = new Dictionary<string, dynamic>{};
        public string DeviceId { get; set;}
        public string ConnectionState { get; set; } //should be Connected, Disabled, Disconnected, or Disconnected_Retrying

        public string Status { get; set;} // should be Disabled or Enabled

        public ArbitraryTags Tags { get; set;}
        public void updateTags( string tagName, string tagValue){
            Tags.updateTags(tagName, tagValue);
        }
        public Dictionary<string, dynamic> DesiredProperties { get; set;}
        public void addDesiredProperty( string propName, string propValue){
            DesiredProperties.Add(propName, propValue);
        }
        public void cleanDesiredProperties(){
             DesiredProperties.Clear();
        }
        public Dictionary<string, dynamic> ReportedProperties { get; set;}
        public void addReportedProperty( string propName, string propValue){
            ReportedProperties.Add(propName, propValue);
        }
        public void cleanReportedProperties(){
             ReportedProperties.Clear();
        }

    }

    public class IoTDeviceEntry {
        public IoTDeviceEntry(string DeviceId, string NickName)
        {
            this.DeviceId = DeviceId;
            this.NickName = NickName;
        }
        public string DeviceId { get; set;}
        public string NickName { get; set;}
    }

    public class IoTDeviceProperties {
        public string DeviceType { get; set; }
        public string User { get; set; }
        public string NickName { get; set; }
        public string Mac { get; set; }
        public Boolean TelemetryData { get; set; }
        public Boolean Controller { get; set; }
        public Boolean AutoRegistered { get; set; }
    }
}