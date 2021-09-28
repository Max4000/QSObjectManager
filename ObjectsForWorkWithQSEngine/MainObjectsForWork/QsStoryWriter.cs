using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Qlik.Engine;
using Qlik.Sense.Client;
using Qlik.Sense.Client.Storytelling;
using UtilClasses;
using UtilClasses.ProgramOptionsClasses;
// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo

#pragma warning disable 618

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class QsStoryWriter : IWriteStoryItemToDisk, IDeleteInfoFromDisk, IProgramOptionsEvent
    {
        private ProgramOptions Options { get; } = new();

        private WriteStoryToDiskInfo _storyToDiskInfo = new();

        private IStory _currentStoryToWrite;


        public event WriteStoryItemToDiskHandler NewStoryItemToDiskSend;
        public event DeleteInfoFromDisktHandler NewDeleteItemFromDiskSend;
        public event ProgramOptionsHandler NewProgramOptionsSend;

        public QsStoryWriter(IProgramOptionsEvent programOptionsEvent,  
            IWriteStoryToDisk writeStoryToDisk,IDeleteStoryFromDisk deleteStory)
        {
            IProgramOptionsEvent progOptionsEvent = programOptionsEvent;
            progOptionsEvent.NewProgramOptionsSend += ProgramOptionsReceived;

           
            IWriteStoryToDisk writeInfo = writeStoryToDisk;
            writeInfo.NewWriteStoryToDiskSend += WriteStoryToDiskReceived;

            IDeleteStoryFromDisk delStory = deleteStory;
            delStory.NewDeleteStoryFromDiskSend += DeleteStoryFromDiskReceived;

            var unused = new QsSlideWriter(this,this,this);
        }

        private void OnNewDeleteItemFromDisk(DeleteItemFromDiskEventArgs e)
        {
            if (this.NewDeleteItemFromDiskSend != null)
                this.NewDeleteItemFromDiskSend(this,e);
        }

        private void DeleteStoryFromDiskReceived(object sender, DeleteStoryFromAppArgs e)
        {
            string storiFilder = e.DeleteInfo.CurrentStoreFolder;

            foreach (var xmlFile in Directory.GetFiles(storiFilder, "*.xml"))
            {
                XmlDocument doc = new XmlDocument();

                doc.Load(xmlFile);

                XmlNode root = doc.DocumentElement;

                if (root != null)
                    foreach (XmlNode node in root.ChildNodes)
                    {
                        switch (node.Name)
                        {
                            case "Items":
                            {
                                foreach (XmlNode item in node.ChildNodes)
                                {
                                    XmlAttributeCollection attrs = item.Attributes;

                                    if (attrs != null)
                                    {
                                        string folderItem = attrs.GetNamedItem("id2")?.Value;

                                        OnNewDeleteItemFromDisk(new DeleteItemFromDiskEventArgs(new DeleteItemFromDiskInfo()
                                        {
                                            ItemFolder = storiFilder +"\\"+folderItem,
                                            ItemName = folderItem

                                        }));
                                        Directory.Delete(storiFilder + "\\" + folderItem);

                                    }
                                }
                                break;
                            }
                        }
                    }
                foreach (var file in Directory.GetFiles(storiFilder))
                {
                    File.Delete(file);
                }
                
            }
            
        }

        
        private void WriteStoryToDiskReceived(object sender, WriteStoryToDiskEventArgs e)
        {
            e.WriteInfo.Copy(ref _storyToDiskInfo);
            WriteStoryToDisk();
        }

       

        private void WriteStoryToDisk()
        {
            
            string pathToStories = Options.RepositoryPath + "\\" + _storyToDiskInfo.StoreFolder;
            string pathToContent = Options.RepositoryPath + "\\" + _storyToDiskInfo.AppContentFolder;
            string pathToDefault = Options.RepositoryPath + "\\" + _storyToDiskInfo.DefaultContentFolder;

            if (!Directory.Exists(pathToDefault))
                Directory.CreateDirectory(pathToDefault);

            if (!Directory.Exists(pathToContent))
                Directory.CreateDirectory(pathToContent);


            if (!Directory.Exists(pathToStories))
                Directory.CreateDirectory(pathToStories);

            pathToStories += "\\stories";

            if (!Directory.Exists(pathToStories))
                Directory.CreateDirectory(pathToStories);

            string pathToStore  = pathToStories + "\\" + _storyToDiskInfo.CurrentStory.Id;

            if (!Directory.Exists(pathToStore))
                Directory.CreateDirectory(pathToStore);

            string fileXml = pathToStore + "\\" + _storyToDiskInfo.CurrentStory.Id + ".xml";

            

            _currentStoryToWrite = _storyToDiskInfo.App.GetStory(_storyToDiskInfo.CurrentStory.Id);
            
            

            XmlTextWriter xmlWriter = new XmlTextWriter(fileXml, Encoding.UTF8)
            {
                Formatting = Formatting.Indented
            };

            
            xmlWriter.WriteStartDocument();
            // ReSharper disable once StringLiteralTypo
            xmlWriter.WriteComment("Файл содержит описание истории " + _storyToDiskInfo.CurrentStory.Name);

            xmlWriter.WriteStartElement("story");

                    xmlWriter.WriteStartElement("properties");

                    Utils.CreateElement("item","name","string",_storyToDiskInfo.CurrentStory.Name,xmlWriter);
                    Utils.CreateElement("item", "id", "string", _storyToDiskInfo.CurrentStory.Id, xmlWriter);
                    

                    string pathEndNamePropertiesMetaDef = pathToStore + "\\" + "Properties.MetaDef.json";

                    Utils.PrintStructureToFile("item", "Properties.MetaDef", "MetaAttributesDef", "Properties.MetaDef.json", xmlWriter,
                        pathEndNamePropertiesMetaDef, _currentStoryToWrite?.Properties.MetaDef);

                    
                    string pathEndNamePropertiesChildListDef = pathToStore + "\\" + "Properties.ChildListDef.json";

                    Utils.PrintStructureToFile("item", "Properties.ChildListDef", "StoryChildListDef", "Properties.ChildListDef.json", xmlWriter,
                        pathEndNamePropertiesChildListDef, _currentStoryToWrite?.Properties.ChildListDef);

                    
                    Utils.CreateElement("item", "Properties.Rank", "float",
                        _currentStoryToWrite?.Properties.Rank.ToString(CultureInfo.InvariantCulture), xmlWriter);

                    
                    string pathEndNamePropertiesThumbnail = pathToStore + "\\" + "Properties.Thumbnail.json";

                    Utils.PrintStructureToFile("item", "Properties.Thumbnail", "StaticContentUrlContainerDef", "Properties.Thumbnail.json", xmlWriter,
                        pathEndNamePropertiesThumbnail, _currentStoryToWrite?.Properties.Thumbnail);

                    #region Hidden code

                    //Utils.PrintStructureToFile("item", "Layout", "StoryLayout", "Layout.json", xmlWriter,
                    //    pathEndNameLayoutFile, _currentStoryToWrite?.Layout);
                    //Utils.CreateElement("item", "Rank", "float", _currentStoryToWrite?.Layout.Rank.ToString(CultureInfo.InvariantCulture), xmlWriter);

                    //Utils.PrintStructureToFile("item", "Thumbnail", "StaticContentUrlContainer", "Thumbnail.json", xmlWriter,
                    //    pathEndNameThumbnailFile, _currentStoryToWrite?.Thumbnail);

                    //Utils.PrintStructureToFile("item", "NxInfo", "NxInfo", "NxInfo.json", xmlWriter,
                    //    pathEndNameNxInfoFile, _currentStoryToWrite?.Layout.Info);

                    //Utils.PrintStructureToFile("item", "NxLayoutErrors", "NxLayoutErrors", "NxLayoutErrors.json", xmlWriter,
                    //    pathEndNameNxLayoutErrorsFile, _currentStoryToWrite?.Layout.Error);

                    //Utils.CreateElement("item", "Layout.SelectionInfo.InSelections", "bool",
                    //    _currentStoryToWrite?.Layout.SelectionInfo.InSelections.ToString(), xmlWriter);

                    //Utils.CreateElement("item", "Layout.SelectionInfo.MadeSelections", "bool",
                    //    _currentStoryToWrite?.Layout.SelectionInfo.MadeSelections.ToString(), xmlWriter);
                    #endregion

                    xmlWriter.WriteEndElement();//properties
                    
                    xmlWriter.WriteStartElement("Items");
                    
                    int i = -1;

                    if (_currentStoryToWrite != null)
                        foreach (var item in _currentStoryToWrite.Items)
                        {
                            i++;

                            string file = "Item" + i.ToString() + ".json";
                            string pathEndNameItemFile = pathToStore + "\\" + file;

                            xmlWriter.WriteStartElement("item");

                            xmlWriter.WriteAttributeString("id1", i.ToString());
                            xmlWriter.WriteAttributeString("id2", item.Info.Id);
                            xmlWriter.WriteAttributeString("Type1", "StoryChildListContainer");
                            xmlWriter.WriteAttributeString("Type2", item.Info.Type);
                            xmlWriter.WriteAttributeString("name", file);

                            StoreItemToFile(pathEndNameItemFile, item);

                            OnNewStoryItemWriteToDisk(new StoryItemInfoEventArgs(new StoryItemInfo()
                            {
                                Container = item, Id = item.Info.Id, 
                                LocalRootFolder = pathToStore,
                                Story = _currentStoryToWrite,
                                App = _storyToDiskInfo.App,
                                CurrentApp = _storyToDiskInfo.CurrentApp.Copy(),
                                AppContentFolder = pathToContent,
                                DefaultContnetFolder = pathToDefault
                            }));

                            xmlWriter.WriteEndElement();
                        }

                    xmlWriter.WriteEndElement();//items


            xmlWriter.WriteEndElement();//story
            xmlWriter.WriteEndDocument();
            
            xmlWriter.Flush();
            xmlWriter.Close();

        }

        private void StoreItemToFile(string file , IStoryChildListContainer item)
        {

            if (_currentStoryToWrite != null)
            {
                if (item != null)
                {
                    string json = item.PrintStructure(Newtonsoft.Json.Formatting.Indented);

                    using var propertyFile = new AppEntryWriter(file);

                    propertyFile.Writer.Write(json);
                    propertyFile.Writer.Close();
                    propertyFile.Dispose();
                }

            }
        }

        private void OnNewStoryItemWriteToDisk(StoryItemInfoEventArgs e)
        {
            if (NewStoryItemToDiskSend != null)
                NewStoryItemToDiskSend(this, e);
        }


        private void ProgramOptionsReceived(object sender, ProgramOptionsEventArgs e)
        {
            e.ProgramOptions.Copy(Options);
            if (this.NewProgramOptionsSend != null)
            {
                NewProgramOptionsSend(this, e);
            }
            
        }

        
    }

    public class StoryItemInfo
    {
        public IStoryChildListContainer Container;
        public IStory Story;
        public IApp App;
        public NameAndIdAndLastReloadTime CurrentApp;
        public string Id;
        public string LocalRootFolder;
        public string AppContentFolder;
        public string DefaultContnetFolder;

        public void Copy(ref StoryItemInfo anotherItem)
        {
            anotherItem.AppContentFolder = AppContentFolder;
            anotherItem.Container = Container;
            anotherItem.CurrentApp = CurrentApp.Copy();
            anotherItem.App = App;
            anotherItem.Id = Id;
            anotherItem.LocalRootFolder = LocalRootFolder;
            anotherItem.Story = Story;
            anotherItem.DefaultContnetFolder = DefaultContnetFolder;
        }

    }

    public class StoryItemInfoEventArgs : EventArgs
    {
        public readonly StoryItemInfo ItemInfo;

        public StoryItemInfoEventArgs(StoryItemInfo item)
        {
            ItemInfo = item;
        }
    }

    public interface IWriteStoryItemToDisk
    {
        event WriteStoryItemToDiskHandler NewStoryItemToDiskSend;
    }

    public delegate void WriteStoryItemToDiskHandler(object sender, StoryItemInfoEventArgs e);



    public class WriteStoryToDiskInfo
    {
        public NameAndIdAndLastReloadTime CurrentApp;
        public NameAndIdAndLastReloadTime CurrentStory;
        public string StoreFolder;
        public IApp App;
        public XmlTextWriter CurrentXmlTextWriter;
        public string AppContentFolder;
        public string DefaultContentFolder;

        public void Copy(ref WriteStoryToDiskInfo anotherInfo)
        {
            anotherInfo.App = App;
            anotherInfo.CurrentApp = CurrentApp.Copy();
            anotherInfo.CurrentStory = CurrentStory.Copy();
            anotherInfo.StoreFolder = StoreFolder;
            anotherInfo.CurrentXmlTextWriter = CurrentXmlTextWriter;
            anotherInfo.AppContentFolder = AppContentFolder;
            anotherInfo.DefaultContentFolder = DefaultContentFolder;
        }

    }

    public class WriteStoryToDiskEventArgs : EventArgs
    {

        public readonly WriteStoryToDiskInfo WriteInfo;

        
        public WriteStoryToDiskEventArgs(WriteStoryToDiskInfo record)
        {
            WriteInfo = record;
        }
    }

    public interface IWriteStoryToDisk
    {
        event WriteStoryToDiskHandler NewWriteStoryToDiskSend;
    }

    public delegate void WriteStoryToDiskHandler(object sender, WriteStoryToDiskEventArgs e);

    
    public class DeleteStoryFromAppRecordInfo
    {
        public string CurrentAppFolder;
        public string CurrentStoreFolder;
    }

    public class DeleteStoryFromAppArgs : EventArgs
    {

        public readonly DeleteStoryFromAppRecordInfo DeleteInfo;

        
        public DeleteStoryFromAppArgs(DeleteStoryFromAppRecordInfo record)
        {
            DeleteInfo = record;
        }
    }

    public interface IDeleteStoryFromDisk
    {
        event DeleteStoryFromDiskHandler NewDeleteStoryFromDiskSend;
    }

    public delegate void DeleteStoryFromDiskHandler(object sender, DeleteStoryFromAppArgs e);

}
