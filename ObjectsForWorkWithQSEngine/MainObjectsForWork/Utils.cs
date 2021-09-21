using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Qlik.Engine;
using Qlik.Sense.Client;
using UtilClasses;
using Formatting = Newtonsoft.Json.Formatting;
// ReSharper disable CommentTypo

#pragma warning disable 618

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class Utils
    {
        
        /// <summary>
        /// Возвращает список приложений находящихся на сервере
        /// или локальном компьютере
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
        /// Возвращает список историй у конкретного приложения
        /// на этом подключении к серверу или локальному компьютеру
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
        /// записывает один элемент данных
        /// в XML файл
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


        /// <summary>
        /// Записывает элемент данных в XML файл
        /// и сохраняет JSON объект в файле
        /// </summary>
        /// <param name="nameOfElement"></param>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="xmlTextWriter"></param>
        /// <param name="fileName"></param>
        /// <param name="abstractStructure"></param>
        public static void PrintStructureToFile(string nameOfElement, string id, string type, string name, XmlTextWriter xmlTextWriter, string fileName, IAbstractStructure abstractStructure)
        {
            CreateElement(nameOfElement, id, type, name, xmlTextWriter);

            string json = "";

            using (var propertyFile = new AppEntryWriter(fileName))
            {

                if (abstractStructure != null)
                {
                    json = abstractStructure.PrintStructure(Formatting.Indented);
                }

                propertyFile.Writer.Write(json);
                propertyFile.Writer.Close();
            }

        }

        public static string ReadJsonFile(string file)
        {
            
            using var streamRead = new StreamReader(new FileStream(file, FileMode.Open), Encoding.UTF8);

            return streamRead.ReadToEnd();

        }
    }


}
