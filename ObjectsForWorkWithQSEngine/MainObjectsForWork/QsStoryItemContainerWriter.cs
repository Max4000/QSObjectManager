using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Qlik.Sense.Client.Storytelling;
using UtilClasses.ProgramOptionsClasses;

#pragma warning disable CS0618

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class QsStoryItemContainerWriter
    {
        public ProgramOptions Options { get; } = new();
        public StoryItemInfo ItemInfo = new();
        public DeleteItemFromDiskInfo  DeleteItem= new ();
        public QsStoryItemContainerWriter(IProgramOptionsEvent programOptionsEvent, IWriteStoryItemToDisk writeStoryItem,IDeleteInfoFromDisk deleteInfoFromDisk)
        {
            
            IProgramOptionsEvent programopt = programOptionsEvent;

            programopt.NewProgramOptionsSend += NewProgramOptionsReseive;

            IWriteStoryItemToDisk storyItem = writeStoryItem;

            storyItem.NewStoryItemToDiskSend += NewWriteStoryItemToDiskReseive;

            IDeleteInfoFromDisk deleteInfo = deleteInfoFromDisk;

            deleteInfo.NewDeleteItemFromDiskSend += NewDeleteItemFromDiskReseieved;

        }

        private void NewDeleteItemFromDiskReseieved(object sender, DeleteItemFromDiskEventArgs e)
        {
            e.DeleteInfo.Copy(DeleteItem);
            DoDelete();
        }

        public void DoDelete()
        {
            foreach (var file in Directory.GetFiles(DeleteItem.ItemFolder))
            {
                File.Delete(file);
            }
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

                    
                    Utils.PrintStructureToFile("item", "Data", "SlideObjectView", "Data.json", xmlWriter,
                        fileData, ItemInfo.Container.Data);
                    
                    Utils.PrintStructureToFile("item", "INxContainerEntry.Info", "NxInfo", "INxContainerEntry.Info.json", xmlWriter,
                        fileInfo, ItemInfo.Container.Info);

                    Utils.PrintStructureToFile("item", "INxContainerEntry.Meta", "NxMeta", "INxContainerEntry.Meta.json", xmlWriter,
                        fileMeta, ItemInfo.Container.Meta);

                //xmlWriter.WriteEndElement();


            xmlWriter.WriteEndElement();//item

            ISlide slide = ItemInfo.Story.GetSlide(ItemInfo.Id);

            xmlWriter.WriteStartElement("slide");

                xmlWriter.WriteStartElement("properties");
                    
                    Utils.CreateElement("item", "Rank", "float", slide.Layout.Rank.ToString(CultureInfo.InvariantCulture), xmlWriter);

                    string pathToFileNxInfo = itemPath + "\\" + "ISlide.NxInfo.json";
                    
                    Utils.PrintStructureToFile("item", "NxInfo", "NxInfo", "ISlide.NxInfo.json", xmlWriter,
                        pathToFileNxInfo, ItemInfo.Container.Info);

                    string pathToFileNxMeta = itemPath + "\\" + "ISlide.NxMeta.json";

                    Utils.PrintStructureToFile("item", "NxMeta", "NxMeta", "ISlide.NxMeta.json", xmlWriter,
                        pathToFileNxMeta, slide.Layout.Meta);

                    string pathToFileNxLayoutErrors = itemPath + "\\" + "ISlide.NxLayoutErrors.json";
                    
                    Utils.PrintStructureToFile("item", "NxLayoutErrors", "NxLayoutErrors", "ISlide.NxLayoutErrors.json", xmlWriter,
                        pathToFileNxLayoutErrors, slide.Layout.Error);

                    Utils.CreateElement("item", "NxSelectionInfo.InSelections", "bool", slide.Layout.SelectionInfo.InSelections.ToString(), xmlWriter);
                    
                    Utils.CreateElement("item", "NxSelectionInfo.MadeSelections", "bool", slide.Layout.SelectionInfo.MadeSelections.ToString(), xmlWriter);

                    string pathToFileLayout = itemPath + "\\" + "ISlide.Layout.json";
                    
                    Utils.PrintStructureToFile("item", "Layout", "SlideLayout", "ISlide.Layout.json", xmlWriter,
                        pathToFileLayout, slide.Layout);


                    string pathToFileProperties = itemPath + "\\" + "ISlide.Properties.json";

                    Utils.PrintStructureToFile("item", "slide.Properties.ChildListDef", "SlideChildListDef", "ISlide.Properties.json", xmlWriter,
                        pathToFileProperties, slide.Properties);


                xmlWriter.WriteEndElement();
            
            xmlWriter.WriteEndElement();


            xmlWriter.WriteEndDocument();

            xmlWriter.Flush();
            xmlWriter.Close();

        }

/*
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
*/

/*
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
*/

/*
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
*/
/*
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
*/


/*
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
*/
        
/*
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
*/

/*
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
*/

/*
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
*/

    }

    public class DeleteItemFromDiskInfo
    {
        public string ItemFolder;
        
        public void Copy(DeleteItemFromDiskInfo anotherInfo)
        {
            anotherInfo.ItemFolder = ItemFolder;
        }
    }

    public class DeleteItemFromDiskEventArgs : EventArgs
    {

        public readonly DeleteItemFromDiskInfo DeleteInfo;

        //Конструкторы
        public DeleteItemFromDiskEventArgs(DeleteItemFromDiskInfo record)
        {
            DeleteInfo = record;
        }
    }

    public interface IDeleteInfoFromDisk
    {
        event NewIDeleteInfoFromDisktHandler NewDeleteItemFromDiskSend;
    }

    public delegate void NewIDeleteInfoFromDisktHandler(object sender, DeleteItemFromDiskEventArgs e);

}
