﻿using ComputerUtils.Logging;
using OculusDB.Database;
using OculusGraphQLApiLib;
using OculusGraphQLApiLib.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OculusDB
{
    public class OculusInteractor
    {
        public static void Init()
        {
            GraphQLClient.forcedLocale = "en_US";
            GraphQLClient.throwException = false;
            GraphQLClient.log = false;
        }
        public static List<Application> GetAllApplications(Headset headset, int appsToSkip, int appsToDo)
        {
            List<Application> allApps = new List<Application>();
            foreach(Application a in EnumerateAllApplications(headset))
            {
                allApps.Add(a);
            }
            return allApps;
        }

        public static long GetApplicationCount(Headset headset)
        {
            return GraphQLClient.AllApps(headset, null, 1).data.node.all_items.count;
        }

        public static IEnumerable<Application> EnumerateAllApplications(Headset headset)
        {
            Data<AppStoreAllAppsSection> s = GraphQLClient.AllApps(headset);
            int i = 0;
            if(s.data.node == null)
            {
                throw new Exception("Could not get data to enumerate applications.");
            }
            while (i < s.data.node.all_items.count)
            {
                string cursor = "";
                foreach (Node<Application> e in s.data.node.all_items.edges)
                {
                    cursor = e.cursor;
                    i++;
                    yield return e.node;
                }
                s = GraphQLClient.AllApps(headset, cursor);
            }
        }

        public static List<Application> GetAllApplicationsDetail(Headset headset)
        {
            List<Application> applications = new List<Application>();
            foreach (Application a in EnumerateAllApplicationsDetail(headset))
            {
                applications.Add(a);
            }
            return applications;
        }

        public static IEnumerable<Application> EnumerateAllApplicationsDetail(Headset headset)
        {
            foreach (Application a in EnumerateAllApplications(headset))
            {
                if (MongoDBInteractor.DoesIdExistInCurrentScrape(a.id))
                {
                    Logger.Log(a.displayName + "(" + a.id + ") exists in current scrape. Skipping");
                    continue;
                }
                Logger.Log("Getting application details for " + a.displayName);
                yield return GraphQLClient.GetAppDetail(a.id, headset).data.node;
            }
        }

        public static IEnumerable<IAPItem> EnumerateDLCs(string appId)
        {
            foreach(Node<AppItemBundle> item in GraphQLClient.GetDLCs(appId).data.node.latest_supported_binary.lastIapItems.edges)
            {
                if(item.node.IsIAPItem())
                {
                    yield return item.node;
                }
            }
        }

        public static IEnumerable<AppItemBundle> EnumerateDLCsAndPacks(string appId)
        {
            foreach (Node<AppItemBundle> item in GraphQLClient.GetDLCs(appId).data.node.latest_supported_binary.lastIapItems.edges)
            {
                Logger.Log(JsonSerializer.Serialize(item));
                yield return item.node;
            }
        }

        public static IEnumerable<AppItemBundle> EnumerateDLCPacks(string appId)
        {
            foreach (Node<AppItemBundle> item in GraphQLClient.GetDLCs(appId).data.node.latest_supported_binary.lastIapItems.edges)
            {
                if (item.node.IsAppItemBundle())
                {
                    yield return item.node;
                }
            }
        }
    }
}
