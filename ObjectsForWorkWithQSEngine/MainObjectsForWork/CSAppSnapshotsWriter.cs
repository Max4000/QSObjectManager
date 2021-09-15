using System.Globalization;
using System.Text;
using System.Xml;
using Qlik.Engine;
using Qlik.Sense.Client;
using Qlik.Sense.Client.Snapshot;
using Qlik.Sense.Client.Visualizations.Components;
using Formatting = System.Xml.Formatting;

#pragma warning disable 618

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class CsAppSnapshotsWriter
    {
        
        private SnapshotWriteInfo _snapshotWriteInfo = new();

        

        public CsAppSnapshotsWriter(IWriteSnapshotToDisk writeSnapshotToDisk)
        {
            
            IWriteSnapshotToDisk writeSnapshotEvent = writeSnapshotToDisk;
            writeSnapshotEvent.NewSnapshotToDiskSend += SnapshotToDiskReceived;
        }

        private void SnapshotToDiskReceived(object sender, SnapshotWriteInfoEventArgs e)
        {
            e.ItemInfo.Copy(ref _snapshotWriteInfo);
            DoWrite();
        }

        private void WriteHyperCubeToDisk(HyperCube cube)
        {
            string itemPath = _snapshotWriteInfo.ItemFolder;

            XmlTextWriter xmlWriterItem = new XmlTextWriter(_snapshotWriteInfo.ItemFolder + "\\" + "HyperCube.xml", Encoding.UTF8)
            {
                Formatting = Formatting.Indented
            };

            xmlWriterItem.WriteStartDocument();
            
            xmlWriterItem.WriteComment("Файл содержит описание гиперкуба снапшота " + _snapshotWriteInfo.SlideItem.Style.Id);

            xmlWriterItem.WriteStartElement("Properties");

            Utils.CreateElement("item", "HyperCube.StateName", "string", cube.StateName, xmlWriterItem);
            Utils.CreateElement("item", "HyperCube.Size.cx", "int", cube.Size.cx.ToString(), xmlWriterItem);
            Utils.CreateElement("item", "HyperCube.Size.cy", "int", cube.Size.cy.ToString(), xmlWriterItem);
            
            Utils.PrintStructureToFile("item", "HyperCube.Error", "NxValidationError", "HyperCube.Error.json", xmlWriterItem,
                itemPath + "\\" + "HyperCube.Error.json", cube.Error);

            //Utils.PrintStructureToFile("item", "HyperCube.Error", "NxValidationError", "HyperCube.Error.json", xmlWriterItem,
            //    itemPath + "\\" + "HyperCube.Error.json", cube.Mode);

            xmlWriterItem.WriteEndElement();
            xmlWriterItem.WriteEndDocument();

            xmlWriterItem.Flush();
            xmlWriterItem.Close();

            xmlWriterItem.Dispose();

        }

        private void DoWrite()
        {

            ISnapshot snapshot = _snapshotWriteInfo.App.GetSnapshot(_snapshotWriteInfo.SlideItem.Style.Id);

            string itemPath = _snapshotWriteInfo.ItemFolder;

            XmlTextWriter xmlWriterItem = new XmlTextWriter(_snapshotWriteInfo.ItemFolder + "\\" + "Snapshot.xml", Encoding.UTF8)
            {
                Formatting = Formatting.Indented
            };

            xmlWriterItem.WriteStartDocument();
            xmlWriterItem.WriteComment("Файл содержит описание снапшота " + _snapshotWriteInfo.SlideItem.Style.Id);

            xmlWriterItem.WriteStartElement("Properties");

            #region Пишем в файл  XML

            Utils.PrintStructureToFile("item", "Properties.Info", "NxInfo", "Properties.Info.json", xmlWriterItem,
                itemPath + "\\" + "Properties.Info.json", snapshot.Properties.Info);

            Utils.PrintStructureToFile("item", "Properties.MetaDef", "MetaAttributesDef", "Properties.MetaDef.json", xmlWriterItem,
                itemPath + "\\" + "Properties.MetaDef.json", snapshot.Properties.MetaDef);


            Utils.CreateElement("item", "Properties.Title", "string", snapshot.Properties.Title, xmlWriterItem);

            Utils.CreateElement("item", "Properties.SheetId", "string", snapshot.Properties.SheetId, xmlWriterItem);

            Utils.PrintStructureToFile("item", "Properties.SnapshotData.Object", "SnapshotObjectDef", "Properties.SnapshotData.Object.json", xmlWriterItem,
                itemPath + "\\" + "Properties.SnapshotData.Object.json", snapshot.Properties.SnapshotData.Object);

            Utils.PrintStructureToFile("item", "Properties.SnapshotData.Content", "SnapshotContentDef", "Properties.SnapshotData.Content.json", xmlWriterItem,
                itemPath + "\\" + "Properties.SnapshotData.Content.json", snapshot.Properties.SnapshotData.Content);

            Utils.PrintStructureToFile("item", "Properties.SnapshotData.Parent", "SnapshotObjectSizeDef", "Properties.SnapshotData.Parent.json", xmlWriterItem,
                itemPath + "\\" + "Properties.SnapshotData.Parent.json", snapshot.Properties.SnapshotData.Parent);

            Utils.CreateElement("item", "Properties.SnapshotData.IsZoomed", "bool", snapshot.Properties.SnapshotData.IsZoomed.ToString(), xmlWriterItem);

            Utils.CreateElement("item", "Properties.SnapshotData.ElementRatio", "float", snapshot.Properties.SnapshotData.ElementRatio.ToString(CultureInfo.InvariantCulture), xmlWriterItem);

            Utils.CreateElement("item", "Properties.SnapshotData.ScrollTopPercent", "float", snapshot.Properties.SnapshotData.ScrollTopPercent.ToString(CultureInfo.InvariantCulture), xmlWriterItem);

            Utils.CreateElement("item", "Properties.SnapshotData.VerticalScrollWidth", "float", snapshot.Properties.SnapshotData.VerticalScrollWidth.ToString(CultureInfo.InvariantCulture), xmlWriterItem);

            Utils.CreateElement("item", "Properties.VisualizationType", "string", snapshot.Properties.VisualizationType, xmlWriterItem);

            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            Utils.CreateElement("item", "Properties.Timestamp", "float", snapshot.Properties.Timestamp.ToString(), xmlWriterItem);

            Utils.CreateElement("item", "Properties.SourceObjectId", "string", snapshot.Properties.SourceObjectId, xmlWriterItem);

            Utils.CreateElement("item", "Properties.IsClone", "bool", snapshot.Properties.IsClone.ToString(), xmlWriterItem);

            Utils.PrintStructureToFile("item", "ISnapshot.Properties", "SnapshotProperties", "ISnapshot.Properties.json", xmlWriterItem,
                itemPath + "\\" + "ISnapshot.Properties.json", snapshot.Properties);


            NxMeta meta = snapshot.Properties.Get<NxMeta>("qMeta");

            Utils.PrintStructureToFile("item", "ISnapshot.Properties.qMeta", "SnapshotProperties", "ISnapshot.Properties.qMeta.json", xmlWriterItem,
                itemPath + "\\" + "ISnapshot.Properties.qMeta.json", meta);
            
            string creationDate = snapshot.Properties.Get<string>("creationDate");

            Utils.CreateElement("item", "Properties.creationDate", "string", creationDate, xmlWriterItem);

            string creationDate2 = snapshot.Properties.Get<string>("creationDate");

            Utils.CreateElement("item", "Properties.creationDate2", "string", creationDate2, xmlWriterItem);

            bool showTitles = snapshot.Properties.Get<bool>("showTitles");

            Utils.CreateElement("item", "Properties.showTitles", "bool", showTitles.ToString(), xmlWriterItem);

            string subtitle = snapshot.Properties.Get<string>("subtitle");

            Utils.CreateElement("item", "Properties.subtitle", "string", subtitle, xmlWriterItem);

            string footnote = snapshot.Properties.Get<string>("footnote");

            Utils.CreateElement("item", "Properties.footnote", "string", footnote, xmlWriterItem);

            bool showDetails = snapshot.Properties.Get<bool>("showDetails");

            Utils.CreateElement("item", "Properties.showDetails", "bool", showDetails.ToString(), xmlWriterItem);

            string orientation = snapshot.Properties.Get<string>("orientation");

            Utils.CreateElement("item", "Properties.orientation", "string", orientation, xmlWriterItem);

            int scrollStartPos = snapshot.Properties.Get<int>("scrollStartPos");

            Utils.CreateElement("item", "Properties.scrollStartPos", "scrollStartPos", scrollStartPos.ToString(), xmlWriterItem);

            string nullMode = snapshot.Properties.Get<string>("nullMode");

            Utils.CreateElement("item", "Properties.nullMode", "string", nullMode, xmlWriterItem);

            CombochartDataPoint point = snapshot.Properties.Get<CombochartDataPoint>("dataPoint");

            Utils.PrintStructureToFile("item", "ISnapshot.Properties.point", "CombochartDataPoint", "ISnapshot.Properties.point.json", xmlWriterItem,
                itemPath + "\\" + "ISnapshot.Properties.point.json", point);

            ColorMode color = snapshot.Properties.Get<ColorMode>("color");

            Utils.PrintStructureToFile("item", "ISnapshot.Properties.color", "ColorMode", "ISnapshot.Properties.color.json", xmlWriterItem,
                itemPath + "\\" + "ISnapshot.Properties.color.json", color);

            Legend legend = snapshot.Properties.Get<Legend>("legend");

            Utils.PrintStructureToFile("item", "ISnapshot.Properties.legend", "Legend", "ISnapshot.Properties.legend.json", xmlWriterItem,
                itemPath + "\\" + "ISnapshot.Properties.legend.json", legend);

            HyperCube hyperCube = snapshot.Properties.Get<HyperCube>("qHyperCube");

            if (hyperCube != null)
            {
                Utils.PrintStructureToFile("item", "ISnapshot.Properties.qHyperCube", "HyperCube",
                    "ISnapshot.Properties.qHyperCube.json", xmlWriterItem,
                    itemPath + "\\" + "ISnapshot.Properties.qHyperCube.json", hyperCube);
            }

            #endregion
            xmlWriterItem.WriteEndElement();
            xmlWriterItem.WriteEndDocument();

            xmlWriterItem.Flush();
            xmlWriterItem.Close();

            xmlWriterItem.Dispose();

        }

        
    }

    
}
