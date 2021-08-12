using Qlik.Engine;
using System;
using System.Xml;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qlik.Sense.Client;
using Qlik.Sense.Client.Storytelling;

#pragma warning disable CS0618

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class QsStoryRestorer : IRestoreSlideInfoFromDisk
    {
        
        private readonly RestoreInfo _restoreInfo = new();
        private readonly RestoreStoryFromDiskInfo _restoreStoryFromDiskInfo = new();


        public event NewRestoreSlideInfoFromDiskHandler NewRestoreSlideInfoFromDiskSend;

        public QsStoryRestorer(IRestoreInfoEvent restoreInfo , IRestoreStoryFromDisk storyFromDisk)
        {
            

            IRestoreInfoEvent restoreInfoEvent = restoreInfo;

            restoreInfoEvent.NewRestoreInfoSend += NewRestoreInfoReceived;

            IRestoreStoryFromDisk story = storyFromDisk;

            story.NewRestoreStoryFromDiskSend += NewRestoreStoryFromDiskReceived;

            var unused = new QsSlideRestorer(this);
        }

        private void OnNewRestoreSlideInfoFromDisk(RestoreSlideInfoEventArgs e)
        {
            if (NewRestoreSlideInfoFromDiskSend != null)
                NewRestoreSlideInfoFromDiskSend(this, e);
        }

        private void NewRestoreStoryFromDiskReceived(object sender, RestoreStoryFromDiskEventArgs e)
        {
            e.RestoreInfo.Copy( _restoreStoryFromDiskInfo);
            DoRestore();
        }

        private void DoRestore()
        {

            string searchFileStoryInStore = _restoreStoryFromDiskInfo.StoryFolder + "\\" + _restoreStoryFromDiskInfo.CurrentStory.Id + ".xml";

            _restoreStoryFromDiskInfo.App.DestroyGenericObject(_restoreStoryFromDiskInfo.CurrentStory.Id);


            JObject jObject = JObject.Parse(Utils.ReadJsonFile(_restoreStoryFromDiskInfo.StoryFolder + "\\Properties.MetaDef.json"));

            JToken rootToken = jObject.Root;

            string title = "", description = "", annotation = "";

            if (rootToken != null)
            {
                foreach (JToken token in rootToken.Children())
                {
                    string path = token.Path;

                    switch (path)
                    {
                        case "title":
                        {
                            title = token.First.Value<string>();
                            break;
                        }
                        case "description":
                        {
                            description = token.First.Value<string>();
                            break;
                        }
                        case "annotation":
                        {
                            annotation = token.First.Value<string>();
                            break;
                        }
                    }

                }


            }

            MetaAttributesDef metaAttributes = new MetaAttributesDef()
            {
                Title = title,
                Annotation = annotation,
                Description = description
            };


            StoryChildListDef childListDef =
                JsonConvert.DeserializeObject<StoryChildListDef>(
                    Utils.ReadJsonFile(_restoreStoryFromDiskInfo.StoryFolder + "\\Properties.ChildListDef.json"));
            
            StaticContentUrlContainerDef thumbail =
                JsonConvert.DeserializeObject<StaticContentUrlContainerDef>(
                    Utils.ReadJsonFile(_restoreStoryFromDiskInfo.StoryFolder + "\\Properties.Thumbnail.json"));

            StoryProperties mStory = new StoryProperties();

            
            mStory.Set("qMetaDef", metaAttributes);
            
            mStory.Set("qChildListDef", childListDef);
            
            mStory.Set("rank", GetRank(searchFileStoryInStore));
            
            mStory.Set("thumbnail", thumbail);

            IStory currentStory = _restoreStoryFromDiskInfo.App.CreateStory(_restoreStoryFromDiskInfo.CurrentStory.Id, mStory);


            var xmlDocument = new XmlDocument();

            xmlDocument.Load(searchFileStoryInStore);

            XmlNode root = xmlDocument.DocumentElement;

            if (root != null)
                foreach (XmlNode nodeStory in root.ChildNodes)
                {
                    switch (nodeStory.Name)
                    {
                        case "Items":
                            {
                                foreach (XmlNode item in nodeStory.ChildNodes)
                                {
                                    if (item.Attributes != null)
                                    {
                                        string slideFolder = item.Attributes.GetNamedItem("id2")?.Value;

                                        RestoreSlideInfo restInfo = new RestoreSlideInfo()
                                        {
                                            FullPathToSlideFolder = _restoreStoryFromDiskInfo.StoryFolder + "\\" + slideFolder,
                                            SlideFolder = slideFolder,
                                            Story = currentStory
                                        };

                                        OnNewRestoreSlideInfoFromDisk(new RestoreSlideInfoEventArgs(restInfo));
                                    }
                                }

                                break;
                            }
                    }

                }

            
            
        }

        private float GetRank(string pathXml)
        {
            var xmlDocument = new XmlDocument();

            xmlDocument.Load(pathXml);

            XmlNode root = xmlDocument.DocumentElement;

            if (root != null)
                foreach (XmlNode nodeStory in root.ChildNodes)
                {
                    switch (nodeStory.Name)
                    {
                        case "properties":
                        {
                            foreach (XmlNode item in nodeStory.ChildNodes)
                            {
                                if (item.Attributes != null)
                                {
                                    string value = item.Attributes.GetNamedItem("id")?.Value;

                                    if (string.CompareOrdinal(value, "Properties.Rank") == 0)
                                        return float.Parse(Strings.Replace(item.Attributes.GetNamedItem("name")?.Value,".",",") ?? string.Empty);

                                }
                            }

                            break;
                        }
                    }

                }

            return -1.0f;


        }

        private void NewRestoreInfoReceived(object sender, RestoreInfoEventArgs e)
        {
            e.RestoreInfo.Copy(_restoreInfo);
        }


    }

    public class RestoreStoryFromDiskInfo
    {
        public IApp App;
        public NameAndIdPair CurrentApp;
        public NameAndIdPair CurrentStory;
        public string StoryFolder;
        

        public void Copy(RestoreStoryFromDiskInfo anotherInfo)
        {
            anotherInfo.App = App;
            anotherInfo.CurrentApp = CurrentApp.Copy();
            anotherInfo.CurrentStory = CurrentStory.Copy();
            anotherInfo.StoryFolder = StoryFolder;
        }

    }

    public class RestoreStoryFromDiskEventArgs : EventArgs
    {

        public readonly RestoreStoryFromDiskInfo RestoreInfo;

        
        public RestoreStoryFromDiskEventArgs(RestoreStoryFromDiskInfo record)
        {
            RestoreInfo = record;
        }
    }

    public interface IRestoreStoryFromDisk
    {
        event NewRestoreStoryFromDiskHandler NewRestoreStoryFromDiskSend;
    }

    public delegate void NewRestoreStoryFromDiskHandler(object sender, RestoreStoryFromDiskEventArgs e);

}
