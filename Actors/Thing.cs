using Actors.Interfaces;
using Domain;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Actors
{
    [StatePersistence(StatePersistence.Persisted)]
    public class Thing : Actor, IThing
    {
        private readonly string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=testewinner;AccountKey=mb2rGzqF3XmWpaUFIvL9XiXP925asJ0DgxV/fg0nf6pfh674WR6OgeZWwz1tXF0hssv3JG8FNy86YI7vjdNaIA==;EndpointSuffix=core.windows.net";
        private ThingState State = new ThingState();
        public Thing(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
        }

        protected override Task OnActivateAsync()
        {
            State._telemetry = new List<ThingTelemetry>();
            State._deviceGroupId = ""; // not activated
            return base.OnActivateAsync();
        }

        public Task SendTelemetryAsync(ThingTelemetry telemetry)
        {
            State._telemetry.Add(telemetry); // saving data at the device level
            if (State._deviceGroupId != "")
            {
                var deviceGroup = ActorProxy.Create<IThingGroup>(new ActorId(State._deviceGroupId));
                return deviceGroup.SendTelemetryAsync(telemetry); // sending telemetry data for aggregation
            }
            return Task.FromResult(true);
        }

        public Task ActivateMeAsync(string region, int version)
        {
            CloudTable cloudTable;
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            cloudTable = cloudTableClient.GetTableReference("Equipamentos");
            cloudTable.CreateIfNotExistsAsync();

            State._deviceInfo = new ThingInfo()
            {
                DeviceId = Guid.NewGuid().ToString(),
                Region = region,
                Version = version
            };

            var Device = new DeviceEntity(State._deviceInfo.DeviceId, State._deviceInfo.Region)
            {
                Version = State._deviceInfo.Version.ToString()
            };
            TableOperation insertOperation = TableOperation.InsertOrReplace(Device);
            cloudTable.ExecuteAsync(insertOperation);

            // based on the info, assign a group... for demonstration we are assigning a random group
            State._deviceGroupId = region;

            var deviceGroup = ActorProxy.Create<IThingGroup>(new ActorId(region));
            return deviceGroup.RegisterDevice(State._deviceInfo);
        }
    }
}
