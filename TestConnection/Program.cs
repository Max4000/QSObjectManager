using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using MConnect.Connection;
using MConnect.Location;
using MyBookmark2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qlik.Engine;
using Qlik.Engine.Communication.Converters;
using Qlik.Sense.JsonRpc;
using UtilClasses;
using ISession = Qlik.Sense.JsonRpc.ISession;
using Session = Qlik.Engine.Session;

namespace TestConnection
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            DoWork();
        }

        private static void DoWork()
        {
            IConnect location =
                new MConnect.Location.RemoteConnection("http://localhost:4848/");
            location.Connect();

            string Id = IdForName(location, "Covid_Monitor_202006032256_AR23.qvf");

            IAppIdentifier appId = location.GetConnection().AppWithId(Id);

            MQlikConnection connect =
                new MQlikConnection(location.GetConnection(), Session.WithApp(appId));


            connect.App = location.GetConnection().App(appId);

            GenericBookmarkProperties prop = new GenericBookmarkProperties();

            prop.ReadJson(new JsonTextReader(new StreamReader(
                new FileStream(
                    @"D:\QS_STORE\Covid_Monitor_202006032256_AR23_20210919184752196\stories\c203046d-6026-4b75-96d3-ebbeef537a58\ZFepSG\Item0\Properties.json",
                    FileMode.Open), Encoding.UTF8)));

                //@"D:\QS_STORE\Covid_Monitor_202006032256_AR23_20210918033201036\stories\c203046d-6026-4b75-96d3-ebbeef537a58\ZFepSG\Item0\Properties.json", null);

            IEnumerable<string> cube = prop.GetAllProperties(true);
            IEnumerable<string> cube2 = prop.GetAllPropertyPaths(true);

            HyperCube cube3 = prop.Get<HyperCube>("qHyperCube");


            NxMetaDef def = prop.MetaDef;

           
            GenericBookmark snapshot = connect.App.GetGenericBookmark(prop.Info.Id);

            if (snapshot != null)
                connect.App.DestroyGenericBookmark(prop.Info.Id);

            var tk = connect.App.CreateGenericBookmark(prop);

            


        }

        

        private static string IdForName(IConnect location, string name)
        {

            foreach (NameAndIdPair elem in Utils.GetApps(location.GetConnection()))
            {
                if (String.CompareOrdinal(elem.Name, name) == 0)
                {
                    return elem.Id;
                }
            }

            return string.Empty;
        }
    }
}
