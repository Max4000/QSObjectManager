using System.Collections.Generic;
using System.IO;
using System.Text;
using UtilClasses.ProgramOptionsClasses;
using UtilClasses.ServiceClasses;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class StoreAppsInfoClass
    {
        private string RepositoryPath { get; set; }

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
            
            foreach (var file in Directory.GetFiles(RepositoryPath, "*.txt"))
            {
                lstResult.Add(GetNameAnIdAppFromFile(file));
            }

            return lstResult;
        }

        public StoreAppsInfoClass(IProgramOptionsEvent optionsEvent)
        {
            IProgramOptionsEvent programOptionsEvent = optionsEvent;
            programOptionsEvent.NewProgramOptionsSend += ProgramOptionsEvent_NewProgramOptionsSend;
        }

        private void ProgramOptionsEvent_NewProgramOptionsSend(object sender, ProgramOptionsEventArgs e)
        {
            this.RepositoryPath = e.ProgramOptions.RepositoryPath;
        }

        /// <summary>
        /// Читает короткое имя приложения  и его полный идентификатор
        /// из txt файла с меткой времени
        /// </summary>
        /// <param name="mFileApp"></param>
        /// <returns>Пару с коротким именем файла и его полным идентифкатором</returns>
        public  NameAndIdPair GetNameAnIdAppFromFile(string mFileApp)
        {

            Encoding utf8 = Encoding.GetEncoding(65001);
            using StreamReader readerFileApp = new StreamReader(mFileApp, utf8);

            readerFileApp.ReadLine();
            
            var appName = readerFileApp.ReadLine()?.Split('=')[1];
            var appId = readerFileApp.ReadLine()?.Split('=')[1];
            

            readerFileApp.Close();
            readerFileApp.Dispose();

            return new NameAndIdPair(appName, appId);
        }

        /// <summary>
        /// Функция возвращает список пар
        /// историй с наимиенованиями и их полными иеднтафикаторами
        /// </summary>
        /// <param name="nameid">Пара короткое имя приложения и его полный идентификатор</param>
        /// <returns>Список пар коротких имен историй и и их идентификторов</returns>
        public  IList<NameAndIdPair> GetHistoryListForSelectedApp(NameAndIdPair nameid)
        {
            string file = Path.GetFileNameWithoutExtension(nameid.Name);
            string fileApp = FindFiles.SearchFileAppInStore(RepositoryPath, file,"*.txt");
            Path.GetFileNameWithoutExtension(fileApp);

            Encoding utf8 = Encoding.GetEncoding(65001);

            IList<NameAndIdPair> lstResult = new List<NameAndIdPair>();

            using StreamReader readerFileApp = new StreamReader(fileApp, utf8);
            string line;

            readerFileApp.ReadLine();
            readerFileApp.ReadLine();
            readerFileApp.ReadLine();

            int i = 0;

            IList<string> ms = new List<string>();
            while ((line = readerFileApp.ReadLine()) != null)
            {
                i++;


                switch (i)
                {
                    case 2:
                    {
                        ms.Add(line.Split("=")[1]);
                        break;
                    }
                    case 3:
                    {
                        ms.Add(line.Split("=")[1]);
                        break;
                    }
                    case 4:
                    {
                        lstResult.Add(new NameAndIdPair(ms[0],ms[1]));

                        ms.Clear();

                        i = 0;

                        break;
                    }


                }

            }

            readerFileApp.Close();
            readerFileApp.Dispose();

           
            return lstResult;
        }
    }
}
