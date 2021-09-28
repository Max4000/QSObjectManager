using System.Xml;
using Microsoft.VisualBasic;
using Qlik.Sense.Client;
using Qlik.Sense.Client.Storytelling;
using UtilClasses.ProgramOptionsClasses;

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
            if (NewProgramOptionsSend != null)
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

            ReaderAbstractStructureClass.ReadObjectFromJsonFile(metaAttributes,
                _restoreStoryFromDiskInfo.StoryFolder + "\\Properties.MetaDef.json");
            
            StoryChildListDef childListDef = new StoryChildListDef();
            
            ReaderAbstractStructureClass.ReadObjectFromJsonFile(childListDef,
                _restoreStoryFromDiskInfo.StoryFolder + "\\Properties.ChildListDef.json");
            
            StaticContentUrlContainerDef thumbail = new StaticContentUrlContainerDef();

            ReaderAbstractStructureClass.ReadObjectFromJsonFile(thumbail,
                _restoreStoryFromDiskInfo.StoryFolder + "\\Properties.Thumbnail.json");


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
                                        
                                        FolderForAddContent = _restoreStoryFromDiskInfo.FolderNameWithAddContent,
                                        AddListContent = _restoreStoryFromDiskInfo.AddContentList
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
                                        return float.Parse(Strings.Replace(item.Attributes.GetNamedItem("name")?.Value,
                                            ".", ",") ?? string.Empty);

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

   
  

}
