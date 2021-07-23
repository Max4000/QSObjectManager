using Qlik.Engine;
using System;
using System.Xml;
using Newtonsoft.Json;
using Qlik.Sense.Client;
using Qlik.Sense.Client.Storytelling;
using UtilClasses.ProgramOptionsClasses;

#pragma warning disable CS0618

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class QsStoryRestorer :IProgramOptionsEvent, IConnectionStatusInfoEvent, IRestoreInfoEvent, IRestoreSlideInfoFromDisk
    {
        public ProgramOptions Options { get; } = new();
        private IConnect _location;

        private readonly RestoreInfo _restoreInfo = new();
        private readonly RestoreStoryFromDiskInfo _restoreStoryFromDiskInfo = new();

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private QsStoryItemContainerRestorer QsStoryItemContainerRestorer { get; } 



        public event NewProgramOptionsHandler NewProgramOptionsSend;
        public event NewRestoreInfoHandler NewRestoreInfoSend;
        public event ConnectionStatusInfoHandler NewConnectionStatusInfoSend;
        public event NewRestoreSlideInfoFromDiskHandler NewRestoreSlideInfoFromDiskSend;

        public QsStoryRestorer(IProgramOptionsEvent programOptions,
            IConnectionStatusInfoEvent connectionStatusInfoEvent, IRestoreInfoEvent restoreInfo , IRestoreStoryFromDisk storyFromDisk)
        {
            IProgramOptionsEvent programOptionsEvent = programOptions;

            programOptionsEvent.NewProgramOptionsSend += NewProgramOptionsReceived;

            IConnectionStatusInfoEvent connectionStatusInfo = connectionStatusInfoEvent;

            connectionStatusInfo.NewConnectionStatusInfoSend += NewConnectionStatusInfoReceived;

            IRestoreInfoEvent restoreInfoEvent = restoreInfo;

            restoreInfoEvent.NewRestoreInfoSend += NewRestoreInfoReceived;

            IRestoreStoryFromDisk story = storyFromDisk;

            story.NewRestoreStoryFromDiskSend += NewRestoreStoryFromDiskReceived;

            QsStoryItemContainerRestorer = new QsStoryItemContainerRestorer(this, this, this,this);

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

            IAppIdentifier appId = _location.GetConnection().AppWithId(_restoreStoryFromDiskInfo.CurrentApp.Id);

            var app = _location.GetConnection().App(appId);
            
            app?.DestroyGenericObject(_restoreStoryFromDiskInfo.CurrentStory.Id);

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

            IStory story1 = app?.CreateStory(_restoreStoryFromDiskInfo.CurrentStory.Id, mStory);

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
                                            Story = story1
                                        };

                                        OnNewRestoreSlideInfoFromDisk(new RestoreSlideInfoEventArgs(restInfo));
                                    }
                                }

                                break;
                            }
                    }

                }

            app?.SaveAs("Rtx");
            
        }

        private void NewRestoreInfoReceived(object sender, RestoreInfoEventArgs e)
        {
            e.RestoreInfo.Copy(_restoreInfo);

            OnNewRestoreInfoReceived(e);
        }

        private void OnNewRestoreInfoReceived(RestoreInfoEventArgs e)
        {
            if (NewRestoreInfoSend != null)
                NewRestoreInfoSend(this, e);
        }


        private void NewConnectionStatusInfoReceived(object sender, ConnectionStatusInfoEventArgs e)
        {
            e.ConnectionStatusInfo.Copy(ref _location);
            OnNewConnectionStatusInfo(e);
        }

        public void OnNewConnectionStatusInfo(ConnectionStatusInfoEventArgs e)
        {
            if (NewConnectionStatusInfoSend != null)
                NewConnectionStatusInfoSend(this, e);
        }

        private void NewProgramOptionsReceived(object sender, ProgramOptionsEventArgs e)
        {
            e.ProgramOptions.Copy(Options);
            OnNewOptions(e);
        }

        public void OnNewOptions(ProgramOptionsEventArgs e)
        {
            if (NewProgramOptionsSend != null)
                NewProgramOptionsSend(this, e);
        }
    }

    public class RestoreStoryFromDiskInfo
    {
        public NameAndIdPair CurrentApp;
        public NameAndIdPair CurrentStory;
        public string StoryFolder;
        

        public void Copy(RestoreStoryFromDiskInfo anotherInfo)
        {
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
