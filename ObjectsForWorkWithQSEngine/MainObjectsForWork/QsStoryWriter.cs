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

#pragma warning disable 618

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class QsStoryWriter : IProgramOptionsEvent, IWriteStoryItemToDisk, IDeleteInfoFromDisk
    {
        public ProgramOptions Options { get; } = new();

        private IConnect _location;

        private WriteStoryToDiskInfo _storyToDiskInfo = new();

        private IApp _app;
        private IStory _currentStoryToWrite;

        public QsStoryItemContainerWriter ItemContainerWriter { get; }

        public event NewProgramOptionsHandler NewProgramOptionsSend;
        public event NewWriteStoryItemToDisktHandler NewStoryItemToDiskSend;
        public event NewIDeleteInfoFromDisktHandler NewDeleteItemFromDiskSend;

        public QsStoryWriter(IProgramOptionsEvent programOptionsEvent, IConnectionStatusInfoEvent connectionStatusInfoEvent, 
            IWriteStoryToDisk writeStoryToDisk,IDeleteStoryFromDisk deleteStory)
        {
            IProgramOptionsEvent progOptionsEvent = programOptionsEvent;
            progOptionsEvent.NewProgramOptionsSend += NewProgramOptionsReceived;

            IConnectionStatusInfoEvent connectStatusInfoEvent = connectionStatusInfoEvent;
            connectStatusInfoEvent.NewConnectionStatusInfoSend += NewConnectionStatusInfoReceived;

            IWriteStoryToDisk writeInfo = writeStoryToDisk;
            writeInfo.NewWriteStoryToDiskSend += NewWriteStoryToDiskReceived;

            IDeleteStoryFromDisk delStory = deleteStory;
            delStory.NewDeleteStoryFromkSend += NewDeleteStoryFromkDiskReceived;

            ItemContainerWriter = new QsStoryItemContainerWriter(this, this,this);
        }

        private void OnNewDeleteItemFromDisk(DeleteItemFromDiskEventArgs e)
        {
            if (this.NewDeleteItemFromDiskSend != null)
                this.NewDeleteItemFromDiskSend(this,e);
        }

        private void NewDeleteStoryFromkDiskReceived(object sender, DeleteStorisFromAppArgs e)
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

                                        OnNewDeleteItemFromDisk(new DeleteItemFromDiskEventArgs(new DeleteItemFromDiskInfo(){ItemFolder = storiFilder +"\\"+folderItem}));
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
                //Directory.Delete(storiFilder);
            }
            
        }

        
        private void NewWriteStoryToDiskReceived(object sender, WriteStoryToDiskEventArgs e)
        {
            e.WriteInfo.Copy(ref _storyToDiskInfo);
            WriteStoryToDisk();
        }

       

        private void WriteStoryToDisk()
        {
            string pathToStories = Options.RepositoryPath + "\\" + _storyToDiskInfo.StoreFolder;

            if (!Directory.Exists(pathToStories))
                Directory.CreateDirectory(pathToStories);

            pathToStories += "\\stories";

            if (!Directory.Exists(pathToStories))
                Directory.CreateDirectory(pathToStories);

            string pathToStore  = pathToStories + "\\" + _storyToDiskInfo.CurrentStory.Id;

            if (!Directory.Exists(pathToStore))
                Directory.CreateDirectory(pathToStore);

            string fileXml = pathToStore + "\\" + _storyToDiskInfo.CurrentStory.Id + ".xml";

            IAppIdentifier appId = _location.GetConnection().AppWithId(_storyToDiskInfo.CurrentApp.Id);

            _app = _location.GetConnection().App(appId);

            _currentStoryToWrite = _app?.GetStory(_storyToDiskInfo.CurrentStory.Id);
            
            string pathEndNamePropertiesFile = pathToStore + "\\" + "Properties.json";
            string pathEndNameLayoutFile = pathToStore + "\\" + "Layout.json";
            string pathEndNameThumbnailFile = pathToStore + "\\" + "Thumbnail.json";
            //string pathEndNameMetaAttributesFile = pathToStore + "\\" + "MetaAttributes.json";
            string pathEndNameNxInfoFile = pathToStore + "\\" + "NxInfo.json";
            string pathEndNameNxLayoutErrorsFile = pathToStore + "\\" + "NxLayoutErrors.json";

            XmlTextWriter xmlWriter = new XmlTextWriter(fileXml, Encoding.UTF8)
            {
                Formatting = Formatting.Indented
            };

            
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteComment("Файл содержит описание истории " + _storyToDiskInfo.CurrentStory.Name);

            xmlWriter.WriteStartElement("story");

                    xmlWriter.WriteStartElement("properties");

                    Utils.CreateElement("item","name","string",_storyToDiskInfo.CurrentStory.Name,xmlWriter);
                    Utils.CreateElement("item", "id", "string", _storyToDiskInfo.CurrentStory.Id, xmlWriter);

                    Utils.PrintStructureToFile("item", "Properties", "StoryProperties", "Properties.json", xmlWriter,
                        pathEndNamePropertiesFile, _currentStoryToWrite?.Properties);
                    Utils.PrintStructureToFile("item", "Layout", "StoryLayout", "Layout.json", xmlWriter,
                        pathEndNameLayoutFile, _currentStoryToWrite?.Layout);
                    Utils.CreateElement("item", "Rank", "float", _currentStoryToWrite?.Layout.Rank.ToString(CultureInfo.InvariantCulture), xmlWriter);

                    Utils.PrintStructureToFile("item", "Thumbnail", "StaticContentUrlContainer", "Thumbnail.json", xmlWriter,
                        pathEndNameThumbnailFile, _currentStoryToWrite?.Thumbnail);

                    Utils.PrintStructureToFile("item", "NxInfo", "NxInfo", "NxInfo.json", xmlWriter,
                        pathEndNameNxInfoFile, _currentStoryToWrite?.Layout.Info);

                    Utils.PrintStructureToFile("item", "NxLayoutErrors", "NxLayoutErrors", "NxLayoutErrors.json", xmlWriter,
                        pathEndNameNxLayoutErrorsFile, _currentStoryToWrite?.Layout.Error);

                    Utils.CreateElement("item", "Layout.SelectionInfo.InSelections", "bool",
                        _currentStoryToWrite?.Layout.SelectionInfo.InSelections.ToString(), xmlWriter);

                    Utils.CreateElement("item", "Layout.SelectionInfo.MadeSelections", "bool",
                        _currentStoryToWrite?.Layout.SelectionInfo.MadeSelections.ToString(), xmlWriter);
            
                    xmlWriter.WriteEndElement();//properties
                    
                    xmlWriter.WriteStartElement("Items");
                    
                    int i = -1;

                    if (_currentStoryToWrite != null)
                        foreach (var item in _currentStoryToWrite.Items)
                        {
                            i++;

                            string mfile = "Item" + i.ToString() + ".json";
                            string pathEndNameItemFile = pathToStore + "\\" + mfile;

                            xmlWriter.WriteStartElement("item");

                            xmlWriter.WriteAttributeString("id1", i.ToString());
                            xmlWriter.WriteAttributeString("id2", item.Info.Id);
                            xmlWriter.WriteAttributeString("Type1", "StoryChildListContainer");
                            xmlWriter.WriteAttributeString("Type2", item.Info.Type);
                            xmlWriter.WriteAttributeString("name", mfile);

                            StoreItemToFile(pathEndNameItemFile, item);

                            OnNewStoryItemWriteToDisk(new StoryItemInfoEventArgs(new StoryItemInfo()
                            {
                                Container = item, Id = item.Info.Id, LocalRootFolder = pathToStore,
                                Story = _currentStoryToWrite
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
                }

            }
        }

        private void OnNewStoryItemWriteToDisk(StoryItemInfoEventArgs e)
        {
            if (NewStoryItemToDiskSend != null)
                NewStoryItemToDiskSend(this, e);
        }


        private void NewConnectionStatusInfoReceived(object sender, ConnectionStatusInfoEventArgs e)
        {
            e.ConnectionStatusInfo.Copy(ref _location);
        }

        private void NewProgramOptionsReceived(object sender, ProgramOptionsEventArgs e)
        {
            e.ProgramOptions.Copy(Options);
            
            if (NewProgramOptionsSend != null)
                NewProgramOptionsSend(this, e);
        }

        
    }

    public class StoryItemInfo
    {
        public IStoryChildListContainer Container;
        public IStory Story;
        public string Id;
        public string LocalRootFolder;
        public void Copy(ref StoryItemInfo anotherItem)
        {
            anotherItem.Container = Container;
            anotherItem.Id = Id;
            anotherItem.LocalRootFolder = LocalRootFolder;
            anotherItem.Story = Story;
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
        event NewWriteStoryItemToDisktHandler NewStoryItemToDiskSend;
    }

    public delegate void NewWriteStoryItemToDisktHandler(object sender, StoryItemInfoEventArgs e);



    public class WriteStoryToDiskInfo
    {
        public NameAndIdPair CurrentApp;
        public NameAndIdPair CurrentStory;
        public string StoreFolder;
        public XmlTextWriter CurrentXmlTextWriter;

        public void Copy(ref WriteStoryToDiskInfo anotherInfo)
        {
            anotherInfo.CurrentApp = CurrentApp.Copy();
            anotherInfo.CurrentStory = CurrentStory.Copy();
            anotherInfo.StoreFolder = StoreFolder;
            anotherInfo.CurrentXmlTextWriter = CurrentXmlTextWriter;
        }

    }

    public class WriteStoryToDiskEventArgs : EventArgs
    {

        public readonly WriteStoryToDiskInfo WriteInfo;

        //Конструкторы
        public WriteStoryToDiskEventArgs(WriteStoryToDiskInfo record)
        {
            WriteInfo = record;
        }
    }

    public interface IWriteStoryToDisk
    {
        event NewWriteStoryToDisktHandler NewWriteStoryToDiskSend;
    }

    public delegate void NewWriteStoryToDisktHandler(object sender, WriteStoryToDiskEventArgs e);

    
    public class DeleteStoryFromAppRecordInfo
    {
        public string CurrentAppFolder;
        public string CurrentStoreFolder;
    }

    public class DeleteStorisFromAppArgs : EventArgs
    {

        public readonly DeleteStoryFromAppRecordInfo DeleteInfo;

        //Конструкторы
        public DeleteStorisFromAppArgs(DeleteStoryFromAppRecordInfo record)
        {
            DeleteInfo = record;
        }
    }

    public interface IDeleteStoryFromDisk
    {
        event NewDeleteStoryFromDisktHandler NewDeleteStoryFromkSend;
    }

    public delegate void NewDeleteStoryFromDisktHandler(object sender, DeleteStorisFromAppArgs e);

}
