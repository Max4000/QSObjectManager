using System;
using System.Collections.Generic;
using System.Xml;
using Newtonsoft.Json;
using Qlik.Engine;
using Qlik.Sense.Client;
using Qlik.Sense.Client.Snapshot;
using Qlik.Sense.Client.Storytelling;

#pragma warning disable 618


namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class QsStoryItemContainerRestorer
    {

        private readonly RestoreSlideInfo _restoreSlideInfo = new();

        private readonly Dictionary<string, XmlPair> _currentItemDict = new();

        public QsStoryItemContainerRestorer(
             IRestoreSlideInfoFromDisk slideInfoFromDisk)
        {
            
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
                        using SlideItemProperties itemProperties = CreateSlideItemProperties(slide,
                            _currentItemDict["id"].Value, slideItem.Folder, "snapshot");

                        slide.CreateSnapshotSlideItem(_currentItemDict["id"].Value, itemProperties);

                        

                        break;
                    }
                    case "image":
                    {
                        using SlideItemProperties itemProperties = CreateSlideItemProperties(slide,
                            _currentItemDict["id"].Value, slideItem.Folder, "image");

                        slide.CreateSlideItem(_currentItemDict["id"].Value, itemProperties) ;

                       

                        break;
                    }
                }



                DoRestoreItemSlide();
            }

        }

        private SlideItemProperties CreateSlideItemProperties(ISlide slide, string id, string itemFolder, string itemType)
        {
            SlideItemProperties result = null;
            
            SlideStyle style = JsonConvert.DeserializeObject<SlideStyle>(
                Utils.ReadJsonFile(_restoreSlideInfo.FullPathToSlideFolder + "\\" + itemFolder + "\\Style.json"));

            StaticContentUrlContainerDef contUrl = JsonConvert.DeserializeObject<StaticContentUrlContainerDef>(
                Utils.ReadJsonFile(_restoreSlideInfo.FullPathToSlideFolder + "\\" + itemFolder + "\\SrcPath.json"));

            StaticContentUrlDef url = contUrl?.Get<StaticContentUrlDef>("qStaticContentUrlDef");

            switch (itemType)
            {
                case "snapshot":
                {
                    string ids = style.Get<string>("id");

                    result = slide.CreateSnapshotSlideItemProperties(id, ids);

                    break;
                }
                case "image":
                {

                    string stringPathToImage = url?.Get<string>("qUrl");

                    result = slide.CreateImageSlideItemProperties(id, stringPathToImage);

                    result.Set("srcPath", contUrl);

                    break;
                }
            }

            
            
            if (result != null)
            {

                #region GenericObjectProperties

                    NxInfo nxInfo = JsonConvert.DeserializeObject<NxInfo>(
                        Utils.ReadJsonFile(_restoreSlideInfo.FullPathToSlideFolder + "\\" + itemFolder + "\\Info.json"));
                    
                    result.Set("qInfo",nxInfo);

                    result.Set("qExtendsId",_currentItemDict["ExtendsId"].Value);

                    NxMetaDef nxMetaDef = JsonConvert.DeserializeObject<NxMetaDef>(
                        Utils.ReadJsonFile(_restoreSlideInfo.FullPathToSlideFolder + "\\" + itemFolder + "\\MetaDef.json"));

                    result.Set("qMetaDef",nxMetaDef);

                    result.Set("qStateName", _currentItemDict["StateName"].Value);

                #endregion

                #region SlideItemProperties

                    result.Set("title", _currentItemDict["Title"].Value);

                    string ratio = _currentItemDict["Ratio"].Value;
                    result.Set("ratio", bool.Parse(ratio));

                    SlidePosition slideposition = JsonConvert.DeserializeObject<SlidePosition>(
                        Utils.ReadJsonFile(_restoreSlideInfo.FullPathToSlideFolder + "\\" + itemFolder + "\\Position.json"));
                    
                    result.Set("position", slideposition);

                    string dataPath = this._currentItemDict["DataPath"].Value;
                    result.Set("dataPath", dataPath);


                    

                    result.Set("visualization", _currentItemDict["Visualization"].Value);

                    result.Set("visualizationType", _currentItemDict["VisualizationType"].Value);

                    result.Set("style", style);

                    result.Set("sheetId", _currentItemDict["SheetId"].Value);


                    SelectionState selectionState = JsonConvert.DeserializeObject<SelectionState>(
                        Utils.ReadJsonFile(_restoreSlideInfo.FullPathToSlideFolder + "\\" + itemFolder + "\\SelectionState.json"));

                    result.Set("selectionState", selectionState);

                    
                    SnapshotProperties embeddedSnapshotDef = JsonConvert.DeserializeObject<SnapshotProperties>(
                        Utils.ReadJsonFile(_restoreSlideInfo.FullPathToSlideFolder + "\\" + itemFolder +
                                           "\\EmbeddedSnapshotDef.json"));

                    result.Set("qEmbeddedSnapshotDef", embeddedSnapshotDef);

                #endregion
                return result;
            }

            return null;
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
