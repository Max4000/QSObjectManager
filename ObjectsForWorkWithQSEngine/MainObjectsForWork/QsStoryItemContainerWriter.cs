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

            programopt.NewProgramOptionsSend += NewProgramOptionsReceived;

            IWriteStoryItemToDisk storyItem = writeStoryItem;

            storyItem.NewStoryItemToDiskSend += NewWriteStoryItemToDiskReceived;

            IDeleteInfoFromDisk deleteInfo = deleteInfoFromDisk;

            deleteInfo.NewDeleteItemFromDiskSend += NewDeleteItemFromDiskReceived;

        }

        private void NewDeleteItemFromDiskReceived(object sender, DeleteItemFromDiskEventArgs e)
        {
            e.DeleteInfo.Copy(DeleteItem);
            DoDelete();
        }

        public void DoDelete()
        {
            var xmlDocument = new XmlDocument();

            xmlDocument.Load(DeleteItem.ItemFolder+"\\"+DeleteItem.ItemName+ ".xml");
            XmlNode root = xmlDocument.DocumentElement;

            if (root != null)
                foreach (XmlNode nodeApp in root.ChildNodes)
                {
                    switch (nodeApp.Name)
                    {

                            case "slide":
                            {

                                foreach (XmlNode node in nodeApp.ChildNodes)
                                {
                                    switch (node.Name)
                                    {

                                        case "SlideItems":
                                        {

                                            foreach (XmlNode nodeItem in node.ChildNodes)
                                            {
                                                if (nodeItem.Attributes != null)
                                                {
                                                    string folderItemSlide = nodeItem.Attributes.GetNamedItem("id")
                                                        ?.Value;

                                                    foreach (var file in Directory.GetFiles(DeleteItem.ItemFolder +
                                                        "\\" +
                                                        folderItemSlide))
                                                    {
                                                        File.Delete(file);
                                                    }

                                                    Directory.Delete(DeleteItem.ItemFolder + "\\" +
                                                                     folderItemSlide);
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
            
            foreach (var file in Directory.GetFiles(DeleteItem.ItemFolder))
            {
                File.Delete(file);
            }
        }

        private void NewWriteStoryItemToDiskReceived(object sender, StoryItemInfoEventArgs e)
        {
            e.ItemInfo.Copy(ref ItemInfo);
            WriteStoryItem();
        }

        private void NewProgramOptionsReceived(object sender, ProgramOptionsEventArgs e)
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

                xmlWriter.WriteStartElement("Properties");

                    #region Пока скрыто

                    //Utils.CreateElement("item", "Rank", "float", slide.Layout.Rank.ToString(CultureInfo.InvariantCulture), xmlWriter);

                    //string pathToFileNxInfo = itemPath + "\\" + "ISlide.NxInfo.json";

                    //Utils.PrintStructureToFile("item", "NxInfo", "NxInfo", "ISlide.NxInfo.json", xmlWriter,
                    //    pathToFileNxInfo, ItemInfo.Container.Info);

                    //string pathToFileNxMeta = itemPath + "\\" + "ISlide.NxMeta.json";

                    //Utils.PrintStructureToFile("item", "NxMeta", "NxMeta", "ISlide.NxMeta.json", xmlWriter,
                    //    pathToFileNxMeta, slide.Layout.Meta);

                    //string pathToFileNxLayoutErrors = itemPath + "\\" + "ISlide.NxLayoutErrors.json";

                    //Utils.PrintStructureToFile("item", "NxLayoutErrors", "NxLayoutErrors", "ISlide.NxLayoutErrors.json", xmlWriter,
                    //    pathToFileNxLayoutErrors, slide.Layout.Error);

                    //Utils.CreateElement("item", "NxSelectionInfo.InSelections", "bool", slide.Layout.SelectionInfo.InSelections.ToString(), xmlWriter);

                    //Utils.CreateElement("item", "NxSelectionInfo.MadeSelections", "bool", slide.Layout.SelectionInfo.MadeSelections.ToString(), xmlWriter);

                    //string pathToFileLayout = itemPath + "\\" + "ISlide.Layout.json";

                    //Utils.PrintStructureToFile("item", "Layout", "SlideLayout", "ISlide.Layout.json", xmlWriter,
                    //    pathToFileLayout, slide.Layout);


                    //string pathToFileProperties = itemPath + "\\" + "ISlide.Properties.json";

                    //Utils.PrintStructureToFile("item", "slide.Properties.ChildListDef", "SlideChildListDef", "ISlide.Properties.json", xmlWriter,
                    //    pathToFileProperties, slide.Properties);
                #endregion

                    string childListDef = itemPath + "\\" + "ISlide.Properties.ChildListDef.json";
                    
                    Utils.PrintStructureToFile("item", "ISlide.Properties.ChildListDef",
                            "SlideChildListDef", "ISlide.Properties.ChildListDef.json", xmlWriter,
                            childListDef, slide.Properties.ChildListDef);
                    
                    Utils.CreateElement("item", "ISlide.Properties.Rank", "float",
                        slide.Properties.Rank.ToString(CultureInfo.InvariantCulture), xmlWriter);

                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("SlideItems");

                    int itemNo = -1;


                    foreach (ISlideItem unused in slide.SlideItems)
                    {
                        itemNo++;

                        Utils.CreateElement("item", "Item" + itemNo.ToString(), "ISlideItem.SlideItemProperties",
                            unused.Id, xmlWriter);
                    }
                    
                    WriteSlideItemsToDisk(slide);

                xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();


            xmlWriter.WriteEndDocument();

            xmlWriter.Flush();
            xmlWriter.Close();

        }

        private void WriteSlideItemsToDisk(ISlide slide)
        {
            int itemNo = -1;

            string itemRootFolder = ItemInfo.LocalRootFolder + "\\" + ItemInfo.Id;
            
            foreach (ISlideItem slideItem in slide.SlideItems)
            {
                itemNo++;
                
                string itemPathFolder = itemRootFolder + "\\" + "Item" + itemNo.ToString();

                Directory.CreateDirectory(itemPathFolder);

                string fileXmlItem = itemPathFolder + "\\" + "Item" + itemNo.ToString() + ".xml";
                
                XmlTextWriter xmlWriterItem = new XmlTextWriter(fileXmlItem, Encoding.UTF8)
                {
                    Formatting = Formatting.Indented
                };

                xmlWriterItem.WriteStartDocument();
                xmlWriterItem.WriteComment("Файл содержит описание элемента слайда Item" + itemNo.ToString());
                
                
                
                xmlWriterItem.WriteStartElement("Item");
                
                    xmlWriterItem.WriteStartElement("Properties");

                        Utils.CreateElement("itemprop", "id", "string",
                        slideItem.Id, xmlWriterItem);

                        Utils.CreateElement("itemprop", "Title", "string",
                            slideItem.Properties.Title, xmlWriterItem);
                        
                        Utils.CreateElement("itemprop", "Ratio", "bool",
                            slideItem.Properties.Ratio.ToString(), xmlWriterItem);
                
                        string position = itemPathFolder + "\\" + "Position.json";

                        Utils.PrintStructureToFile("itemprop", "Position",
                            "SlidePosition", "Position.json", xmlWriterItem,
                            position, slideItem.Properties.Position);

                        Utils.CreateElement("itemprop", "DataPath", "string",
                            slideItem.Properties.DataPath, xmlWriterItem);

                        string srcPath = itemPathFolder + "\\" + "SrcPath.json";

                        Utils.PrintStructureToFile("itemprop", "SrcPath",
                            "StaticContentUrlContainerDef", "SrcPath.json", xmlWriterItem,
                            srcPath, slideItem.Properties.Position);
                        
                        Utils.CreateElement("itemprop", "Visualization", "string",
                            slideItem.Properties.Visualization, xmlWriterItem);

                        Utils.CreateElement("itemprop", "VisualizationType", "string",
                            slideItem.Properties.VisualizationType, xmlWriterItem);

                        string style = itemPathFolder + "\\" + "Style.json";

                        Utils.PrintStructureToFile("itemprop", "Style",
                            "SlideStyle", "Style.json", xmlWriterItem,
                            style, slideItem.Properties.Style);

                        Utils.CreateElement("itemprop", "SheetId", "string",
                            slideItem.Properties.SheetId, xmlWriterItem);

                        string selectionState = itemPathFolder + "\\" + "SelectionState.json";

                        Utils.PrintStructureToFile("itemprop", "SelectionState",
                            "SelectionState", "SelectionState.json", xmlWriterItem,
                            selectionState, slideItem.Properties.SelectionState);

                        string embeddedSnapshotDef = itemPathFolder + "\\" + "EmbeddedSnapshotDef.json";

                        Utils.PrintStructureToFile("itemprop", "EmbeddedSnapshotDef",
                            "SnapshotProperties", "EmbeddedSnapshotDef.json", xmlWriterItem,
                            embeddedSnapshotDef, slideItem.Properties.EmbeddedSnapshotDef);
                        
                
                    xmlWriterItem.WriteEndElement();
                
                xmlWriterItem.WriteEndElement();
                xmlWriterItem.WriteEndDocument();

                xmlWriterItem.Flush();
                xmlWriterItem.Close();
                
                xmlWriterItem.Dispose();
            }

        }


    }

    public class DeleteItemFromDiskInfo
    {
        public string ItemFolder;
        public string ItemName;
        
        public void Copy(DeleteItemFromDiskInfo anotherInfo)
        {
            anotherInfo.ItemFolder = ItemFolder;
            anotherInfo.ItemName = ItemName;
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
        event NewDeleteInfoFromDisktHandler NewDeleteItemFromDiskSend;
    }

    public delegate void NewDeleteInfoFromDisktHandler(object sender, DeleteItemFromDiskEventArgs e);

}
