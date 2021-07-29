using Qlik.Engine;
using System;
using System.Xml;
using Newtonsoft.Json;
using Qlik.Sense.Client;
using Qlik.Sense.Client.Storytelling;

#pragma warning disable CS0618

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class QsStoryRestorer : IRestoreSlideInfoFromDisk
    {
        
        private readonly RestoreInfo _restoreInfo = new();
        private readonly RestoreStoryFromDiskInfo _restoreStoryFromDiskInfo = new();

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private QsSlideRestorer QsSlideRestorer { get; } 

        public event NewRestoreSlideInfoFromDiskHandler NewRestoreSlideInfoFromDiskSend;

        public QsStoryRestorer(IRestoreInfoEvent restoreInfo , IRestoreStoryFromDisk storyFromDisk)
        {
            

            IRestoreInfoEvent restoreInfoEvent = restoreInfo;

            restoreInfoEvent.NewRestoreInfoSend += NewRestoreInfoReceived;

            IRestoreStoryFromDisk story = storyFromDisk;

            story.NewRestoreStoryFromDiskSend += NewRestoreStoryFromDiskReceived;

            QsSlideRestorer = new QsSlideRestorer(this);

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

            
            
            _restoreStoryFromDiskInfo.App.DestroyGenericObject(_restoreStoryFromDiskInfo.CurrentStory.Id);

            MetaAttributesDef metaAttributes =
                JsonConvert.DeserializeObject<MetaAttributesDef>(
                    Utils.ReadJsonFile(_restoreStoryFromDiskInfo.StoryFolder + "\\Properties.MetaDef.json"));


            StoryChildListDef childListDef =
                JsonConvert.DeserializeObject<StoryChildListDef>(
                    Utils.ReadJsonFile(_restoreStoryFromDiskInfo.StoryFolder + "\\Properties.ChildListDef.json"));
            
            StaticContentUrlContainerDef thumbail =
                JsonConvert.DeserializeObject<StaticContentUrlContainerDef>(
                    Utils.ReadJsonFile(_restoreStoryFromDiskInfo.StoryFolder + "\\Properties.Thumbnail.json"));

            StoryProperties mStory = new StoryProperties();

            
            mStory.Set("qMetaDef", metaAttributes);
            
            mStory.Set("qChildListDef", childListDef);
            
            mStory.Set("rank", (float) -1.0);
            
            mStory.Set("thumbnail", thumbail);

            IStory currentStory = _restoreStoryFromDiskInfo.App.CreateStory(_restoreStoryFromDiskInfo.CurrentStory.Id, mStory);

            #region Пока скрыто




            //ISlide slide = story1?.CreateSlide();

            //SlideItemProperties result = slide?.CreateSnapshotSlideItemProperties("Pxhfp", "40aba019 - 5c0b - 4d1c - 96e4 - a4c5c5dd79b3");

            //slide.CreateSnapshotSlideItem("Pxhfp", result);

            #endregion
            string searchFileStoryInStore = _restoreStoryFromDiskInfo.StoryFolder + "\\" + _restoreStoryFromDiskInfo.CurrentStory.Id + ".xml";

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
