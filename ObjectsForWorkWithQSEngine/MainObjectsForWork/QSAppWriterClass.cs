using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Qlik.Engine;
using UtilClasses.ProgramOptionsClasses;
using UtilClasses.ServiceClasses;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class QsAppWriterClass : IProgramOptionsEvent , /*IConnectionStatusInfoEvent,*/ IWriteStoryToDisk,IDeleteStoryFromDisk
    {
        private readonly WriteInfo _wrtWriteInfo = new();
        public ProgramOptions Options { get; } = new();

        private IConnect _location;

        private XmlTextWriter _xmlWriter;

        private IApp _app;

        public event NewProgramOptionsHandler NewProgramOptionsSend;
        //public event ConnectionStatusInfoHandler NewConnectionStatusInfoSend;
        public event NewWriteStoryToDiskHandler NewWriteStoryToDiskSend;
        public event NewDeleteStoryFromDiskHandler NewDeleteStoryFromDiskSend;

        private void NewConnectionStatusInfoReceived(object sender, ConnectionStatusInfoEventArgs e)
        {
            e.ConnectionStatusInfo.Copy(ref this._location);
            //OnNewConnectionStatusInfo(e);
        }

        private void OnNewStoryInfoToDisk(WriteStoryToDiskEventArgs e)
        {

            if (NewWriteStoryToDiskSend != null)
                NewWriteStoryToDiskSend(this, e);
        }

        private void OnNewStoryDeleteFromDisk(DeleteStoryFromAppArgs e)
        {
            if (NewDeleteStoryFromDiskSend != null)
                NewDeleteStoryFromDiskSend(this, e);
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

        //public void OnNewConnectionStatusInfo(ConnectionStatusInfoEventArgs e)
        //{
        //    if (NewConnectionStatusInfoSend != null)
        //        NewConnectionStatusInfoSend(this, e);
        //}

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

            var unused = new QsStoryWriter( this,this,this);
        }

       

        private void DeleteStoriesFromDisk(string seachFile)
        {
            if (!string.IsNullOrEmpty(seachFile))
            {
                string appFolder = Options.RepositoryPath + "\\" + Path.GetFileNameWithoutExtension(seachFile);

                string storiesFolder = appFolder + "\\" + "stories";


                foreach (var dir in Directory.GetDirectories(storiesFolder))
                {

                    OnNewStoryDeleteFromDisk(new DeleteStoryFromAppArgs(new DeleteStoryFromAppRecordInfo()
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

            IAppIdentifier appId = _location.GetConnection().AppWithId(_wrtWriteInfo.SelectedApp.Id);

            _app = _location.GetConnection().App(appId);


            string searchFileAppInStore = FindFiles.SearchFileAppInStore(Options.RepositoryPath, mNameSelectedApp, "*.xml");

            if (!string.IsNullOrEmpty(searchFileAppInStore))
            {
                DeleteStoriesFromDisk(searchFileAppInStore);

                string appFolder = Options.RepositoryPath + "\\" + Path.GetFileNameWithoutExtension(searchFileAppInStore);

                Directory.Delete(appFolder + "\\" + "stories");
                
                Directory.Delete(appFolder);
                
                DeleteHeadierOfAppFromDisk(searchFileAppInStore);
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

                                if (_app != null)
                                {
                                    WriteStoryToDiskInfo storeInfo = new WriteStoryToDiskInfo
                                    {
                                        App =     _app,
                                        StoreFolder = Path.GetFileNameWithoutExtension(fileXml),
                                        CurrentApp = _wrtWriteInfo.SelectedApp.Copy(),
                                        CurrentStory = story.Copy(),
                                        CurrentXmlTextWriter = _xmlWriter
                                    };


                                    OnNewStoryInfoToDisk(new WriteStoryToDiskEventArgs(storeInfo));
                                }


                                _xmlWriter.WriteEndElement();
                        }

                    _xmlWriter.WriteEndElement();
                
                _xmlWriter.WriteEndElement();

            _xmlWriter.WriteEndElement();


            _xmlWriter.WriteEndDocument();

            _xmlWriter.Flush();
            _xmlWriter.Close();
            if (_app != null) _app.Dispose();
            _app = null;

        }


        private void DeleteHeadierOfAppFromDisk(string seachFile)
        {
            File.Delete(seachFile);
        }

        private string GetNewNameAppXmLfile()
        {
            string mNameSelectedApp = Path.GetFileNameWithoutExtension(_wrtWriteInfo.SelectedApp.Name);

            return  Options.RepositoryPath + "\\" + mNameSelectedApp + "_" + DateTimeUtils.NowToString() + ".xml";

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
