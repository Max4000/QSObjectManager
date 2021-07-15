using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UtilClasses.ProgramOptionsClasses;
using UtilClasses.ServiceClasses;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class QsAppWriterClass : IProgramOptionsEvent , IConnectionStatusInfoEvent, IWriteStoryToDiskt,IDeleteStoryFromDisk
    {
        private readonly WriteInfo _wrtWriteInfo = new();
        public ProgramOptions Options { get; } = new();

        private LocationObject _location;

        private XmlTextWriter _xmlWriter;

        public QsStory Story { get; }

        public event NewProgramOptionsHandler NewProgramOptionsSend;
        public event ConnectionStatusInfoHandler NewConnectionStatusInfoSend;
        public event NewWriteStoryToDisktHandler NewWriteStoryToDiskSend;
        public event NewDeleteStoryFromDisktHandler NewDeleteStoryFromkSend;

        private void NewConnectionStatusInfoReceived(object sender, ConnectionStatusInfoEventArgs e)
        {
            e.ConnectionStatusInfo.Copy(ref this._location);
            OnNewConnectioStatusInfo(e);
        }

        private void OnNewStoryInfoToDisk(WriteStoryToDiskEventArgs e)
        {

            if (NewWriteStoryToDiskSend != null)
                NewWriteStoryToDiskSend(this, e);
        }

        private void OnNewStoryDeleteFromDisk(DeleteStorisFromAppArgs e)
        {
            if (NewDeleteStoryFromkSend != null)
                NewDeleteStoryFromkSend(this, e);
        }

        private void NewProgramOptionsReceived(object sender, ProgramOptionsEventArgs e)
        {
            e.ProgramOptions.Copy(this.Options);
            OnNewOptions(e);
        }


        private void NewWriteInfoReceive(object sender, WriteInfoEventArgs e)
        {
            e.WriteInfo.Copy(_wrtWriteInfo);
            DoWrite();
        }
        public void OnNewOptions(ProgramOptionsEventArgs e)
        {
            if (NewProgramOptionsSend != null)
                NewProgramOptionsSend(this, e);
        }

        public void OnNewConnectioStatusInfo(ConnectionStatusInfoEventArgs e)
        {
            if (NewConnectionStatusInfoSend != null)
                NewConnectionStatusInfoSend(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writeInfoEvent"></param>
        /// <param name="programOptionsEvent"></param>
        /// <param name="connectionStatusInfoEvent"></param>
        public QsAppWriterClass(IWriteInfoEvent writeInfoEvent ,IProgramOptionsEvent programOptionsEvent, IConnectionStatusInfoEvent connectionStatusInfoEvent)
        {
            IWriteInfoEvent obj = writeInfoEvent;
            obj.NewWriteInfoSend += NewWriteInfoReceive;

            IProgramOptionsEvent obj2 = programOptionsEvent;
            obj2.NewProgramOptionsSend += NewProgramOptionsReceived;

            IConnectionStatusInfoEvent obj3 = connectionStatusInfoEvent;

            obj3.NewConnectionStatusInfoSend += NewConnectionStatusInfoReceived;

            Story = new QsStory(this, this,this,this);

        }

       

        private void DeleteStoriesFromDisk(string seachFile)
        {
            if (!string.IsNullOrEmpty(seachFile))
            {
                string appFolder = Options.RepositoryPath + "\\" + Path.GetFileNameWithoutExtension(seachFile);

                string storiesFolder = appFolder + "\\" + "stories";


                foreach (var dir in Directory.GetDirectories(storiesFolder))
                {
                    OnNewStoryDeleteFromDisk(new DeleteStorisFromAppArgs(new DeleteStorisFromAppRecordInfo()
                        {CurrentAppFolder = appFolder, CurrentStoreFolder = dir}));

                    Directory.Delete(dir);
                }
            }
        }

        private void DoWrite()
        {
            if (_location == null)
                return;
            
            if (!Directory.Exists(Options.RepositoryPath))
            {

                return;
            }

            string mNameSelectedApp = Path.GetFileNameWithoutExtension(_wrtWriteInfo.SelectedApp.Name);


            string  seachFile = FindFiles.SearchFileAppInStore(Options.RepositoryPath, mNameSelectedApp, "*.xml");

            if (!string.IsNullOrEmpty(seachFile))
            {
                DeleteStoriesFromDisk(seachFile);

                string appFolder = Options.RepositoryPath + "\\" + Path.GetFileNameWithoutExtension(seachFile);

                Directory.Delete(appFolder + "\\" + "stories");
                
                Directory.Delete(appFolder);
                
                DeleteHeadierOfAppFromDisk(seachFile);
            }

            string fileXml = GetNewNameAppXmLfile();

            _xmlWriter = new XmlTextWriter(fileXml, Encoding.UTF8)
            {
                Formatting = Formatting.Indented
            };

            
            _xmlWriter.WriteStartDocument();

            _xmlWriter.WriteComment("Файл содержит описание приложения "+ _wrtWriteInfo.SelectedApp.Name);

            
            _xmlWriter.WriteStartElement("application");

            
                _xmlWriter.WriteStartElement("properties");

                    _xmlWriter.WriteElementString("name", _wrtWriteInfo.SelectedApp.Name);

            
                    _xmlWriter.WriteElementString("id", _wrtWriteInfo.SelectedApp.Id);

                    _xmlWriter.WriteStartElement("stories");


                        foreach (var story in this._wrtWriteInfo.SelectedStories)
                        {
                            _xmlWriter.WriteStartElement("story");
                                
                                _xmlWriter.WriteElementString("storyName", story.Name); 
                                _xmlWriter.WriteElementString("id",story.Id);

                                WriteStoryToDiskInfo storeInfo = new WriteStoryToDiskInfo
                                {
                                    StoreFolder = Path.GetFileNameWithoutExtension(fileXml),
                                    CurrentApp = _wrtWriteInfo.SelectedApp.Copy(),
                                    CuurentStory = story.Copy(),
                                    CurrentXmlTextWriter = _xmlWriter
                                };


                                OnNewStoryInfoToDisk(new WriteStoryToDiskEventArgs(storeInfo));       
                                

                            _xmlWriter.WriteEndElement();
                        }

                    _xmlWriter.WriteEndElement();
                
                _xmlWriter.WriteEndElement();

            _xmlWriter.WriteEndElement();


            _xmlWriter.WriteEndDocument();

            _xmlWriter.Flush();
            _xmlWriter.Close();

        }


        private void DeleteHeadierOfAppFromDisk(string seachFile)
        {
            File.Delete(seachFile);
        }

        private string GetNewNameAppXmLfile()
        {
            string mNameSelectedApp = Path.GetFileNameWithoutExtension(_wrtWriteInfo.SelectedApp.Name);

            return  Options.RepositoryPath + "\\" + mNameSelectedApp + "_" + DateTimeUtlis.NowToString() + ".xml";

        }

    }

    public class WriteInfo
    {
        public NameAndIdPair SelectedApp;
        public IList<NameAndIdPair> SelectedStories;

        public WriteInfo(NameAndIdPair selectedApp, IList<NameAndIdPair> selectedStories)
        {
            SelectedApp = selectedApp;
            SelectedStories = selectedStories;
        }

        public WriteInfo()
        {

        }
        public void Copy(WriteInfo anotherWriteInfo)
        {
            anotherWriteInfo.SelectedApp = SelectedApp.Copy();
            anotherWriteInfo.SelectedStories = new List<NameAndIdPair>();
            foreach (var story in this.SelectedStories)
            {
                anotherWriteInfo.SelectedStories.Add(story.Copy());
            }
        }

    }

    public class WriteInfoEventArgs : EventArgs
    {

        public readonly WriteInfo WriteInfo;

        //Конструкторы
        public WriteInfoEventArgs(WriteInfo record)
        {
            WriteInfo = record;
        }
    }

    public interface IWriteInfoEvent
    {
        event NewWriterInfosHandler NewWriteInfoSend;
    }

    public delegate void NewWriterInfosHandler(object sender, WriteInfoEventArgs e);
}
