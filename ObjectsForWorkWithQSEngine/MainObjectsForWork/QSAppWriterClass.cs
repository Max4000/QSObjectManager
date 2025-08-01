﻿using System;
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

        public event ProgramOptionsHandler NewProgramOptionsSend;
        //public event ConnectionStatusInfoHandler NewConnectionStatusInfoSend;
        public event WriteStoryToDiskHandler NewWriteStoryToDiskSend;
        public event DeleteStoryFromDiskHandler NewDeleteStoryFromDiskSend;

        private void ConnectionStatusInfoReceived(object sender, ConnectionStatusInfoEventArgs e)
        {
            this._location = e.ConnectionStatusInfo.Copy();
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

        private void ProgramOptionsReceived(object sender, ProgramOptionsEventArgs e)
        {
            e.ProgramOptions.Copy(this.Options);
            OnNewOptions(e);
        }


        private void WriteInfoReceived(object sender, WriteInfoEventArgs e)
        {
            e.WriteInfo.Copy(_wrtWriteInfo);
            DoWrite();
        }
        public void OnNewOptions(ProgramOptionsEventArgs e)
        {
            if (NewProgramOptionsSend != null)
                NewProgramOptionsSend(this, e);
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
            obj.NewWriteInfoSend += WriteInfoReceived;

            IProgramOptionsEvent obj2 = programOptionsEvent;
            obj2.NewProgramOptionsSend += ProgramOptionsReceived;

            IConnectionStatusInfoEvent obj3 = connectionStatusInfoEvent;

            obj3.NewConnectionStatusInfoSend += ConnectionStatusInfoReceived;

            var unused = new QsStoryWriter( this,this,this);
        }

       

        private void DeleteStoriesFromDisk(string searchFile)
        {
            if (!string.IsNullOrEmpty(searchFile))
            {
                string appFolder = Options.RepositoryPath + "\\" + Path.GetFileNameWithoutExtension(searchFile);

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

                foreach (var mfile in Directory.GetFiles(appFolder + "\\" + "appcontent"))
                {
                    File.Delete(mfile);
                }

                Directory.Delete(appFolder + "\\" + "appcontent");

                foreach (var mfile in Directory.GetFiles(appFolder + "\\" + "default"))
                {
                    File.Delete(mfile);
                }

                Directory.Delete(appFolder + "\\" + "default");

                Directory.Delete(appFolder);


                DeleteHeadierOfAppFromDisk(searchFileAppInStore);
            }

            string fileXml = GetNewNameAppXmlFile();

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
                    _xmlWriter.WriteElementString("LastReloadTime", _wrtWriteInfo.SelectedApp.LastReloadTime);

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
                                        AppContentFolder = Path.GetFileNameWithoutExtension(fileXml) + "\\appcontent",
                                        DefaultContentFolder = Path.GetFileNameWithoutExtension(fileXml) + "\\default",
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


        private void DeleteHeadierOfAppFromDisk(string searchFile)
        {
            File.Delete(searchFile);
        }

        private string GetNewNameAppXmlFile()
        {
            string mNameSelectedApp = Path.GetFileNameWithoutExtension(_wrtWriteInfo.SelectedApp.Name);

            return  Options.RepositoryPath + "\\" + mNameSelectedApp + "_" + DateTimeUtils.NowToString() + ".xml";

        }

    }

    public class WriteInfo
    {
        public NameAndIdAndLastReloadTime SelectedApp;
        public IList<NameAndIdAndLastReloadTime> SelectedStories;

        public WriteInfo(NameAndIdAndLastReloadTime selectedApp, IList<NameAndIdAndLastReloadTime> selectedStories)
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
            anotherWriteInfo.SelectedStories = new List<NameAndIdAndLastReloadTime>();
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
        event WriterInfosHandler NewWriteInfoSend;
    }

    public delegate void WriterInfosHandler(object sender, WriteInfoEventArgs e);
}
