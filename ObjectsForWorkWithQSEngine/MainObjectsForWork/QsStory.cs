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
    public class QsStory
    {
        public ProgramOptions Options { get; } = new();

        private LocationObject _location;

        private WriteStoryToDiskInfo _storyToDiskInfo = new();

        private IApp _app;
        private IStory _currentStoryToWrite;

        public QsStory(IProgramOptionsEvent programOptionsEvent, IConnectionStatusInfoEvent connectionStatusInfoEvent, 
            IWriteStoryToDiskt writeStoryToDiskt,IDeleteStoryFromDisk deleteStory)
        {
            IProgramOptionsEvent progOptionsEvent = programOptionsEvent;
            progOptionsEvent.NewProgramOptionsSend += NewProgramOptionsReceived;

            IConnectionStatusInfoEvent connectStatusInfoEvent = connectionStatusInfoEvent;
            connectStatusInfoEvent.NewConnectionStatusInfoSend += NewConnectionStatusInfoReceived;

            IWriteStoryToDiskt writeInfo = writeStoryToDiskt;
            writeInfo.NewWriteStoryToDiskSend += NewWriteStoryToDiskReceived;

            IDeleteStoryFromDisk delStory = deleteStory;
            delStory.NewDeleteStoryFromkSend += NewDeleteStoryFromkReceived;
        }

        private void NewDeleteStoryFromkReceived(object sender, DeleteStorisFromAppArgs e)
        {
            string storiFilder = e.DeleteInfo.CurrentStoreFolder;

            foreach (var file in Directory.GetFiles(storiFilder))
            {
                File.Delete(file);
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

            string pathToStore  = pathToStories + "\\" + _storyToDiskInfo.CuurentStory.Id;

            if (!Directory.Exists(pathToStore))
                Directory.CreateDirectory(pathToStore);

            string fileXml = pathToStore + "\\" + _storyToDiskInfo.CuurentStory.Id + ".xml";

            _currentStoryToWrite = Utils.GetStoryFromApp(_location.LocationPersonalEdition, _storyToDiskInfo.CurrentApp.Id,
                _storyToDiskInfo.CuurentStory.Id);

            string pathEndNamePropertiesFile = pathToStore + "\\" + "Properties.json";
            string pathEndNameLayoutFile = pathToStore + "\\" + "Layout.json";
            string pathEndNameThumbnailFile = pathToStore + "\\" + "Thumbnail.json";
            string pathEndNameMetaAttributesFile = pathToStore + "\\" + "MetaAttributes.json";
            string pathEndNameNxInfoFile = pathToStore + "\\" + "NxInfo.json";
            string pathEndNameNxLayoutErrorsFile = pathToStore + "\\" + "NxLayoutErrors.json";

            XmlTextWriter xmlWriter = new XmlTextWriter(fileXml, Encoding.UTF8)
            {
                Formatting = Formatting.Indented
            };

            
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteComment("Файл содержит описание истории " + _storyToDiskInfo.CuurentStory.Name);

            xmlWriter.WriteStartElement("story");

                    xmlWriter.WriteStartElement("properties");

                        xmlWriter.WriteStartElement("item");

                            xmlWriter.WriteAttributeString("id", "name");
                            xmlWriter.WriteAttributeString("Type", "string");
                            xmlWriter.WriteAttributeString("name", _storyToDiskInfo.CuurentStory.Name);

                        xmlWriter.WriteEndElement();


                        xmlWriter.WriteStartElement("item");
            
                            xmlWriter.WriteAttributeString("id", "id");
                            xmlWriter.WriteAttributeString("Type", "string");
                            xmlWriter.WriteAttributeString("name", _storyToDiskInfo.CuurentStory.Id);

                        xmlWriter.WriteEndElement();


                        xmlWriter.WriteStartElement("item");

                            xmlWriter.WriteAttributeString("id", "Properties");
                            xmlWriter.WriteAttributeString("Type", "StoryProperties");
                            xmlWriter.WriteAttributeString("name", "Properties.json");
                            
                            StorePropertiesToFile(pathEndNamePropertiesFile);

                        xmlWriter.WriteEndElement();


                        xmlWriter.WriteStartElement("item");

                            xmlWriter.WriteAttributeString("id", "Layout");
                            xmlWriter.WriteAttributeString("Type", "StoryLayout");
                            xmlWriter.WriteAttributeString("name", "Layout.json");

                            StoreLayoutToFile(pathEndNameLayoutFile);

                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("item");

                            xmlWriter.WriteAttributeString("id", "Rank");
                            xmlWriter.WriteAttributeString("Type", "float");
                            xmlWriter.WriteAttributeString("name", _currentStoryToWrite.Layout.Rank.ToString(CultureInfo.InvariantCulture));

                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("item");

                            xmlWriter.WriteAttributeString("id", "Thumbnail");
                            xmlWriter.WriteAttributeString("Type", "StaticContentUrlContainer");
                            xmlWriter.WriteAttributeString("name", "Thumbnail.json");

                            StoreThumbnailToFile(pathEndNameThumbnailFile);

                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("item");

                            xmlWriter.WriteAttributeString("id", "MetaAttributes");
                            xmlWriter.WriteAttributeString("Type", "MetaAttributes");
                            xmlWriter.WriteAttributeString("name", "MetaAttributes.json");

                            StoreMetaAttributesToFile(pathEndNameMetaAttributesFile);

                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("item");

                            xmlWriter.WriteAttributeString("id", "NxInfo");
                            xmlWriter.WriteAttributeString("Type", "NxInfo");
                            xmlWriter.WriteAttributeString("name", "NxInfo.json");

                            StoreNxInfoToFile(pathEndNameNxInfoFile);

                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("item");

                            xmlWriter.WriteAttributeString("id", "NxLayoutErrors");
                            xmlWriter.WriteAttributeString("Type", "NxLayoutErrors");
                            xmlWriter.WriteAttributeString("name", "NxLayoutErrors.json");

                            StoreNxLayoutErrorsToFile(pathEndNameNxLayoutErrorsFile);

                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("item");

                            xmlWriter.WriteAttributeString("id", "NxSelectionInfo");
                            xmlWriter.WriteAttributeString("Type", "NxSelectionInfo");
                            xmlWriter.WriteAttributeString("name", "NxLayoutErrors.json");


                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("item");

                            xmlWriter.WriteAttributeString("id", "Layout.SelectionInfo.InSelections");
                            xmlWriter.WriteAttributeString("Type", "bool");
                            xmlWriter.WriteAttributeString("name", _currentStoryToWrite.Layout.SelectionInfo.InSelections.ToString());


                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("item");

                            xmlWriter.WriteAttributeString("id", "Layout.SelectionInfo.MadeSelections");
                            xmlWriter.WriteAttributeString("Type", "bool");
                            xmlWriter.WriteAttributeString("name", _currentStoryToWrite.Layout.SelectionInfo.MadeSelections.ToString());


                        xmlWriter.WriteEndElement();


            xmlWriter.WriteEndElement();//properties


            xmlWriter.WriteEndElement();//story
            xmlWriter.WriteEndDocument();
            
            xmlWriter.Flush();
            xmlWriter.Close();

        }

        private void StoreNxLayoutErrorsToFile(string file)
        {

            if (_currentStoryToWrite != null)
            {
                NxLayoutErrors nxLayoutErrors = _currentStoryToWrite.NxLayoutErrors;

                string json = nxLayoutErrors.PrintStructure(Newtonsoft.Json.Formatting.Indented);

                using var propertyFile = new AppEntryWriter(file);

                propertyFile.Writer.Write(json);
                propertyFile.Writer.Close();
            }
        }


        private void StoreNxInfoToFile(string file)
        {

            if (_currentStoryToWrite != null)
            {
                NxInfo layoutInfo = _currentStoryToWrite.Layout.Info;

                string json = layoutInfo.PrintStructure(Newtonsoft.Json.Formatting.Indented);

                using var propertyFile = new AppEntryWriter(file);

                propertyFile.Writer.Write(json);
                propertyFile.Writer.Close();
            }
        }


        private void StoreMetaAttributesToFile(string file)
        {

            if (_currentStoryToWrite != null)
            {
                MetaAttributes metaAttributes = _currentStoryToWrite.MetaAttributes;

                string json = metaAttributes.PrintStructure(Newtonsoft.Json.Formatting.Indented);

                using var propertyFile = new AppEntryWriter(file);

                propertyFile.Writer.Write(json);
                propertyFile.Writer.Close();
            }
        }


        private void StoreThumbnailToFile(string file)
        {

            if (_currentStoryToWrite != null)
            {
                StaticContentUrlContainer storyThumbnail = _currentStoryToWrite.Thumbnail;

                string thumbnailJson = storyThumbnail.PrintStructure(Newtonsoft.Json.Formatting.Indented);

                using var propertyFile = new AppEntryWriter(file);

                propertyFile.Writer.Write(thumbnailJson);
                propertyFile.Writer.Close();
            }
        }

        private void StoreLayoutToFile(string file)
        {

            if (_currentStoryToWrite != null)
            {
                StoryLayout storyLayout = _currentStoryToWrite.Layout;

                string layoutJson = storyLayout.PrintStructure(Newtonsoft.Json.Formatting.Indented);

                using var propertyFile = new AppEntryWriter(file);

                propertyFile.Writer.Write(layoutJson);
                propertyFile.Writer.Close();
            }
        }
        private void StorePropertiesToFile(string file)
        {

            IAppIdentifier appId = _location.LocationPersonalEdition.AppWithId(_storyToDiskInfo.CurrentApp.Id);

            _app = _location.LocationPersonalEdition.App(appId);

            _currentStoryToWrite = _app?.GetStory(_storyToDiskInfo.CuurentStory.Id);

            if (_currentStoryToWrite != null)
            {
                StoryProperties mProperties = _currentStoryToWrite.Properties;

                string propertyJson = mProperties.PrintStructure(Newtonsoft.Json.Formatting.Indented);

                using var propertyFile = new  AppEntryWriter(file);

                propertyFile.Writer.Write(propertyJson);
                propertyFile.Writer.Close();
            }
        }

        private void NewConnectionStatusInfoReceived(object sender, ConnectionStatusInfoEventArgs e)
        {
            e.ConnectionStatusInfo.Copy(ref _location);
        }

        private void NewProgramOptionsReceived(object sender, ProgramOptionsEventArgs e)
        {
            e.ProgramOptions.Copy(Options);
        }

    }


    public class WriteStoryToDiskInfo
    {
        public NameAndIdPair CurrentApp;
        public NameAndIdPair CuurentStory;
        public string StoreFolder;
        public XmlTextWriter CurrentXmlTextWriter;

        public void Copy(ref WriteStoryToDiskInfo anotherInfo)
        {
            anotherInfo.CurrentApp = CurrentApp.Copy();
            anotherInfo.CuurentStory = CuurentStory.Copy();
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

    public interface IWriteStoryToDiskt
    {
        event NewWriteStoryToDisktHandler NewWriteStoryToDiskSend;
    }

    public delegate void NewWriteStoryToDisktHandler(object sender, WriteStoryToDiskEventArgs e);

    
    public class DeleteStorisFromAppRecordInfo
    {
        public string CurrentAppFolder;
        public string CurrentStoreFolder;
    }

    public class DeleteStorisFromAppArgs : EventArgs
    {

        public readonly DeleteStorisFromAppRecordInfo DeleteInfo;

        //Конструкторы
        public DeleteStorisFromAppArgs(DeleteStorisFromAppRecordInfo record)
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
