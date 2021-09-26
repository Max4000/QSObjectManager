using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Qlik.Engine;
using UtilClasses.ProgramOptionsClasses;
using UtilClasses.ServiceClasses;
// ReSharper disable StringLiteralTypo

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class QsAppRestoreClass :   IRestoreInfoEvent, IRestoreStoryFromDisk,IProgramOptionsEvent , IResultDigest
    {
        //private string RepositoryPath { get; set; }

        private IConnect _location;

        private NameAndIdAndLastReloadTime _selectedApp;

        private readonly RestoreInfo _restoreInfo =new ();

        //private IApp _appSource;
        private IApp _appTarget;

        private readonly ProgramOptions _programOptions = new();

        private string _folderForAddContent;

        
        public event RestoreInfoHandler NewRestoreInfoSend;
        public event RestoreStoryFromDiskHandler NewRestoreStoryFromDiskSend;
        public event ProgramOptionsHandler NewProgramOptionsSend;
        public event ResultDigestInfoHandler NewResultDigestInfoSend;

        private void OnNewProgramOptions(ProgramOptionsEventArgs e)
        {
            if (NewProgramOptionsSend != null)
                NewProgramOptionsSend(this, e);
        }

        private void OnNewResultDigestInfo(ResultDigestInfoEventArgs e)
        {
            if (NewResultDigestInfoSend != null)
                NewResultDigestInfoSend(this, e);
        }

        public  IList<NameAndIdAndLastReloadTime> GetAppsFromStore()
        {
            if (string.IsNullOrEmpty(_programOptions.RepositoryPath))
                return null;
            IList<NameAndIdAndLastReloadTime> lstResult = new List<NameAndIdAndLastReloadTime>();
            
            foreach (var file in Directory.GetFiles(_programOptions.RepositoryPath, "*.xml"))
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
            programOptionsEvent.NewProgramOptionsSend += ProgramOptionsSendReceived;

            IConnectionStatusInfoEvent connectionStatusInfo = connectionEvent;
            connectionStatusInfo.NewConnectionStatusInfoSend += ConnectionStatusInfoReceived;

            ISelectAppEvent selectApp = selectAppEvent;

            selectApp.NewAppSelectedSend += AppSelectedReceived;

            IRestoreInfoEvent restoreInfoEvent = restoreEvent;

            restoreInfoEvent.NewRestoreInfoSend += RestoreInfoReceived;

            var unused = new QsStoryRestorer(this,this,this);
        }

        private void RestoreInfoReceived(object sender, RestoreInfoEventArgs e)
        {
            e.RestoreInfo.Copy(_restoreInfo);
            OnNewRestoreInfoSend(e);
            DoRestore();
        }

        private string CreateOrEmptyFolderForAddContent()
        {
            string user = UtilClasses.CurrentUser.GetExplorerUser();

            int pos = user.LastIndexOf("\\", StringComparison.Ordinal);

            string folderForAddContent = "C:\\Users\\" + user.Substring(pos + 1) +
                                         "\\Desktop\\ПАПКА_С_НОВЫМ_КОНТЕНТОМ_ДЛЯ_" + _restoreInfo.TargetApp.Name;
            if (Directory.Exists(folderForAddContent))
            {
                foreach (var file in Directory.GetFiles(folderForAddContent))
                {
                    File.Delete(file);
                }
                Directory.Delete(folderForAddContent);
            }

            Directory.CreateDirectory(folderForAddContent);
            

            return folderForAddContent;

        }

        private void DoRestore()
        {


            if (_location == null)
                return;

            if (!Directory.Exists(_programOptions.RepositoryPath))
                return;


            _folderForAddContent = CreateOrEmptyFolderForAddContent();
            

            try
            {

                //IAppIdentifier sourceAppId = _location.GetConnection().AppWithId(_restoreInfo.SourceApp.Id);
                IAppIdentifier targetAppId = _location.GetConnection().AppWithId(_restoreInfo.TargetApp.Id);


                //_appSource = _location.GetConnection().App(sourceAppId);
                
                _appTarget = _location.GetConnection().App(targetAppId);
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось подключиться к приложению", ex);
            }

            string mNameSelectedApp = Path.GetFileNameWithoutExtension(_restoreInfo.SourceApp.Name);


            string searchFileAppInStore = FindFiles.SearchFileAppInStore(_programOptions.RepositoryPath, mNameSelectedApp, "*.xml");

            string appcontentFolder = Path.GetFileNameWithoutExtension(searchFileAppInStore) + "\\" + "appcontent";
            
            string defaultFolder = Path.GetFileNameWithoutExtension(searchFileAppInStore) + "\\" + "default";

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
                                        NameAndIdAndLastReloadTime story = new NameAndIdAndLastReloadTime();

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
                                                SourceApp = null,
                                                TargetApp = _appTarget,

                                                CurrentAppSource = _restoreInfo.SourceApp.Copy(),
                                                CurrentAppTarget = _restoreInfo.TargetApp.Copy(),

                                                AppContentFolder = appcontentFolder,
                                                DefaultFolder = defaultFolder,
                                               
                                                CurrentStory = searchStory.Copy(),
                                                StoryFolder = _programOptions.RepositoryPath + "\\" +
                                                              Path.GetFileNameWithoutExtension(searchFileAppInStore) +
                                                              "\\stories\\" + searchStory.Id,
                                                FolderNameWithAddContent = _folderForAddContent,
                                                AddContentList = new List<string>()
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

            int ik = 0;

            string name = Path.GetFileNameWithoutExtension(_restoreInfo.SourceApp.Name);
            string tryName;

            //StaticContentList lys = _appTarget.GetLibraryContent("appcontent");


            while (true)
            {
                ik++;

                if (_programOptions.IsServer())
                    tryName = name + " (copy" + ik.ToString() + ")";
                else
                {
                    tryName = name + " (copy" + ik.ToString() + ")" + ".qvf";
                }

                bool found = false;

                foreach (var pair in Utils.GetApps(_location.GetConnection()))
                {
                    if (string.CompareOrdinal(tryName, pair.Name) == 0)
                        found = true;
                }

                if (!found)
                    break;
            }
            
            
            _appTarget.DoSave();
            
            
            _appTarget.Dispose();
            
            
            _appTarget = null;



        }

        private void OnNewRestoreInfoSend(RestoreInfoEventArgs e)
        {
            if (this.NewRestoreInfoSend != null)
                NewRestoreInfoSend(this, e);
        }

        private void AppSelectedReceived(object sender, SelectedAppEventArgs e)
        {
            _selectedApp = e.SelectedApp.Copy();
        }

        private void ConnectionStatusInfoReceived(object sender, ConnectionStatusInfoEventArgs e)
        {
            this._location = e.ConnectionStatusInfo.Copy();
        }


        private void ProgramOptionsSendReceived(object sender, ProgramOptionsEventArgs e)
        {
            //this.RepositoryPath = e.ProgramOptions.RepositoryPath;
            e.ProgramOptions.Copy(_programOptions);
            OnNewProgramOptions(e);

        }





        public NameAndIdAndLastReloadTime GetNameAnIdAppFromFile(string mFileApp)
        {
            NameAndIdAndLastReloadTime result = new NameAndIdAndLastReloadTime();
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
                            case "LastReloadTime":
                            {
                                result.LastReloadTime = nodeProperty.InnerText;

                                break;
                            }
                        }
                    }
                }

            return result;
        }

        
        public  IList<NameAndIdAndLastReloadTime> GetHistoryListForSelectedApp()
        {
            
            string fileApp = FindFiles.SearchFileAppInStore(_programOptions.RepositoryPath, Path.GetFileNameWithoutExtension(_selectedApp.Name),"*.xml");
            
            IList<NameAndIdAndLastReloadTime> lstResult = new List<NameAndIdAndLastReloadTime>();

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
                                    NameAndIdAndLastReloadTime story = new NameAndIdAndLastReloadTime();

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

        public readonly NameAndIdAndLastReloadTime SelectedApp;

        
        public SelectedAppEventArgs(NameAndIdAndLastReloadTime record)
        {
            SelectedApp = record;
        }
    }

    public interface ISelectAppEvent
    {
        event AppSelectedHandler NewAppSelectedSend;
    }

    public delegate void AppSelectedHandler(object sender, SelectedAppEventArgs e);

    
    public class RestoreInfo
    {
        public NameAndIdAndLastReloadTime SourceApp;
        public IList<NameAndIdAndLastReloadTime> SelectedStories;
        public NameAndIdAndLastReloadTime TargetApp;


        public RestoreInfo(NameAndIdAndLastReloadTime sourceApp, IList<NameAndIdAndLastReloadTime> selectedStories, NameAndIdAndLastReloadTime targetApp)
        {
            SourceApp = sourceApp;
            TargetApp = targetApp;
            SelectedStories = selectedStories;
        }

        public RestoreInfo()
        {

        }
        public void Copy(RestoreInfo anotherWriteInfo)
        {
            anotherWriteInfo.TargetApp = TargetApp.Copy();
            anotherWriteInfo.SourceApp = SourceApp.Copy();
            anotherWriteInfo.SelectedStories = new List<NameAndIdAndLastReloadTime>();
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
        event RestoreInfoHandler NewRestoreInfoSend;
    }

    public delegate void RestoreInfoHandler(object sender, RestoreInfoEventArgs e);
}
