using System;
using System.Collections.Generic;
using System.Xml;
using Newtonsoft.Json;
using Qlik.Sense.Client;
using Qlik.Sense.Client.Snapshot;
using Qlik.Sense.Client.Storytelling;
using UtilClasses.ProgramOptionsClasses;
#pragma warning disable 618


namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class QsStoryItemContainerRestorer
    {
        public ProgramOptions Options { get; } = new();
        private IConnect _location;

        private readonly RestoreInfo _restoreInfo = new();

        private readonly RestoreSlideInfo _restoreSlideInfo = new();

        private readonly Dictionary<string, XmlPair> _currentItemDict = new();

        public QsStoryItemContainerRestorer(IProgramOptionsEvent options, IConnectionStatusInfoEvent connection,
            IRestoreInfoEvent restore, IRestoreSlideInfoFromDisk slideInfoFromDisk)
        {
            IProgramOptionsEvent programOptions = options;
            programOptions.NewProgramOptionsSend += NewProgramOptionsReceived;

            IConnectionStatusInfoEvent connect = connection;
            connect.NewConnectionStatusInfoSend += NewConnectionStatusInfoReceived;

            IRestoreInfoEvent restoreInfo = restore;

            restoreInfo.NewRestoreInfoSend += NewRestoreInfoReceived;

            IRestoreSlideInfoFromDisk slideinfo = slideInfoFromDisk;

            slideinfo.NewRestoreSlideInfoFromDiskSend += NewRestoreSlideInfoFromDiskReceived;

        }

        private void NewRestoreSlideInfoFromDiskReceived(object sender, RestoreSlideInfoEventArgs e)
        {
            e.RestoreInfo.Copy(_restoreSlideInfo);
            DoRestore();
        }

        private void DoRestore()
        {

            IList<ItemPair> listItems = new List<ItemPair>();
            float rank = 0;
            
            ReadXmlFileOfSlide(ref rank,listItems);


            string childListDefFile = _restoreSlideInfo.FullPathToSlideFolder + "\\ISlide.Properties.ChildListDef.json";

            SlideChildListDef childListDef = JsonConvert.DeserializeObject<SlideChildListDef>(
                Utils.ReadJsonFile(childListDefFile));

            SlideProperties slideProperties = new SlideProperties();

            slideProperties.Set("qChildListDef", childListDef);
            slideProperties.Set("rank",rank);

            ISlide slide = _restoreSlideInfo.Story.CreateSlide(_restoreSlideInfo.SlideFolder, slideProperties);

            foreach (ItemPair slideItem in listItems)
            {
                ReadCurrentItemSlideXml(slideItem.Folder);

                string visualization = _currentItemDict["Visualization"].Value;

                switch (visualization)
                {
                    case "snapshot":
                    {
                        using SlideItemProperties itemProperties = CreateSnapshotSlideItemProperties(slide, _currentItemDict["id"].Value, slideItem.Folder);
                        
                        slide.CreateSnapshotSlideItem(_currentItemDict["id"].Value, itemProperties);

                        break;
                    }
                }



                DoRestoreItemSlide();
            }

        }

        private SlideItemProperties CreateSnapshotSlideItemProperties(ISlide slide, string id, string itemFolder)
        {


            string pathToStyle = _restoreSlideInfo.FullPathToSlideFolder + "\\" + itemFolder + "\\Style.json";

            SlideStyle style = JsonConvert.DeserializeObject<SlideStyle>(
                Utils.ReadJsonFile(pathToStyle));

            string ids = style.Get<string>("id");

            SlideItemProperties result = slide.CreateSnapshotSlideItemProperties(id, ids);

            string title = this._currentItemDict["Title"].Value;
            result.Set("title", title);

            
            string ratio = _currentItemDict["Ratio"].Value;
            result.Set("ratio",bool.Parse(ratio));

            
            string pathToPosition = _restoreSlideInfo.FullPathToSlideFolder + "\\" + itemFolder + "\\Position.json";
            SlidePosition  slideposition = JsonConvert.DeserializeObject<SlidePosition>(
                Utils.ReadJsonFile(pathToPosition));
            result.Set("position", slideposition);

            string dataPath = this._currentItemDict["DataPath"].Value;
            result.Set("dataPath", dataPath);

            string staticContentUrlContainerDeffile =
                _restoreSlideInfo.FullPathToSlideFolder + "\\" + itemFolder + "\\SrcPath.json";

            StaticContentUrlContainerDef contUrl = JsonConvert.DeserializeObject<StaticContentUrlContainerDef>(
                Utils.ReadJsonFile(staticContentUrlContainerDeffile));

            result.Set("srcPath",contUrl);

            string visual = this._currentItemDict["Visualization"].Value;
            result.Set("visualization", visual);

            string visualizationType = this._currentItemDict["VisualizationType"].Value;
            result.Set("visualizationType", visualizationType);

            result.Set("style", style);

            string sheetId = this._currentItemDict["SheetId"].Value;
            result.Set("sheetId", sheetId);


            string selectionStateFile =
                _restoreSlideInfo.FullPathToSlideFolder + "\\" + itemFolder + "\\SelectionState.json";
            SelectionState selectionState = JsonConvert.DeserializeObject<SelectionState>(
                Utils.ReadJsonFile(selectionStateFile));

            result.Set("selectionState", selectionState);

            string embeddedSnapshotDefile = _restoreSlideInfo.FullPathToSlideFolder + "\\" + itemFolder + "\\EmbeddedSnapshotDef.json";
            SnapshotProperties embeddedSnapshotDef = JsonConvert.DeserializeObject<SnapshotProperties>(
                Utils.ReadJsonFile(embeddedSnapshotDefile));

            result.Set("qEmbeddedSnapshotDef", embeddedSnapshotDef);
            return result;
        }

        private void ReadCurrentItemSlideXml(string itemFolder)
        {

            _currentItemDict.Clear();

            string pathToXml = _restoreSlideInfo.FullPathToSlideFolder + 
                 "\\" + itemFolder + "\\" + itemFolder + ".xml";

            var xmlDocument = new XmlDocument();

            xmlDocument.Load(pathToXml);

            XmlNode root = xmlDocument.DocumentElement;

            if (root != null)
                foreach (XmlNode nodeStory in root.ChildNodes)
                {
                    switch (nodeStory.Name)
                    {
                        case "Properties":
                        {
                            foreach (XmlNode proprety in nodeStory.ChildNodes)
                            {
                                if (proprety?.Attributes != null)
                                {
                                    string id = proprety.Attributes.GetNamedItem("id")?.Value;
                                    string type = proprety.Attributes.GetNamedItem("Type")?.Value;
                                    string name = proprety.Attributes.GetNamedItem("name")?.Value;

                                    _currentItemDict.Add(id ?? string.Empty, new XmlPair(type, name));
                                }
                            }

                            break;
                        }
                    }
                }
        }

        private void DoRestoreItemSlide()
        {

        }

        private void ReadXmlFileOfSlide(ref float rank, IList<ItemPair> listOfItem)
        {
            string searchFileSlideInStore =
                _restoreSlideInfo.FullPathToSlideFolder + "\\" + _restoreSlideInfo.SlideFolder + ".xml";

            var xmlDocument = new XmlDocument();

            xmlDocument.Load(searchFileSlideInStore);

            XmlNode root = xmlDocument.DocumentElement;


            if (root != null)
                foreach (XmlNode nodeStory in root.ChildNodes)
                {
                    switch (nodeStory.Name)
                    {
                        case "slide":
                            {
                                foreach (XmlNode nodeprop in nodeStory.ChildNodes)
                                {
                                    switch (nodeprop.Name)
                                    {
                                        case "Properties":
                                            {
                                                foreach (XmlNode node in nodeprop.ChildNodes)
                                                {
                                                    if (node.Attributes != null)
                                                    {
                                                        string id = node.Attributes.GetNamedItem("id")?.Value;

                                                        switch (id)
                                                        {
                                                            case "ISlide.Properties.Rank":
                                                                {
                                                                    rank = float.Parse(node.Attributes.GetNamedItem("name")?.Value ?? string.Empty);
                                                                    break;
                                                                }
                                                        }
                                                    }
                                                }

                                                break;
                                            }
                                        case "SlideItems":
                                            {
                                                foreach (XmlNode node in nodeprop.ChildNodes)
                                                {
                                                    if (node.Attributes != null)
                                                    {
                                                        string folder = node.Attributes.GetNamedItem("id")?.Value;
                                                        string id = node.Attributes.GetNamedItem("name")?.Value;

                                                        listOfItem.Add(new ItemPair(folder, id));
                                                    }
                                                }

                                                break;

                                            }
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

        private void NewConnectionStatusInfoReceived(object sender, ConnectionStatusInfoEventArgs e)
        {
            e.ConnectionStatusInfo.Copy(ref _location);
        }

        private void NewProgramOptionsReceived(object sender, ProgramOptionsEventArgs e)
        {
            e.ProgramOptions.Copy(this.Options);
        }
    }

    public class ItemPair
    {
        public string Folder;
        public string Id;

        public ItemPair(string folder, string id)
        {
            this.Folder = folder;
            this.Id = id;
        }
    }

    public class RestoreSlideInfo
    {
        public IStory Story;
        public string FullPathToSlideFolder;
        public string SlideFolder;

        public void Copy(RestoreSlideInfo anotherInfo)
        {
            anotherInfo.Story = Story;
            anotherInfo.FullPathToSlideFolder = FullPathToSlideFolder;
            anotherInfo.SlideFolder = SlideFolder;
        }
    }

    public class RestoreSlideInfoEventArgs : EventArgs
    {

        public readonly RestoreSlideInfo RestoreInfo;

        
        public RestoreSlideInfoEventArgs(RestoreSlideInfo record)
        {
            RestoreInfo = record;
        }
    }

    public interface IRestoreSlideInfoFromDisk
    {
        event NewRestoreSlideInfoFromDiskHandler NewRestoreSlideInfoFromDiskSend;
    }

    public delegate void NewRestoreSlideInfoFromDiskHandler(object sender, RestoreSlideInfoEventArgs e);


    public class XmlPair
    {
        public string Type;
        public string Value;

        public XmlPair(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}
