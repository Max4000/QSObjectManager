using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Qlik.Sense.Client;
using Qlik.Sense.Client.Snapshot;
using Qlik.Sense.Client.Storytelling;
using UtilClasses.ProgramOptionsClasses;

// ReSharper disable StringLiteralTypo
// ReSharper disable CommentTypo

#pragma warning disable CS0618

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class QsSlideWriter
    {
        //public ProgramOptions Options { get; } = new();
        
        private StoryItemInfo _itemInfo = new();
        
        private readonly DeleteItemFromDiskInfo  _deleteItem = new ();

        private readonly ProgramOptions _options  = new();

        public QsSlideWriter(IWriteStoryItemToDisk writeStoryItem,IDeleteInfoFromDisk deleteInfoFromDisk , IProgramOptionsEvent programOptions)
        {
            
            IWriteStoryItemToDisk storyItem = writeStoryItem;

            storyItem.NewStoryItemToDiskSend += WriteStoryItemToDiskReceived;

            IDeleteInfoFromDisk deleteInfo = deleteInfoFromDisk;

            deleteInfo.NewDeleteItemFromDiskSend += DeleteItemFromDiskReceived;

            IProgramOptionsEvent prgOptions = programOptions;

            prgOptions.NewProgramOptionsSend += PrgOptionsOnNewProgramOptionsSend;


        }

        private void PrgOptionsOnNewProgramOptionsSend(object sender, ProgramOptionsEventArgs e)
        {
            e.ProgramOptions.Copy(_options);
        }

        private void DeleteItemFromDiskReceived(object sender, DeleteItemFromDiskEventArgs e)
        {
            e.DeleteInfo.Copy(_deleteItem);
            DoDelete();
        }

        public void DoDelete()
        {
            var xmlDocument = new XmlDocument();

            xmlDocument.Load(_deleteItem.ItemFolder+"\\"+_deleteItem.ItemName+ ".xml");
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

                                                    foreach (var file in Directory.GetFiles(_deleteItem.ItemFolder +
                                                        "\\" +
                                                        folderItemSlide))
                                                    {
                                                        File.Delete(file);
                                                    }

                                                    Directory.Delete(_deleteItem.ItemFolder + "\\" +
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
            
            foreach (var file in Directory.GetFiles(_deleteItem.ItemFolder))
            {
                File.Delete(file);
            }
        }

        private void WriteStoryItemToDiskReceived(object sender, StoryItemInfoEventArgs e)
        {
            e.ItemInfo.Copy(ref _itemInfo);
            WriteStoryItem();
        }

        private void WriteStoryItem()
        {
            string itemPath = _itemInfo.LocalRootFolder + "\\" + _itemInfo.Id;
            
            string fileXml = itemPath + "\\" + _itemInfo.Id + ".xml";

            string fileData = itemPath + "\\" + "Data" + ".json";
            string fileInfo = itemPath + "\\" + "INxContainerEntry.Info" + ".json";
            string fileMeta = itemPath + "\\" + "INxContainerEntry.Meta" + ".json";

            Directory.CreateDirectory(itemPath);
            
            XmlTextWriter xmlWriter = new XmlTextWriter(fileXml, Encoding.UTF8)
            {
                Formatting = Formatting.Indented
            };

            xmlWriter.WriteStartDocument();
            
            xmlWriter.WriteComment("Файл содержит описание элемента " + _itemInfo.Id);

            xmlWriter.WriteStartElement("item");

                xmlWriter.WriteStartElement("properties");

                    
                    Utils.PrintStructureToFile("item", "Data", "SlideObjectView", "Data.json", xmlWriter,
                        fileData, _itemInfo.Container.Data);
                    
                    Utils.PrintStructureToFile("item", "INxContainerEntry.Info", "NxInfo", "INxContainerEntry.Info.json", xmlWriter,
                        fileInfo, _itemInfo.Container.Info);

                    Utils.PrintStructureToFile("item", "INxContainerEntry.Meta", "NxMeta", "INxContainerEntry.Meta.json", xmlWriter,
                        fileMeta, _itemInfo.Container.Meta);

               


            xmlWriter.WriteEndElement();//item

            ISlide slide = _itemInfo.Story.GetSlide(_itemInfo.Id);

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


                    foreach (ISlideItem slideItem in slide.SlideItems)
                    {
                        itemNo++;

                        Utils.CreateElement("item", "Item" + itemNo.ToString(), "ISlideItem.SlideItemProperties",
                            slideItem.Id, xmlWriter);
                    }
                    
                    WriteSlideItemsToDisk(slide);

                xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();


            xmlWriter.WriteEndDocument();

            xmlWriter.Flush();
            xmlWriter.Close();

        }

        private void  CopyImageToRepAppContentFolder(ISlideItem item)
        {
            if (item.Layout.SrcPath.StaticContentUrl.Url.ToLower().Contains("/appcontent/"))
            {
                string file = Utils.GetNameImage(item.Layout.SrcPath.StaticContentUrl.Url);

                string source = _options.AppContentPath + "\\AppContent" + "\\" + _itemInfo.CurrentApp.Id + "\\" + file;

                string destination = _itemInfo.AppContentFolder + "\\" + file;

                if (!File.Exists(destination))
                    File.Copy(source,destination);
            }

            if (item.Layout.SrcPath.StaticContentUrl.Url.ToLower().Contains("/content/default/"))
            {
                string file = Utils.GetNameImage(item.Layout.SrcPath.StaticContentUrl.Url);

                string source = _options.ContentDefault + "\\" + file;

                string destination = _itemInfo.DefaultContnetFolder + "\\" + file;


                if (!File.Exists(destination))
                    File.Copy(source, destination);
            }


        }

        private void WriteSlideItemsToDisk(ISlide slide)
        {
            int itemNo = -1;

            string itemRootFolder = _itemInfo.LocalRootFolder + "\\" + _itemInfo.Id;
            
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

                        #region GenericObjectProperties

                            Utils.PrintStructureToFile("itemprop", "Info",
                                "NxInfo", "Info.json", xmlWriterItem,
                                itemPathFolder + "\\" + "Info.json", slideItem.Properties.Info);

                            Utils.CreateElement("itemprop", "ExtendsId", "string",
                                slideItem.Properties.ExtendsId, xmlWriterItem);


                            Utils.PrintStructureToFile("itemprop", "MetaDef",
                                "NxMetaDef", "MetaDef.json", xmlWriterItem,
                                itemPathFolder + "\\" + "MetaDef.json", slideItem.Properties.MetaDef);

                            Utils.CreateElement("itemprop", "StateName", "string",
                                slideItem.Properties.StateName, xmlWriterItem);

                        #endregion

                        Utils.CreateElement("itemprop", "id", "string",
                        slideItem.Id, xmlWriterItem);

                        #region SlideItemProperties

                            Utils.CreateElement("itemprop", "Title", "string",
                                slideItem.Layout.Title, xmlWriterItem);
                            
                            Utils.CreateElement("itemprop", "Ratio", "bool",
                                slideItem.Layout.Ratio.ToString(), xmlWriterItem);
                            
                            Utils.PrintStructureToFile("itemprop", "Position",
                                "SlidePosition", "Position.json", xmlWriterItem,
                                itemPathFolder + "\\" + "Position.json", slideItem.Layout.Position);

                            Utils.CreateElement("itemprop", "DataPath", "string",
                                slideItem.Layout.DataPath, xmlWriterItem);

                            Utils.PrintStructureToFile("itemprop", "SrcPath",
                                "StaticContentUrlContainerDef", "SrcPath.json", xmlWriterItem,
                                //itemPathFolder + "\\" + "SrcPath.json", slideItem.Properties.SrcPath);
                                itemPathFolder + "\\" + "SrcPath.json", slideItem.Layout.SrcPath);

                            Utils.CreateElement("itemprop", "Visualization", "string",
                                slideItem.Layout.Visualization, xmlWriterItem);




                            Utils.CreateElement("itemprop", "VisualizationType", "string",
                                slideItem.Layout.VisualizationType, xmlWriterItem);

                            string visType = slideItem.Layout.Get<string>("visualization");

                            if (string.CompareOrdinal(visType, "image") == 0)
                            {
                                CopyImageToRepAppContentFolder(slideItem);
                            }

                            Utils.PrintStructureToFile("itemprop", "Style",
                                "SlideStyle", "Style.json", xmlWriterItem,
                                itemPathFolder + "\\" + "Style.json", slideItem.Layout.Style);


                            if (String.CompareOrdinal(slideItem.Layout.Visualization, "snapshot") == 0)
                            {
                                string id = slideItem.Layout.Style.Id;

                                ISnapshot snapshot = this._itemInfo.App.GetSnapshot(id);

                                Utils.PrintStructureToFile("itemprop", "SnapshotProperties",
                                    "SnapshotProperties", "SnapshotProperties.json", xmlWriterItem,
                                    itemPathFolder + "\\" + "SnapshotProperties.json", snapshot.Properties);
                            }


                            Utils.CreateElement("itemprop", "SheetId", "string",
                                slideItem.Layout.SheetId, xmlWriterItem);

                            Utils.PrintStructureToFile("itemprop", "SelectionState",
                                "SelectionState", "SelectionState.json", xmlWriterItem,
                                itemPathFolder + "\\" + "SelectionState.json", slideItem.Layout.SelectionState);

                            Utils.PrintStructureToFile("itemprop", "EmbeddedSnapshotDef",
                                "SnapshotProperties", "EmbeddedSnapshotDef.json", xmlWriterItem,
                                itemPathFolder + "\\" + "EmbeddedSnapshotDef.json", 
                                slideItem.Layout.EmbeddedSnapshotDef);
                        #endregion

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
        event DeleteInfoFromDiskHandler NewDeleteItemFromDiskSend;
    }

    public delegate void DeleteInfoFromDiskHandler(object sender, DeleteItemFromDiskEventArgs e);

}
