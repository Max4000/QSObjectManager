using Qlik.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Qlik.Sense.Client;
using Qlik.Sense.Client.Storytelling;
using UtilClasses.ProgramOptionsClasses;

// ReSharper disable IdentifierTypo


#pragma warning disable CS0618

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class QsStoryRestorer : IRestoreSlideInfoFromDisk, IProgramOptionsEvent
    {
        
        private readonly RestoreInfo _restoreInfo = new();
        private readonly RestoreStoryFromDiskInfo _restoreStoryFromDiskInfo = new();


        public event RestoreSlideInfoFromDiskHandler NewRestoreSlideInfoFromDiskSend;
        public event ProgramOptionsHandler NewProgramOptionsSend;

        public QsStoryRestorer(IRestoreInfoEvent restoreInfo , IRestoreStoryFromDisk storyFromDisk, IProgramOptionsEvent programOptions)
        {
            

            IRestoreInfoEvent restoreInfoEvent = restoreInfo;

            restoreInfoEvent.NewRestoreInfoSend += RestoreInfoReceived;

            IRestoreStoryFromDisk story = storyFromDisk;

            story.NewRestoreStoryFromDiskSend += RestoreStoryFromDiskReceived;

            IProgramOptionsEvent pOptions = programOptions;

            pOptions.NewProgramOptionsSend += ProgramOptionsReceived;

            var unused = new QsSlideRestorer(this,this);
        }

        private void OnNewProgramOptions(ProgramOptionsEventArgs e)
        {
            if (this.NewProgramOptionsSend != null)
                NewProgramOptionsSend(this, e);
        }


        private void ProgramOptionsReceived(object sender, ProgramOptionsEventArgs e)
        {
            OnNewProgramOptions(e);
        }

        private void OnNewRestoreSlideInfoFromDisk(RestoreSlideInfoEventArgs e)
        {
            if (NewRestoreSlideInfoFromDiskSend != null)
                NewRestoreSlideInfoFromDiskSend(this, e);
        }

        private void RestoreStoryFromDiskReceived(object sender, RestoreStoryFromDiskEventArgs e)
        {
            e.RestoreInfo.Copy( _restoreStoryFromDiskInfo);
            DoRestore();
        }

        

        private void DoRestore()
        {
            string searchFileStoryInStore = _restoreStoryFromDiskInfo.StoryFolder + "\\" +
                                            _restoreStoryFromDiskInfo.CurrentStory.Id + ".xml";

            _restoreStoryFromDiskInfo.TargetApp.DestroyGenericObject(_restoreStoryFromDiskInfo.CurrentStory.Id);
            

            MetaAttributesDef metaAttributes = new MetaAttributesDef();
            JsonTextReader rdMetaDef =
                Utils.MakeTextReader(_restoreStoryFromDiskInfo.StoryFolder + "\\Properties.MetaDef.json");
            metaAttributes.ReadJson(rdMetaDef);
            rdMetaDef.Close();

            StoryChildListDef childListDef = new StoryChildListDef();
            JsonTextReader rdchildListDef =
                Utils.MakeTextReader(_restoreStoryFromDiskInfo.StoryFolder + "\\Properties.ChildListDef.json");
            childListDef.ReadJson(rdchildListDef);
            rdchildListDef.Close();

            StaticContentUrlContainerDef thumbail = new StaticContentUrlContainerDef();
            JsonTextReader rdthumbail =
                Utils.MakeTextReader(_restoreStoryFromDiskInfo.StoryFolder + "\\Properties.Thumbnail.json");
            thumbail.ReadJson(rdthumbail);
            rdthumbail.Close();

            StoryProperties mStory = new StoryProperties();
            mStory.Set("qMetaDef", metaAttributes);
            mStory.Set("qChildListDef", childListDef);
            mStory.Set("rank", GetRank(searchFileStoryInStore));
            mStory.Set("thumbnail", thumbail);
            IStory currentStory =
                _restoreStoryFromDiskInfo.TargetApp.CreateStory(_restoreStoryFromDiskInfo.CurrentStory.Id, mStory);

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
                                        FullPathToSlideFolder =
                                            _restoreStoryFromDiskInfo.StoryFolder + "\\" + slideFolder,
                                        SlideFolder = slideFolder,
                                        Story = currentStory,

                                        CurrentSource = _restoreStoryFromDiskInfo.CurrentAppSource.Copy(),

                                        CurrentTarget = _restoreStoryFromDiskInfo.CurrentAppTarget.Copy(),
                                        AppContentFolder = _restoreStoryFromDiskInfo.AppContentFolder,
                                        DafaultContentFolder = _restoreStoryFromDiskInfo.DefaultFolder,
                                        
                                        TargetApp = _restoreStoryFromDiskInfo.TargetApp,
                                        SourceApp = _restoreStoryFromDiskInfo.SourceApp
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

        private void RestoreInfoReceived(object sender, RestoreInfoEventArgs e)
        {
            e.RestoreInfo.Copy(_restoreInfo);
        }


        
    }

    public class RestoreStoryFromDiskInfo
    {
        public IApp SourceApp;
        public IApp TargetApp;
        public NameAndIdAndLastReloadTime CurrentAppSource;
        public NameAndIdAndLastReloadTime CurrentAppTarget;
        public NameAndIdAndLastReloadTime CurrentStory;
        
        public string AppContentFolder;
        public string DefaultFolder;

        public string StoryFolder;
        
        public IList<string> AddContentList;
        public string FolderNameWithAddContent;
        

        public void Copy(RestoreStoryFromDiskInfo anotherInfo)
        {
            anotherInfo.SourceApp = SourceApp;
            anotherInfo.TargetApp = TargetApp;
            anotherInfo.AppContentFolder = AppContentFolder;
            anotherInfo.DefaultFolder = DefaultFolder;
            anotherInfo.CurrentAppSource = CurrentAppSource.Copy();
            anotherInfo.CurrentAppTarget = CurrentAppTarget.Copy();
            anotherInfo.CurrentStory = CurrentStory.Copy();
            anotherInfo.StoryFolder = StoryFolder;
            anotherInfo.AddContentList = AddContentList;
            anotherInfo.FolderNameWithAddContent = FolderNameWithAddContent;
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
        event RestoreStoryFromDiskHandler NewRestoreStoryFromDiskSend;
    }

    public delegate void RestoreStoryFromDiskHandler(object sender, RestoreStoryFromDiskEventArgs e);

}
