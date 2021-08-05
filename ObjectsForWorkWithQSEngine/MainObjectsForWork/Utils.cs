using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Qlik.Engine;
using Qlik.Sense.Client;
using Qlik.Sense.Client.Storytelling;
using UtilClasses;
using Formatting = Newtonsoft.Json.Formatting;

#pragma warning disable 618

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class Utils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static IList<NameAndIdPair> GetApps(ILocation location)
        {
            IList<NameAndIdPair> arr = new List<NameAndIdPair>();

            foreach ( var app in location.GetAppIdentifiers())
            {
               arr.Add(new NameAndIdPair(app.AppName,app.AppId)); 
            }

            return arr;
        }

        /// <summary>
        /// Returns list of stories for full id application
        /// </summary>
        /// <param name="location">object location for Dev Hub</param>
        /// <param name="appid">full id app</param>
        /// <returns>list of stories</returns>
        public static IList<NameAndIdPair> GetStories(ILocation location, string appid)
        {
            IList<NameAndIdPair> lstResult = new List<NameAndIdPair>();

            IAppIdentifier appId = location.AppWithId(appid);
            
            using var app = location.App(appId);
            
            var listOfStories = app.GetStoryList();
            
            foreach (var item in listOfStories.Items)
            {


                var mStory = app?.GetStory(item.Info.Id);

                if (mStory != null)
                {
                    string name = mStory.Layout.Meta.Title;
                    lstResult.Add(new NameAndIdPair(name,item.Info.Id));
                }
            }

            return lstResult;
        }

        public static IStory GetStoryFromApp(ILocation location, string appid,string storeId)
        {
            
            IAppIdentifier appId = location.AppWithId(appid);
            
            using IApp app = location.App(appId);

            return app?.GetStory(storeId);

        }

        public static void CreateElement(string nameOfElement, string id, string type, string name, XmlTextWriter xmlTextWriter)
        {
            xmlTextWriter.WriteStartElement(nameOfElement);

            xmlTextWriter.WriteAttributeString("id", id);
            xmlTextWriter.WriteAttributeString("Type", type);
            xmlTextWriter.WriteAttributeString("name", name);


            xmlTextWriter.WriteEndElement();
        }


        public static void PrintStructureToFile(string nameOfElement, string id, string type, string name, XmlTextWriter xmlTextWriter, string fileName, IAbstractStructure abstractStructure)
        {
            CreateElement(nameOfElement, id, type, name, xmlTextWriter);

            if (abstractStructure != null)
            {
                string json = abstractStructure.PrintStructure(Formatting.Indented);

                if (fileName.EndsWith("Properties.MetaDef.json"))
                    json = json.Replace("\"tags\": [],", "", StringComparison.Ordinal);

                using var propertyFile = new AppEntryWriter(fileName);

                propertyFile.Writer.Write(json);
                propertyFile.Writer.Close();
            }
            else
            {
                using var propertyFile = new AppEntryWriter(fileName);

                propertyFile.Writer.Write("");
                propertyFile.Writer.Close();
            }
        }

        public static string ReadJsonFile(string file)
        {
            using Stream streamProperties = new FileStream(file, FileMode.Open);

            using var streamRead = new StreamReader(streamProperties, Encoding.UTF8);

            return streamRead.ReadToEnd();

        }
    }


}
