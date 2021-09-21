using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MyBookmark2;
using Newtonsoft.Json;
using Qlik.Engine;
using Qlik.Engine.Communication;
using Qlik.Sense.Client;
using Qlik.Sense.Client.Snapshot;
using Qlik.Sense.Client.Storytelling;
using UtilClasses;
using CharToIntConverter = ObjectsForWorkWithQSEngine.Converters.CharToIntConverter;
using QixEnumConverter = ObjectsForWorkWithQSEngine.Converters.QixEnumConverter;

// ReSharper disable All

#pragma warning disable 618

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class CsAppSnapshotsRestorer
    {
        
        private SnapshotWriteInfo _restoreInfo = new();
        private Dictionary<string, XmlPair> _dictionary;

        private JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Newtonsoft.Json.Formatting.Indented,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            Converters = (IList<JsonConverter>)new JsonConverter[2]
            {
                (JsonConverter) new QixEnumConverter(),
                (JsonConverter) new CharToIntConverter()
            }
        };

        public CsAppSnapshotsRestorer( IRestoreSnapshotsFromDisk readSnapshotsFromDisk)
        {
            IRestoreSnapshotsFromDisk readSnapshotsEvent = readSnapshotsFromDisk;

            readSnapshotsEvent.NewSnapshotFromDiskSend += SnapshotFromDiskReceived; 
        }

        private void SnapshotFromDiskReceived(object sender, SnapshotWriteInfoEventArgs e)
        {
            e.ItemInfo.Copy(ref _restoreInfo);

            DoRestore();
        }

        private void DoRestore()
        {

            return;

            _dictionary = ReadSnapshotXml(_restoreInfo.ItemFolder);
            
            SlideStyle style = JsonConvert.DeserializeObject<SlideStyle>(
                Utils.ReadJsonFile(_restoreInfo.ItemFolder + "\\Style.json"));

            string snapshotLibId = style.Get<string>("id");
            

            ISnapshot snapshot = _restoreInfo.App.GetSnapshot(snapshotLibId);

            if (snapshot != null)
                _restoreInfo.App.RemoveSnapshot(snapshotLibId);

            CreateSnapshot(snapshotLibId);

            
            

        }
        private SnapshotProperties restoreSnapshotProperties(string snapshotId, string itemFolder)
        {

            Dictionary<string, XmlPair> snsDictionary = ReadSnapshotXml(itemFolder);

            SnapshotProperties result = new SnapshotProperties();

            NxInfo info = JsonConvert.DeserializeObject<NxInfo>(
                Utils.ReadJsonFile(itemFolder + "\\Properties.Info.json"));

            result.Set("qInfo", info);

            MetaAttributesDef meta = JsonConvert.DeserializeObject<MetaAttributesDef>(
                Utils.ReadJsonFile(itemFolder + "\\Properties.MetaDef.json"));

            result.Set("qMetaDef", meta);

            result.Set("title", snsDictionary["Properties.Title"].Value);

            result.Set("sheetId", snsDictionary["Properties.SheetId"].Value);

            SnapshotDataDef snsData = new SnapshotDataDef();

            SnapshotObjectDef obj = JsonConvert.DeserializeObject<SnapshotObjectDef>(
                Utils.ReadJsonFile(itemFolder + "\\Properties.SnapshotData.Object.json"));

            SnapshotContentDef content = JsonConvert.DeserializeObject<SnapshotContentDef>(
                Utils.ReadJsonFile(itemFolder + "\\Properties.SnapshotData.Content.json"));

            SnapshotObjectSizeDef parent = JsonConvert.DeserializeObject<SnapshotObjectSizeDef>(
                Utils.ReadJsonFile(itemFolder + "\\Properties.SnapshotData.Parent.json"));

            snsData.Set("object", obj);
            snsData.Set("content", content);
            snsData.Set("parent", parent);

            return result;
        }
        // ReSharper disable once IdentifierTypo
        private string GetvisualizationType(GenericObject obj)
        {
            string str = obj.GetType().Name.ToLowerInvariant();
            if (str.Equals("textimage"))
                str = "text-image";
            return str;
        }

        

        private  void SetGeneralSnapshotProperties(
            IApp theApp,
            string snapshotLibId,
            string sheetId,
            string objectId,
            SnapshotProperties snapshotProps)
        {
            GenericObject genericObject = theApp.GetObject<GenericObject>(objectId);
            snapshotProps.SheetId = sheetId;
            snapshotProps.SourceObjectId = objectId;

            MetaAttributesDef metaDef = JsonConvert.DeserializeObject<MetaAttributesDef>(
                Utils.ReadJsonFile(_restoreInfo.ItemFolder + "\\Properties.MetaDef.json"), serializerSettings);

            snapshotProps.MetaDef = new MetaAttributesDef()
            {
                Title = metaDef.Get<string>("title"),
                Description = metaDef.Get<string>("description"),
                Annotation = metaDef.Get<string>("annotation")
            };
            snapshotProps.Info = new NxInfo()
            {
                Id = snapshotLibId,
                Type = "snapshot"
            };
            snapshotProps.VisualizationType = GetvisualizationType(genericObject);
            snapshotProps.Timestamp = Convert.ToSingle(_dictionary["Properties.Timestamp"].Value);
            snapshotProps.IsClone = Convert.ToBoolean(_dictionary["Properties.IsClone"].Value);
        }

        private void SetSnapshotParentSize(
            IApp theApp,
            string sheetId,
            ISnapshotProperties snapshotProps)
        {

            SnapshotObjectSizeDef sizeDef = JsonConvert.DeserializeObject<SnapshotObjectSizeDef>(
                Utils.ReadJsonFile(_restoreInfo.ItemFolder + "\\Properties.SnapshotData.Parent.json"), serializerSettings);

            //snapshotProps.SnapshotData ??= new SnapshotDataDef();

            snapshotProps.SnapshotData.Parent.H = sizeDef.Get<float>("h");
            snapshotProps.SnapshotData.Parent.W = sizeDef.Get<float>("w"); ;
        }

        private void SetSnapshotObjectSize(
            IApp theApp,
            string sheetId,
            string objectId,
            SnapshotProperties snapshotProps)
        {
            SnapshotObjectDef objDef = JsonConvert.DeserializeObject<SnapshotObjectDef>(
                Utils.ReadJsonFile(_restoreInfo.ItemFolder + "\\Properties.SnapshotData.Object.json"), serializerSettings);

            SnapshotObjectSizeDef sizeDef = objDef.Size;

            //snapshotProps.SnapshotData ??= new SnapshotDataDef();

            snapshotProps.SnapshotData.Object.Size.H = sizeDef.H;
            snapshotProps.SnapshotData.Object.Size.W = sizeDef.W;
        }

        private void SetSnapshotContentSize(
            IApp theApp,
            string sheetId,
            string objectId,
            SnapshotProperties snapshotProps)
        {
            //snapshotProps.SnapshotData ??= new SnapshotDataDef();

            SnapshotContentDef content = JsonConvert.DeserializeObject<SnapshotContentDef>(
                Utils.ReadJsonFile(_restoreInfo.ItemFolder + "\\Properties.SnapshotData.Content.json"), serializerSettings);

            SnapshotObjectSizeDef size = content.Size;
            

            snapshotProps.SnapshotData.Content.Size.H = size.H;
            snapshotProps.SnapshotData.Content.Size.W = size.W;

            SnapshotChartDataDef chartData = content.ChartData;

            if (chartData.LegendScrollOffset != 0f)
                snapshotProps.SnapshotData.Content.ChartData.LegendScrollOffset = chartData.LegendScrollOffset ;
            
            if (chartData.ScrollOffset != 0f)
                snapshotProps.SnapshotData.Content.ChartData.ScrollOffset = chartData.ScrollOffset;
            
            if (chartData.DiscreteSpacing != 0f)
                snapshotProps.SnapshotData.Content.ChartData.DiscreteSpacing = chartData.DiscreteSpacing;

            if (chartData.AxisInnerOffset != 0f)
                snapshotProps.SnapshotData.Content.ChartData.AxisInnerOffset = chartData.AxisInnerOffset;

            if (chartData.HasMiniChart)
                snapshotProps.SnapshotData.Content.ChartData.HasMiniChart = chartData.HasMiniChart;

            if (chartData.XAxisMin != 0f)
                snapshotProps.SnapshotData.Content.ChartData.XAxisMin = chartData.XAxisMin;

            if (chartData.XAxisMax != 0f)
                snapshotProps.SnapshotData.Content.ChartData.XAxisMax = chartData.XAxisMax;

            if (chartData.YAxisMin != 0f)
                snapshotProps.SnapshotData.Content.ChartData.YAxisMin = chartData.YAxisMin;

            if (chartData.YAxisMax != 0f)
                snapshotProps.SnapshotData.Content.ChartData.YAxisMax = chartData.YAxisMax;

            if (chartData.MapRect != null)
            {
                
                snapshotProps.SnapshotData.Content.ChartData.MapRect.Height = chartData.MapRect.Height;
                snapshotProps.SnapshotData.Content.ChartData.MapRect.Width = chartData.MapRect.Width;
                snapshotProps.SnapshotData.Content.ChartData.MapRect.X = chartData.MapRect.X;
                snapshotProps.SnapshotData.Content.ChartData.MapRect.Y = chartData.MapRect.Y;
            }

            if (chartData.Rotation != 0f)
                snapshotProps.SnapshotData.Content.ChartData.Rotation = chartData.Rotation;

            IHyperCube bm = snapshotProps.Get<HyperCube>("qHyperCube");


        }


        private SnapshotProperties SetHyperCub(
            IApp theApp,
            string snapshotLibId,
            string sheetId,
            string objectId,
            SnapshotProperties snapshotProps)
        {
            
            HyperCube hyperCube = HyperCubeReader.ReadCube(
                _restoreInfo.ItemFolder + "\\ISnapshot.Properties.qHyperCube.json");

            snapshotProps.Set("qHyperCube", hyperCube);

            return snapshotProps;
        }

        private SnapshotProperties SetSnapshotProperties(
            IApp theApp,
            string snapshotLibId,
            string sheetId,
            string objectId,
            SnapshotProperties snapshotProps)
        {
            SetGeneralSnapshotProperties(theApp,snapshotLibId, sheetId, objectId, snapshotProps);
            SetSnapshotParentSize(theApp,sheetId, snapshotProps);
            SetSnapshotObjectSize(theApp,sheetId, objectId, snapshotProps);
            SetSnapshotContentSize(theApp,sheetId, objectId, snapshotProps);
            //SetHyperCub(theApp, snapshotLibId, sheetId, objectId, snapshotProps);
            return snapshotProps;
        }

        private SnapshotProperties SetScrollableChartsSnapshotProperties(
            string snapshotLibId,
            string sheetId,
            string objectId,
            SnapshotProperties snapshotProperties)
        {
            SnapshotProperties snapshotProperties1 = SetSnapshotProperties(_restoreInfo.App,snapshotLibId, sheetId, objectId, snapshotProperties);
            snapshotProperties1.SnapshotData.Content.ChartData.MapRect = (SnapshotMapRectDef)null;
            snapshotProperties1.SnapshotData.Content.ChartData.HasMiniChart = true;
            snapshotProperties1.SnapshotData.Content.ChartData.LegendScrollOffset = 0.0f;
            snapshotProperties1.SnapshotData.Content.ChartData.ScrollOffset = 0.0f;
            snapshotProperties1.SnapshotData.Content.ChartData.DiscreteSpacing = 0.0f;
            snapshotProperties1.SnapshotData.Content.ChartData.AxisInnerOffset = 0.0f;
            return snapshotProperties1;
        }

        private CreateGenericBookmarkResult CreateSnapshot(string snapshotLibId )
        {
            GenericBookmarkProperties structure = new GenericBookmarkProperties();

            structure.ReadJson(new JsonTextReader(new StreamReader(
                new FileStream(_restoreInfo.ItemFolder + "\\Properties.json", FileMode.Open), Encoding.UTF8)));

            string shid = structure.Get<string>("sheetId");
            
            var sn = _restoreInfo.App.CreateGenericBookmark(structure);

            //_restoreInfo.App.SaveObjects();

            //foreach (var info in _restoreInfo.App.GetAllInfos())
            //{
            //    if (string.CompareOrdinal(info.Id, shid) == 0)
            //    { 
            //        IGenericObject sh =  _restoreInfo.App.GetGenericObject(info.Id);

            //        sh.CreateChild<GenericObject>(_restoreInfo.App.GetGenericBookmark(sn.Info.Id).Properties
            //            .CloneAs<GenericObjectProperties>());
            //    }
            //}

            

            return sn;
        }


        private Dictionary<string, XmlPair> ReadSnapshotXml(string itemFolder)
        {
            Dictionary<string, XmlPair> itemDictResult = new();


            string pathToXml = itemFolder + "\\" + "Snapshot.xml";

            var xmlDocument = new XmlDocument();

            xmlDocument.Load(pathToXml);

            XmlNode root = xmlDocument.DocumentElement;

            if (root != null)
                
                foreach (XmlNode proprety in root.ChildNodes)
                {
                    if (proprety?.Attributes != null)
                    {
                        string id = proprety.Attributes.GetNamedItem("id")?.Value;
                        string type = proprety.Attributes.GetNamedItem("Type")?.Value;
                        string name = proprety.Attributes.GetNamedItem("name")?.Value;

                        itemDictResult.Add(id ?? string.Empty, new XmlPair(type, name));
                    }
                }


            return itemDictResult;
        }



    }
}
