﻿using OculusGraphQLApiLib.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculusDB.Database
{
    public class DBIAPItem
    {
        public DateTime __lastUpdated { get; set; } = DateTime.Now;
        public string __OculusDBType { get; set; } = DBDataTypes.IAPItem;
        public string latestAssetFileId { get; set; } = "";

        // IAPItem
        public AppStoreOffer current_offer { get; set; } = new AppStoreOffer();
        public string display_name { get; set; } = "";
        public string display_short_description { get; set; } = "";
        public string id { get; set; } = "";
        public ParentApplication parentApplication { get; set; } = new ParentApplication();
    }

    public class DBItemId
    {
        public string __OculusDBType { get; set; } = DBDataTypes.IAPItem;
        public string id { get; set; } = "";
    }
}
