using System;
using System.Collections.Generic;
using MyBookmark2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HyperCube = Qlik.Engine.HyperCube;
using NxMetaDef = Qlik.Engine.NxMetaDef;


namespace MyBookmark
{
    class Program
    {
        static void Main(string[] args)
        {

            MyGenericBookmarkProperties prop = new MyGenericBookmarkProperties(
                @"D:\QS_STORE\Covid_Monitor_202006032256_AR23_20210918033201036\stories\c203046d-6026-4b75-96d3-ebbeef537a58\ZFepSG\Item0\Properties.json", null);

            IEnumerable<string> cube = prop.GetAllProperties(true);
            IEnumerable<string> cube2 = prop.GetAllPropertyPaths(true);

            HyperCube cube3 = prop.Get<HyperCube>("qHyperCube");


            NxMetaDef def = prop.MetaDef;

            string st = JToken.FromObject(prop, new JsonSerializer()
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                NullValueHandling = NullValueHandling.Ignore
            }).ToString(Formatting.Indented);
        }

        
    }

    public class AbstractStructureJsonSerializer : JsonSerializer
    {
        
    }
}
