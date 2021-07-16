using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Qlik.Sense.Client.Storytelling;
using UtilClasses;
using UtilClasses.ProgramOptionsClasses;

#pragma warning disable CS0618

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
            string fileInfo = itemPath + "\\" + "INxContainerEntry.Info" + ".json";
            string fileMeta = itemPath + "\\" + "INxContainerEntry.Meta" + ".json";

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

                        xmlWriter.WriteAttributeString("id", "INxContainerEntry.Info");
                        xmlWriter.WriteAttributeString("Type", "NxInfo");
                        xmlWriter.WriteAttributeString("name", "INxContainerEntry.Info.json");

                        WriteInfoToFile(fileInfo);

                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("item");

                        xmlWriter.WriteAttributeString("id", "INxContainerEntry.Meta");
                        xmlWriter.WriteAttributeString("Type", "NxMeta");
                        xmlWriter.WriteAttributeString("name", "INxContainerEntry.Meta.json");

                        WriteMetaToFile(fileMeta);

                    xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();//item

            ISlide slide = ItemInfo.Story.GetSlide(ItemInfo.Id);

            xmlWriter.WriteStartElement("slide");

                xmlWriter.WriteStartElement("properties");

                    xmlWriter.WriteStartElement("item");

                        xmlWriter.WriteAttributeString("id", "Rank");
                        xmlWriter.WriteAttributeString("Type", "float");
                        xmlWriter.WriteAttributeString("name", slide.Layout.Rank.ToString(CultureInfo.InvariantCulture));

                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("item");

                        xmlWriter.WriteAttributeString("id", "NxInfo");
                        xmlWriter.WriteAttributeString("Type", "NxInfo");
                        xmlWriter.WriteAttributeString("name", "ISlide.NxInfo.json");

                        string pathToFileNxInfo = itemPath + "\\" + "ISlide.NxInfo.json" ;

                        WriteSlideNxInfoToFile(pathToFileNxInfo, slide);


                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("item");

                        xmlWriter.WriteAttributeString("id", "NxMeta");
                        xmlWriter.WriteAttributeString("Type", "NxMeta");
                        xmlWriter.WriteAttributeString("name", "ISlide.NxMeta.json");

                        string pathToFileNxMeta = itemPath + "\\" + "ISlide.NxMeta.json" ;

                        WriteSlideNxMetaToFile(pathToFileNxMeta, slide);


                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("item");

                        xmlWriter.WriteAttributeString("id", "NxLayoutErrors");
                        xmlWriter.WriteAttributeString("Type", "NxLayoutErrors");
                        xmlWriter.WriteAttributeString("name", "ISlide.NxLayoutErrors.json");

                        string pathToFileNxLayoutErrors = itemPath + "\\" + "ISlide.NxLayoutErrors.json" ;

                        WriteSlideNxLayoutErrorsToFile(pathToFileNxLayoutErrors, slide);


                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("item");

                        xmlWriter.WriteAttributeString("id", "NxSelectionInfo.InSelections");
                        xmlWriter.WriteAttributeString("Type", "bool");
                        xmlWriter.WriteAttributeString("name", slide.Layout.SelectionInfo.InSelections.ToString());


                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("item");

                        xmlWriter.WriteAttributeString("id", "NxSelectionInfo.MadeSelections");
                        xmlWriter.WriteAttributeString("Type", "bool");
                        xmlWriter.WriteAttributeString("name", slide.Layout.SelectionInfo.MadeSelections.ToString());


                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("item");

                        xmlWriter.WriteAttributeString("id", "Layout");
                        xmlWriter.WriteAttributeString("Type", "SlideLayout");
                        xmlWriter.WriteAttributeString("name", "ISlide.Layout.json");

                        string pathToFileLayout = itemPath + "\\" + "ISlide.Layout.json";

                        WriteSlideLayoutToFile(pathToFileLayout, slide);

                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("item");

                        xmlWriter.WriteAttributeString("id", "slide.Properties.ChildListDef");
                        xmlWriter.WriteAttributeString("Type", "SlideChildListDef");
                        xmlWriter.WriteAttributeString("name", "ISlide.Properties.json");

                        string pathToFileProperties = itemPath + "\\" + "ISlide.Properties.json";

                        WriteSlidePropertiesChildListDefToFile(pathToFileProperties, slide);

                    xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();
            
            xmlWriter.WriteEndElement();


            xmlWriter.WriteEndDocument();

            xmlWriter.Flush();
            xmlWriter.Close();

        }

        private void WriteSlidePropertiesChildListDefToFile(string fileData, ISlide slide)
        {
            if (slide.Properties != null)
            {

                string json = slide.Properties.PrintStructure(Newtonsoft.Json.Formatting.Indented);
                using var propertyFile = new AppEntryWriter(fileData);

                propertyFile.Writer.Write(json);
                propertyFile.Writer.Close();


            }
            else
            {
                using var propertyFile = new AppEntryWriter(fileData);

                propertyFile.Writer.Write("");
                propertyFile.Writer.Close();
            }
        }

        private void WriteSlideLayoutToFile(string fileData, ISlide slide)
        {
            if (slide.Layout != null)
            {

                string json = slide.Layout.PrintStructure(Newtonsoft.Json.Formatting.Indented);
                using var propertyFile = new AppEntryWriter(fileData);

                propertyFile.Writer.Write(json);
                propertyFile.Writer.Close();


            }
            else
            {
                using var propertyFile = new AppEntryWriter(fileData);

                propertyFile.Writer.Write("");
                propertyFile.Writer.Close();
            }
        }

        private void WriteSlideNxLayoutErrorsToFile(string fileData, ISlide slide)
        {
            if (slide.Layout.Error != null)
            {

                string json = slide.Layout.Error.PrintStructure(Newtonsoft.Json.Formatting.Indented);
                using var propertyFile = new AppEntryWriter(fileData);

                propertyFile.Writer.Write(json);
                propertyFile.Writer.Close();


            }
            else
            {
                using var propertyFile = new AppEntryWriter(fileData);

                propertyFile.Writer.Write("");
                propertyFile.Writer.Close();
            }
        }
        private void WriteSlideNxMetaToFile(string fileData, ISlide slide)
        {
            if (slide.Layout.Meta != null)
            {

                string json = slide.Layout.Meta.PrintStructure(Newtonsoft.Json.Formatting.Indented);
                using var propertyFile = new AppEntryWriter(fileData);

                propertyFile.Writer.Write(json);
                propertyFile.Writer.Close();


            }
        }


        private void WriteSlideNxInfoToFile(string fileData ,ISlide slide)
        {
            if (slide.Layout.Info != null)
            {
                
                string json = slide.Layout.Info.PrintStructure(Newtonsoft.Json.Formatting.Indented);
                using var propertyFile = new AppEntryWriter(fileData);

                propertyFile.Writer.Write(json);
                propertyFile.Writer.Close();
            

            }
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
