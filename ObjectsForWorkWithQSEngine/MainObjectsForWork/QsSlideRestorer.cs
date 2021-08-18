using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qlik.Engine;
using Qlik.Sense.Client.Snapshot;
using Qlik.Sense.Client.Storytelling;
// ReSharper disable IdentifierTypo

#pragma warning disable 618


namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class QsSlideRestorer
    {

        private readonly RestoreSlideInfo _restoreSlideInfo = new();

        private readonly Dictionary<string, XmlPair> _currentItemDict = new();

        public QsSlideRestorer(
             IRestoreSlideInfoFromDisk slideInfoFromDisk)
        {
            
            IRestoreSlideInfoFromDisk slideinfo = slideInfoFromDisk;
            slideinfo.NewRestoreSlideInfoFromDiskSend += RestoreSlideInfoFromDiskReceived;

        }

        private void RestoreSlideInfoFromDiskReceived(object sender, RestoreSlideInfoEventArgs e)
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
                    case "text":
                    {
                        using SlideItemProperties itemProperties = CreateSlideItemProperties(slide,
                            _currentItemDict["id"].Value, slideItem.Folder, "text");
                        
                        slide.CreateSlideItem(_currentItemDict["id"].Value, itemProperties);

                        break;
                    }
                    case "shape":
                    {
                        using SlideItemProperties itemProperties = CreateSlideItemProperties(slide,
                            _currentItemDict["id"].Value, slideItem.Folder, "shape");

                        slide.CreateSlideItem(_currentItemDict["id"].Value, itemProperties);

                        break;
                    }
                    case "sheet":
                    {
                        using SlideItemProperties itemProperties = CreateSlideItemProperties(slide,
                            _currentItemDict["id"].Value, slideItem.Folder, "sheet");

                        slide.CreateSlideItem(_currentItemDict["id"].Value, itemProperties);
                        
                        break;
                    }

                }



                DoRestoreItemSlide();
            }

        }

        private string GetPath(string fromString)
        {
            int pos1 = fromString.IndexOf('/', 0);
            int pos2 = fromString.IndexOf('/', pos1 + 1);
            int pos3 = fromString.IndexOf('/', pos2 + 1);
            return fromString.Substring(pos3 + 1);
        }

        private SlideItemProperties CreateSlideItemProperties(ISlide slide, string id, string itemFolder, string itemType)
        {
            SlideItemProperties result = null;

            SlideStyle style = JsonConvert.DeserializeObject<SlideStyle>(
                Utils.ReadJsonFile(_restoreSlideInfo.FullPathToSlideFolder + "\\" + itemFolder + "\\Style.json"));

            JObject jObject = null;
            try
            {

                jObject = JObject.Parse(Utils.ReadJsonFile(_restoreSlideInfo.FullPathToSlideFolder + "\\" +
                                                                   itemFolder + "\\SrcPath.json"));
            }
            catch (Exception)
            {
                // ignored
            }

            JProperty property = jObject?.Property("qStaticContentUrl");

            string valueSrcPath = "";
            string stringPathToImage = "";

            if (property != null)
            {
                JToken jtokenChild = property.Children().First();
                valueSrcPath = jtokenChild.Value<string>("qUrl");
            }


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
                    if (valueSrcPath.Length > 0)
                        stringPathToImage = GetPath(valueSrcPath);

                    result = slide.CreateImageSlideItemProperties(id, stringPathToImage);

                    result.SrcPath.StaticContentUrlDef.Set("qUrl", valueSrcPath);

                    break;
                }
                case "text":
                {
                    string textValue = style.Get<string>("text");
                    
                    string visualType = _currentItemDict["VisualizationType"].Value;

                    Slide.TextType textType = Slide.TextType.Title;
                    
                    switch (visualType)
                    {
                        case "title":
                        {
                            textType = Slide.TextType.Title;
                            break;
                        }
                        case "paragraph":
                        {
                            textType = Slide.TextType.Paragraph;
                            break;
                        }
                    }

                    result = slide.CreateTextSlideItemProperties(id, textType, textValue);

                    break;
                }
                case "shape":
                {
                    string visual = _currentItemDict["VisualizationType"].Value;

                    string colorValue = style.Get<string>("color");

                    Slide.Shapes visualType = ShapeType(visual);

                    result = slide.CreateShapeSlideItemProperties(id, visualType,colorValue);
                    
                    break;
                }
                case "sheet":
                {
                    if (id != null) 
                        result = slide.CreateTextSlideItemProperties(id);

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

        private Slide.Shapes ShapeType(string visual)
        {
           

            Slide.Shapes visualType = Slide.Shapes.Bus;

            switch (visual)
            {
                case "2lines_h":
                {
                    visualType = Slide.Shapes.Shapes2LinesH;
                    break;
                }
                case "2lines_v":
                {
                    visualType = Slide.Shapes.Shapes2LinesV;
                    break;
                }
                case "arrow_d":
                {
                    visualType = Slide.Shapes.ArrowD;
                    break;
                }
                case "arrow_l":
                {
                    visualType = Slide.Shapes.ArrowL;
                    break;
                }
                case "arrow_r":
                {
                    visualType = Slide.Shapes.ArrowR;
                    break;
                }
                case "arrow_u":
                {
                    visualType = Slide.Shapes.ArrowU;
                    break;
                }
                case "banned":
                {
                    visualType = Slide.Shapes.Banned;
                    break;
                }
                case "bus":
                {
                    visualType = Slide.Shapes.Bus;
                    break;
                }
                case "car":
                {
                    visualType = Slide.Shapes.Car;
                    break;
                }
                case "circle":
                {
                    visualType = Slide.Shapes.Circle;
                    break;
                }
                case "clock":
                {
                    visualType = Slide.Shapes.Clock;
                    break;
                }
                case "cloud":
                {
                    visualType = Slide.Shapes.Cloud;
                    break;
                }
                case "cross":
                {
                    visualType = Slide.Shapes.Cross;
                    break;
                }
                case "disc":
                {
                    visualType = Slide.Shapes.Disc;
                    break;
                }
                case "dollar":
                {
                    visualType = Slide.Shapes.Dollar;
                    break;
                }
                case "euro":
                {
                    visualType = Slide.Shapes.Euro;
                    break;
                }
                case "flag":
                {
                    visualType = Slide.Shapes.Flag;
                    break;
                }
                case "globe":
                {
                    visualType = Slide.Shapes.Globe;
                    break;
                }
                case "lightbulb":
                {
                    visualType = Slide.Shapes.Lightbulb;
                    break;
                }
                case "man":
                {
                    visualType = Slide.Shapes.Man;
                    break;
                }
                case "plane":
                {
                    visualType = Slide.Shapes.Plane;
                    break;
                }
                case "pound":
                {
                    visualType = Slide.Shapes.Pound;
                    break;
                }
                case "pullout_b":
                {
                    visualType = Slide.Shapes.PulloutB;
                    break;
                }
                case "pullout_l":
                {
                    visualType = Slide.Shapes.PulloutL;
                    break;
                }
                case "pullout_r":
                {
                    visualType = Slide.Shapes.PulloutR;
                    break;
                }
                case "pullout_t":
                {
                    visualType = Slide.Shapes.PulloutT;
                    break;
                }
                case "running_man":
                {
                    visualType = Slide.Shapes.RunningMan;
                    break;
                }
                case "square":
                {
                    visualType = Slide.Shapes.Square;
                    break;
                }
                case "square_rounded":
                {
                    visualType = Slide.Shapes.SquareRounded;
                    break;
                }
                case "star":
                {
                    visualType = Slide.Shapes.Star;
                    break;
                }
                case "tick":
                {
                    visualType = Slide.Shapes.Tick;
                    break;
                }
                case "train":
                {
                    visualType = Slide.Shapes.Train;
                    break;
                }
                case "tree":
                {
                    visualType = Slide.Shapes.Tree;
                    break;
                }
                case "tri1":
                {
                    visualType = Slide.Shapes.Tri1;
                    break;
                }
                case "tri2":
                {
                    visualType = Slide.Shapes.Tri2;
                    break;
                }
                case "tri3":
                {
                    visualType = Slide.Shapes.Tri3;
                    break;
                }
                case "tri4":
                {
                    visualType = Slide.Shapes.Tri4;
                    break;
                }
                case "woman":
                {
                    visualType = Slide.Shapes.Woman;
                    break;
                }
                case "yen":
                {
                    visualType = Slide.Shapes.Yen;
                    break;
                }
            }

            return visualType;
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
                                                                string value =  node.Attributes.GetNamedItem("name")?.Value ??
                                                                               string.Empty;
                                                                rank = Convert.ToSingle(value.Replace('.',','));
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
        event RestoreSlideInfoFromDiskHandler NewRestoreSlideInfoFromDiskSend;
    }

    public delegate void RestoreSlideInfoFromDiskHandler(object sender, RestoreSlideInfoEventArgs e);


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
