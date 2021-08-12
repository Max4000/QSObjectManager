using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Qlik.Engine;
using Qlik.Sense.Client;
using UtilClasses;
using Formatting = Newtonsoft.Json.Formatting;

#pragma warning disable 618

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class Utils
    {
        
        /// <summary>
        /// Возвращает
        /// </summary>
        /// <param name="location"></param>
        /// <returns>Список приложений</returns>
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
        /// 
        /// </summary>
        /// <param name="location"></param>
        /// <param name="appid"></param>
        /// <returns></returns>
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

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameOfElement"></param>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="xmlTextWriter"></param>
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
