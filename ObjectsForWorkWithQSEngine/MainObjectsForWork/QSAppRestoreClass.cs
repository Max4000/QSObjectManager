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

        private LocationObject _location;

        private NameAndIdPair _selectedApp;

        /// <summary>
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

        public QsAppRestoreClass(IProgramOptionsEvent optionsEvent , IConnectionStatusInfoEvent connectionEvent, ISelectAppEvent selectAppEvent )
        {
            IProgramOptionsEvent programOptionsEvent = optionsEvent;
            programOptionsEvent.NewProgramOptionsSend += NewProgramOptionsSendReceive;

            IConnectionStatusInfoEvent connectionStatusInfo = connectionEvent;
            connectionStatusInfo.NewConnectionStatusInfoSend += NewConnectionStatusInfoReceived;

            ISelectAppEvent selectApp = selectAppEvent;

            selectApp.NewAppSelectedSend += NewAppSelectedReceive;

        }

        private void NewAppSelectedReceive(object sender, SelectedAppEventArgs e)
        {
            this._selectedApp = e.SlectedApp.Copy();
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
                            //case "name":
                            //{
                            //    break;
                            //}
                            //case "id":
                            //{
                            //    break;
                            //}
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

        public readonly NameAndIdPair SlectedApp;

        //Конструкторы
        public SelectedAppEventArgs(NameAndIdPair record)
        {
            SlectedApp = record;
        }
    }

    public interface ISelectAppEvent
    {
        event NewAppSelectedHandler NewAppSelectedSend;
    }

    public delegate void NewAppSelectedHandler(object sender, SelectedAppEventArgs e);
}
