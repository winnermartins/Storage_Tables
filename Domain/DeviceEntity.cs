using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class DeviceEntity : TableEntity
    {
        public DeviceEntity(string id, string region)
        {
            PartitionKey = id;
            RowKey = region;
        }

        public string Version { get; set; }
    }
}
