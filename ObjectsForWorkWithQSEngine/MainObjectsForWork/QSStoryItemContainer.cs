using System.IO;
using System.Text;
using System.Xml;
using UtilClasses;
using UtilClasses.ProgramOptionsClasses;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class QsStoryItemContainer
    {
        public ProgramOptions Options { get; } = new();
        public StoryItemInfo ItemInfo = new();
        public QsStoryItemContainer(IProgramOptionsEvent programOptionsEvent, IWriteStoryItemToDisk writeStoryItem)
        {
            
            IProgramOptionsEvent programopt = programOptionsEvent;

            programopt.NewProgramOptionsSend += NewProgramOptionsReseive;

            IWriteStoryItemToDisk storyItem = writeStoryItem;

            storyItem.NewStoryItemToDiskSend += NewWriteStoryItemToDiskReseive;

        }

        private void NewWriteStoryItemToDiskReseive(object sender, StoryItemInfoEventArgs e)
        {
            e.ItemInfo.Copy(ref ItemInfo);
            WriteStoryItem();
        }

        private void NewProgramOptionsReseive(object sender, ProgramOptionsEventArgs e)
        {
            e.ProgramOptions.Copy(Options);
        }

        private void WriteStoryItem()
        {
            string itemPath = ItemInfo.LocalRootFolder + "\\" + ItemInfo.Id;
            
            string fileXml = itemPath + "\\" + ItemInfo.Id + ".xml";

            string fileData = itemPath + "\\" + "Data" + ".json";
            string fileInfo = itemPath + "\\" + "Info" + ".json";
            string fileMeta = itemPath + "\\" + "Meta" + ".json";

            Directory.CreateDirectory(itemPath);
            
            XmlTextWriter xmlWriter = new XmlTextWriter(fileXml, Encoding.UTF8)
            {
                Formatting = Formatting.Indented
            };

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteComment("Файл содержит описание элемента " + ItemInfo.Id);

            xmlWriter.WriteStartElement("item");

                xmlWriter.WriteStartElement("properties");

                    xmlWriter.WriteStartElement("item");

                        xmlWriter.WriteAttributeString("id", "Data");
                        xmlWriter.WriteAttributeString("Type", "SlideObjectView");
                        xmlWriter.WriteAttributeString("name", "Data.json");

                        WriteDataToFile(fileData);

                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("item");

                        xmlWriter.WriteAttributeString("id", "Info");
                        xmlWriter.WriteAttributeString("Type", "NxInfo");
                        xmlWriter.WriteAttributeString("name", "Info.json");

                        WriteInfoToFile(fileInfo);

                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("item");

                        xmlWriter.WriteAttributeString("id", "Meta");
                        xmlWriter.WriteAttributeString("Type", "NxMeta");
                        xmlWriter.WriteAttributeString("name", "Meta.json");

                        WriteMetaToFile(fileMeta);

                    xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
            
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndDocument();

            xmlWriter.Flush();
            xmlWriter.Close();

        }

        private void WriteMetaToFile(string fileData)
        {
            if (ItemInfo.Container != null)
            {
                if (ItemInfo != null && ItemInfo.Container.Data != null)
                {
                    string json = ItemInfo.Container.Meta.PrintStructure(Newtonsoft.Json.Formatting.Indented);

                    using var propertyFile = new AppEntryWriter(fileData);

                    propertyFile.Writer.Write(json);
                    propertyFile.Writer.Close();
                }

            }
        }

        private void WriteDataToFile(string fileData)
        {
            if (ItemInfo.Container != null)
            {
                if (ItemInfo != null && ItemInfo.Container.Data != null)
                {
                    string json = ItemInfo.Container.Data.PrintStructure(Newtonsoft.Json.Formatting.Indented);

                    using var propertyFile = new AppEntryWriter(fileData);

                    propertyFile.Writer.Write(json);
                    propertyFile.Writer.Close();
                }

            }
        }

        private void WriteInfoToFile(string fileData)
        {
            if (ItemInfo.Container != null)
            {
                if (ItemInfo != null && ItemInfo.Container.Data != null)
                {
                    string json = ItemInfo.Container.Info.PrintStructure(Newtonsoft.Json.Formatting.Indented);

                    using var propertyFile = new AppEntryWriter(fileData);

                    propertyFile.Writer.Write(json);
                    propertyFile.Writer.Close();
                }

            }
        }

    }
}
