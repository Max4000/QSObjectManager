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

        private IList<string> _listAddcontent;


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
                if (Directory.Exists(folderForAddContent + "\\default"))
                {
                    foreach (var file in Directory.GetFiles(folderForAddContent + "\\default"))
                    {
                        File.Delete(file);
                    }
                    Directory.Delete(folderForAddContent + "\\default");
                }

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
            _listAddcontent = new List<string>();

            try
            {
                IAppIdentifier targetAppId = _location.GetConnection().AppWithId(_restoreInfo.TargetApp.Id);
                
                _appTarget = _location.GetConnection().App(targetAppId);
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось подключиться к приложению", ex);
            }

            string mNameSelectedApp = Path.GetFileNameWithoutExtension(_restoreInfo.SourceApp.Name);


            string searchFileAppInStore = FindFiles.SearchFileAppInStore(_programOptions.RepositoryPath, mNameSelectedApp, "*.xml");

            string appContentFolder = Path.GetFileNameWithoutExtension(searchFileAppInStore) + "\\" + "appcontent";
            
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

                                        if (string.CompareOrdinal(story.Id,
                                                searchStory.Id) ==
                                            0)
                                        {
                                            string mStoryFolder = _programOptions.RepositoryPath +
                                                                  "\\" +
                                                                  Path.GetFileNameWithoutExtension(
                                                                      searchFileAppInStore) +
                                                                  "\\stories\\" +
                                                                  searchStory.Id;

                                            RestoreStoryFromDiskInfo info = new RestoreStoryFromDiskInfo
                                            {

                                                TargetApp = _appTarget,

                                                CurrentAppSource = _restoreInfo.SourceApp.Copy(),
                                                CurrentAppTarget = _restoreInfo.TargetApp.Copy(),

                                                AppContentFolder = appContentFolder,
                                                DefaultFolder = defaultFolder,

                                                CurrentStory = searchStory.Copy(),
                                                StoryFolder = mStoryFolder,
                                                FolderNameWithAddContent = _folderForAddContent,
                                                AddContentList = _listAddcontent
                                            };

                                            RestoreStoryFromDiskEventArgs args =
                                                new RestoreStoryFromDiskEventArgs(info);

                                            OnNewRestoreStoryFromDisk(args);
                                        }
                                    }
                                }

                                break;
                            }
                        }
                    }
                }

            _appTarget.DoSave();

            _appTarget.Dispose();
          
            OnNewResultDigestInfo(new ResultDigestInfoEventArgs(new ResultDigestInfo()
            {
                AddContentList = _listAddcontent,
                FolderWithAddContent = _folderForAddContent
            }));

        }

        private void OnNewRestoreInfoSend(RestoreInfoEventArgs e)
        {
            if (NewRestoreInfoSend != null)
                NewRestoreInfoSend(this,
                    e);
        }

        private void AppSelectedReceived(object sender,
            SelectedAppEventArgs e)
        {
            _selectedApp = e.SelectedApp.Copy();
        }

        private void ConnectionStatusInfoReceived(object sender,
            ConnectionStatusInfoEventArgs e)
        {
            _location = e.ConnectionStatusInfo.Copy();
        }

        private void ProgramOptionsSendReceived(object sender,
            ProgramOptionsEventArgs e)
        {

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


        public IList<NameAndIdAndLastReloadTime> GetHistoryListForSelectedApp()
        {

            string fileApp = FindFiles.SearchFileAppInStore(_programOptions.RepositoryPath,
                Path.GetFileNameWithoutExtension(_selectedApp.Name),
                "*.xml");

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

}
