using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;

namespace IoTLib
{

    public abstract class Tags
    {
        public abstract string patch();
    }
    public class initializeTags : Tags
    {
        public override string patch() =>
        $@"{{ tags:
            {{
                User: '{User}',
                NickName: '{NickName}',
                MAC: '{Mac}',
                TelemetryData: {TelemetryData.ToString().ToLower()},
                Controller: {Controller.ToString().ToLower()},
                AutoRegistered: {AutoRegistered.ToString().ToLower()}
            }}
        }}";
        public string User { get; set; }
        public string NickName { get; set; }
        public string Mac { get; set; }
        public Boolean TelemetryData { get; set; }
        public Boolean Controller { get; set; }

        public Boolean AutoRegistered { get; set; }
    }

    public class ArbitraryTags : Tags
    {
        public override string patch()
        {
            string tagStr = string.Join(",", Tags.Select(x => $"{x.Key}: '{Convert.ToString(x.Value)}'").ToArray());
            string patch = $@"{{ tags:
                {{
                    {tagStr}
                }}
            }}";
            return patch;
        }
        public Dictionary<string, dynamic> Tags { get; set;}
        public void updateTags(string tagName, string tagValue){
            if(Tags.ContainsKey(tagName))
                Tags[tagName] = tagValue;
            else
                Tags.Add(tagName, tagValue);
        }
    }
}