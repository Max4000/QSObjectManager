using System;
using System.Collections.Generic;
using HyperCube = Qlik.Engine.HyperCube;
using NxMetaDef = Qlik.Engine.NxMetaDef;


namespace MyBookmark
{
    class Program
    {
        static void Main(string[] args)
        {

            MyGenericBookmarkProperties prop = new MyGenericBookmarkProperties(
                @"C:\Users\abaranovskiy\Documents\DOCSTORE\Covid_Monitor_202006032256_AR23_20210917140134344\stories\c203046d-6026-4b75-96d3-ebbeef537a58\ZFepSG\Item0\Properties.json");

            IEnumerable<string> cube = prop.GetAllProperties(true);
            IEnumerable<string> cube2 = prop.GetAllPropertyPaths(true);

            HyperCube cube3 = prop.Get<HyperCube>("qHyperCube");


            NxMetaDef def = prop.MetaDef;
        }
    }
}
