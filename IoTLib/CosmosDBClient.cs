using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IoTLib
{
    public class CosmosDBConnection
    {
        protected CosmosDBClient cosmosDBClient;
        public CosmosDBConnection() => cosmosDBClient = new CosmosDBClient();
        public CosmosDBClient Connect(){
                cosmosDBClient.Connect();
                return cosmosDBClient;
        }
        public static CosmosDBConnection newConnection => new CosmosDBConnection();

        public CosmosDBConnection withLogger(ILogger log)
        {
            cosmosDBClient.Log = log;
            return this;
        }
        public CosmosDBConnection withUri(string endpointUri)
        {
            cosmosDBClient.EndpointUri = endpointUri;
            return this;
        }
        public CosmosDBConnection withPrimaryKey(string primaryKey)
        {
            cosmosDBClient.PrimaryKey = primaryKey;
            return this;
        }
        public CosmosDBConnection withDatabaseId(string databaseId)
        {
            cosmosDBClient.DatabaseId = databaseId;
            return this;
        }
        public CosmosDBConnection withContainerId(string containerId)
        {
            cosmosDBClient.ContainerId = containerId;
            return this;
        }
    }
    public class CosmosDBClient
    {
        public string EndpointUri { private get; set;}
        public string PrimaryKey { private get; set;}
        public string DatabaseId { private get; set;}
        public string ContainerId { private get; set;}
        public ILogger Log { private get; set;}
        //private Database Database;
        private Container Container;
        private CosmosClient cosmosClient;
        public CosmosDBClient Connect()
        {
            try
            {
                this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
                Container = cosmosClient.GetContainer(DatabaseId, ContainerId);
            }
            catch (CosmosException cosmosException)
            {
                Log.LogError($"Cosmos Exception with Status {cosmosException.StatusCode} : {cosmosException}\n");
            }
            catch (Exception e)
            {
                Log.LogError($"Error: {e}");
            }
            return this;
        }

/*

CosmosDBClient cosmosDBClient = CosmosDBConnection.
                                            newConnection.
                                            withLogger(log).
                                            withUri(Environment.GetEnvironmentVariable("CosmosDbUri")).
                                            withPrimaryKey(Environment.GetEnvironmentVariable("CosmosDbPimaryKey")).
                                            withDatabaseId(Environment.GetEnvironmentVariable("CosmosDbName")).
                                            withContainerId(Environment.GetEnvironmentVariable("CosmosDbContainer")).
                                            Connect();

                                            TaskList.Add(cosmosDBClient.insertTelemetry(eventData.SystemProperties["iothub-connection-device-id"].ToString(), eventData.Data.ToString()));

public async Task<Boolean> insertTelemetry(string deviceId, string data)
        {
            Dictionary<string, dynamic> item = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(data);
            item.Add("deviceId", deviceId); //add device Id
            item.Add("id", Guid.NewGuid()); //add id property

            //set data time to the Cosmos DB compatible format
            if(item.ContainsKey("time"))
            {
                DateTime dataTime = DateTime.ParseExact(item["time"].ToString(), "yyyy-MM-dd:HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                item["time"] = dataTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ");
            }
            //set ttl on item level
            if(item.ContainsKey("ttl"))
            {
                try
                {
                    item["ttl"] = Int32.Parse(item["ttl"]) * 60 * 60 * 24; //from days to seconds
                }
                catch (FormatException)
                {
                    Log.LogError($"Unable to parse ttl '{item["ttl"]}'");
                }
            }

            try
            {
                ItemResponse<Dictionary<string, dynamic>> telemetryData = await Container.CreateItemAsync<Dictionary<string, dynamic>>(item, new PartitionKey(item["deviceId"]));
                return true;
            }
            catch (CosmosException cosmosException)
            {
                Log.LogError($"Cosmos Exception with Status {cosmosException.StatusCode} : {cosmosException}\n");
            }
            catch (AggregateException aggregateException)
            {
                Log.LogError($"Error: {aggregateException}");
            }
            return false;
        }*/

    }
}