using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UtilClasses.ProgramOptionsClasses;
using UtilClasses.ServiceClasses;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class QsAppRestoreClass
    {
        private string RepositoryPath { get; set; }

        private IConnect _location;

        private NameAndIdPair _selectedApp;

        private RestoreInfo _restoreInfo =new ();

        /// <summary>
        // ReSharper disable once CommentTypo
        /// Читает все txt файлы в репозитарии
        /// и возвращает список пар коротких имен файлов и их полных идентификаторов
        /// </summary>
        /// <returns>список пар коротких имен файлов и их полных идентификаторов</returns>
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

        }

        private void NewRestoreInfoReceived(object sender, RestoreInfoEventArgs e)
        {
            e.RestoreInfo.Copy(_restoreInfo);
        }

        private void NewAppSelectedReceive(object sender, SelectedAppEventArgs e)
        {
            this._selectedApp = e.SelectedApp.Copy();
        }

        private void NewConnectionStatusInfoReceived(object sender, ConnectionStatusInfoEventArgs e)
        {
            e.ConnectionStatusInfo.Copy(ref this._location);
        }

        private void NewProgramOptionsSendReceive(object sender, ProgramOptionsEventArgs e)
        {
            this.RepositoryPath = e.ProgramOptions.RepositoryPath;
        }

        /// <summary>
        /// Читает короткое имя приложения  и его полный идентификатор
        /// из txt файла с меткой времени
        /// </summary>
        /// <param name="mFileApp"></param>
        /// <returns>Пару с коротким именем файла и его полным идентифкатором</returns>
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

        /// <summary>
        /// Функция возвращает список пар
        /// историй с наимиенованиями и их полными иеднтафикаторами
        /// </summary>
        /// <returns>Список пар коротких имен историй и и их идентификторов</returns>
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

        //Конструкторы
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
        event NewRsstoreInfosHandler NewRestoreInfoSend;
    }

    public delegate void NewRsstoreInfosHandler(object sender, RestoreInfoEventArgs e);
}
