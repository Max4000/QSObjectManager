using System.Text;
using System.Xml;
using Qlik.Engine;
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

        // ReSharper disable once UnusedMember.Local
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


            GenericBookmark snapshot = _snapshotWriteInfo.App.GetGenericBookmark(_snapshotWriteInfo.SlideItem.Style.Id);


            string itemPath = _snapshotWriteInfo.ItemFolder;

            XmlTextWriter xmlWriterItem = new XmlTextWriter(_snapshotWriteInfo.ItemFolder + "\\" + "Snapshot.xml", Encoding.UTF8)
            {
                Formatting = Formatting.Indented
            };

            xmlWriterItem.WriteStartDocument();
            xmlWriterItem.WriteComment("Файл содержит описание снапшота " + _snapshotWriteInfo.SlideItem.Style.Id);

            xmlWriterItem.WriteStartElement("Properties");

            

            Utils.PrintStructureToFile("item", "Properties.Info", "NxInfo", "Properties.Info.json", xmlWriterItem,
                itemPath + "\\" + "Properties.Info.json", snapshot.Properties.Info);

            Utils.PrintStructureToFile("item", "Properties.MetaDef", "MetaAttributesDef", "Properties.MetaDef.json", xmlWriterItem,
                itemPath + "\\" + "Properties.MetaDef.json", snapshot.Properties.MetaDef);

            Utils.PrintStructureToFile("item", "Properties", "GenericBookmarkProperties", "Properties.json", xmlWriterItem,
                itemPath + "\\" + "Properties.json", snapshot.Properties);





            xmlWriterItem.WriteEndElement();
            xmlWriterItem.WriteEndDocument();

            xmlWriterItem.Flush();
            xmlWriterItem.Close();

            xmlWriterItem.Dispose();

        }

        
    }

    
}
