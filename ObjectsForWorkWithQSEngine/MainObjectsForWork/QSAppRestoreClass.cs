using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Qlik.Engine;
using UtilClasses.ProgramOptionsClasses;
using UtilClasses.ServiceClasses;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class QsAppRestoreClass : IProgramOptionsEvent, IConnectionStatusInfoEvent, IRestoreInfoEvent, IRestoreStoryFromDisk
    {
        private string RepositoryPath { get; set; }

        private IConnect _location;

        private NameAndIdPair _selectedApp;

        private readonly RestoreInfo _restoreInfo =new ();

        private IApp _app;

        public event NewProgramOptionsHandler NewProgramOptionsSend;
        public event ConnectionStatusInfoHandler NewConnectionStatusInfoSend;
        public event NewRestoreInfoHandler NewRestoreInfoSend;
        public event NewRestoreStoryFromDiskHandler NewRestoreStoryFromDiskSend;

        public QsStoryRestorer QsStoryRestorer { get; }

        public  IList<NameAndIdPair> GetAppsFromStore()
        {
            if (string.IsNullOrEmpty(RepositoryPath))
                return null;
            IList<NameAndIdPair> lstResult = new List<NameAndIdPair>();
            
            foreach (var file in Directory.GetFiles(RepositoryPath, "*.xml"))
            {
                lstResult.Add(GetNameAnIdAppFromFile(file));
            }

            return lstResult;
        }

        private void OnNewRestoreStoryFromDisk(RestoreStoryFromDiskEventArgs e)
        {
            if (this.NewRestoreStoryFromDiskSend != null)
                NewRestoreStoryFromDiskSend(this, e);
        }

        public QsAppRestoreClass(IProgramOptionsEvent optionsEvent , IConnectionStatusInfoEvent connectionEvent, ISelectAppEvent selectAppEvent , IRestoreInfoEvent restoreEvent)
        {
            IProgramOptionsEvent programOptionsEvent = optionsEvent;
            programOptionsEvent.NewProgramOptionsSend += NewProgramOptionsSendReceive;

            IConnectionStatusInfoEvent connectionStatusInfo = connectionEvent;
            connectionStatusInfo.NewConnectionStatusInfoSend += NewConnectionStatusInfoReceived;

            ISelectAppEvent selectApp = selectAppEvent;

            selectApp.NewAppSelectedSend += NewAppSelectedReceive;

            IRestoreInfoEvent restoreInfoEvent = restoreEvent;

            restoreInfoEvent.NewRestoreInfoSend += NewRestoreInfoReceived;

            QsStoryRestorer = new QsStoryRestorer(this, this, this,this);

        }

        private void NewRestoreInfoReceived(object sender, RestoreInfoEventArgs e)
        {
            e.RestoreInfo.Copy(_restoreInfo);
            OnNewRestoreInfoSend(e);
            DoRestore();
        }

        private void DoRestore()
        {
            if (_location == null)
                return;

            if (!Directory.Exists(RepositoryPath))
                return;

            IAppIdentifier appId = _location.GetConnection().AppWithId(_restoreInfo.SelectedApp.Id);

            _app = _location.GetConnection().App(appId);

            string mNameSelectedApp = Path.GetFileNameWithoutExtension(_restoreInfo.SelectedApp.Name);


            string searchFileAppInStore = FindFiles.SearchFileAppInStore(RepositoryPath, mNameSelectedApp, "*.xml");

            var xmlDocument = new XmlDocument();

            xmlDocument.Load(searchFileAppInStore);
            XmlNode root = xmlDocument.DocumentElement;

            if (root != null)
                foreach (XmlNode nodeApp in root.ChildNodes)
                {
                    foreach (XmlNode nodeProperty in nodeApp.ChildNodes)
                    {
                        switch (nodeProperty.Name)
                        {

                            case "stories":
                            {
                                XmlNode stories = nodeProperty;
                                foreach (var searchStory in this._restoreInfo.SelectedStories)
                                {

                                    foreach (XmlNode element in stories.ChildNodes)
                                    {
                                        NameAndIdPair story = new NameAndIdPair();

                                        foreach (XmlNode mStory in element.ChildNodes)
                                        {
                                            switch (mStory.Name)
                                            {
                                                case "storyName":
                                                {
                                                    story.Name = mStory.InnerText;
                                                    break;
                                                }
                                                case "id":
                                                {
                                                    story.Id = mStory.InnerText;
                                                    break;
                                                }
                                            }
                                        }

                                        if (string.CompareOrdinal(story.Id, searchStory.Id) == 0)
                                        {
                                            RestoreStoryFromDiskInfo info = new RestoreStoryFromDiskInfo
                                            {
                                                App = _app,
                                                CurrentApp = _restoreInfo.SelectedApp.Copy(),
                                                CurrentStory = searchStory.Copy(),
                                                StoryFolder = RepositoryPath + "\\" +
                                                              Path.GetFileNameWithoutExtension(searchFileAppInStore) +
                                                              "\\stories\\" + searchStory.Id
                                            };

                                            RestoreStoryFromDiskEventArgs args = new RestoreStoryFromDiskEventArgs(info);

                                            OnNewRestoreStoryFromDisk(args);
                                        }
                                    }
                                }

                                break;
                            }
                        }
                    }
                }

            

            _app.SaveAs("Cov4");




        }

        private void OnNewRestoreInfoSend(RestoreInfoEventArgs e)
        {
            if (this.NewRestoreInfoSend != null)
                NewRestoreInfoSend(this, e);
        }

        private void NewAppSelectedReceive(object sender, SelectedAppEventArgs e)
        {
            _selectedApp = e.SelectedApp.Copy();
        }

        private void NewConnectionStatusInfoReceived(object sender, ConnectionStatusInfoEventArgs e)
        {
            e.ConnectionStatusInfo.Copy(ref this._location);
            OnNewConnectionStatusInfo(e);
        }

        public void OnNewConnectionStatusInfo(ConnectionStatusInfoEventArgs e)
        {
            if (NewConnectionStatusInfoSend != null)
                NewConnectionStatusInfoSend(this, e);
        }

        private void NewProgramOptionsSendReceive(object sender, ProgramOptionsEventArgs e)
        {
            this.RepositoryPath = e.ProgramOptions.RepositoryPath;
            OnNewOptions(e);
        }

        public void OnNewOptions(ProgramOptionsEventArgs e)
        {
            if (NewProgramOptionsSend != null)
                NewProgramOptionsSend(this, e);
        }


        public NameAndIdPair GetNameAnIdAppFromFile(string mFileApp)
        {
            NameAndIdPair result = new NameAndIdPair();
            var xmlDocument = new XmlDocument();

            xmlDocument.Load(mFileApp);

            XmlNode root = xmlDocument.DocumentElement;

            if (root != null)
                foreach (XmlNode nodeApp in root.ChildNodes)
                {
                    foreach (XmlNode nodeProperty in nodeApp.ChildNodes)
                    {
                        switch (nodeProperty.Name)
                        {
                            case "name":
                            {
                                result.Name = nodeProperty.InnerText;

                                break;
                            }
                            case "id":
                            {
                                result.Id = nodeProperty.InnerText;

                                break;
                            }
                        }
                    }
                }

            return result;
        }

        
        public  IList<NameAndIdPair> GetHistoryListForSelectedApp()
        {
            
            string fileApp = FindFiles.SearchFileAppInStore(RepositoryPath, Path.GetFileNameWithoutExtension(_selectedApp.Name),"*.xml");
            
            IList<NameAndIdPair> lstResult = new List<NameAndIdPair>();

            var xmlDocument = new XmlDocument();

            xmlDocument.Load(fileApp);

            XmlNode root = xmlDocument.DocumentElement;

            if (root != null)
                foreach (XmlNode nodeApp in root.ChildNodes)
                {
                    foreach (XmlNode nodeProperty in nodeApp.ChildNodes)
                    {
                        switch (nodeProperty.Name)
                        {
                            
                            case "stories":
                            {
                                XmlNode stories = nodeProperty;

                                foreach (XmlNode element in stories.ChildNodes)
                                {
                                    NameAndIdPair story = new NameAndIdPair();

                                    foreach (XmlNode mStory in element.ChildNodes)
                                    {
                                        switch (mStory.Name)
                                        {
                                            case "storyName":
                                            {
                                                story.Name = mStory.InnerText;
                                                break;
                                            }
                                            case "id":
                                            {
                                                story.Id = mStory.InnerText;
                                                break;
                                            }
                                        }
                                    }

                                    lstResult.Add(story);
                                }

                                break;
                            }
                        }
                    }
                }

            return lstResult;
        }
    }

    public class SelectedAppEventArgs : EventArgs
    {

        public readonly NameAndIdPair SelectedApp;

        
        public SelectedAppEventArgs(NameAndIdPair record)
        {
            SelectedApp = record;
        }
    }

    public interface ISelectAppEvent
    {
        event NewAppSelectedHandler NewAppSelectedSend;
    }

    public delegate void NewAppSelectedHandler(object sender, SelectedAppEventArgs e);

    
    public class RestoreInfo
    {
        public NameAndIdPair SelectedApp;
        public IList<NameAndIdPair> SelectedStories;

        public RestoreInfo(NameAndIdPair selectedApp, IList<NameAndIdPair> selectedStories)
        {
            SelectedApp = selectedApp;
            SelectedStories = selectedStories;
        }

        public RestoreInfo()
        {

        }
        public void Copy(RestoreInfo anotherWriteInfo)
        {
            anotherWriteInfo.SelectedApp = SelectedApp.Copy();
            anotherWriteInfo.SelectedStories = new List<NameAndIdPair>();
            foreach (var story in this.SelectedStories)
            {
                anotherWriteInfo.SelectedStories.Add(story.Copy());
            }
        }

    }

    public class RestoreInfoEventArgs : EventArgs
    {

        public readonly RestoreInfo RestoreInfo;

        //Конструкторы
        public RestoreInfoEventArgs(RestoreInfo record)
        {
            RestoreInfo = record;
        }
    }

    public interface IRestoreInfoEvent
    {
        event NewRestoreInfoHandler NewRestoreInfoSend;
    }

    public delegate void NewRestoreInfoHandler(object sender, RestoreInfoEventArgs e);
}
