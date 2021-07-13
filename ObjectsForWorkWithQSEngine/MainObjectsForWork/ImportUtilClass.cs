using System.Collections.Generic;
using System.IO;
using System.Text;
using Qlik.Engine;
using UtilClasses;
using UtilClasses.ProgramOptionsClasses;
using UtilClasses.ServiceClasses;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class ImportUtilClass
    {
        private readonly ProgramOptions _programOptions = new();

        public ImportUtilClass(IProgramOptionsEvent optionsEvent)
        {
            IProgramOptionsEvent programOptionsEvent = optionsEvent;
            programOptionsEvent.NewProgramOptionsSend += ProgramOptionsEvent_NewProgramOptionsSend;
        }

        private void ProgramOptionsEvent_NewProgramOptionsSend(object sender, ProgramOptionsEventArgs e)
        {
            e.ProgramOptions.Copy(this._programOptions);
        }

        /// <summary>
        /// Получает в качестве аргументов папку с хранилищем, ссылку на объект Dev Hub,
        /// пару с коротким именем приложения и его полным идентифкатором
        /// и список пар пользовательских историй которые надо сохранить
        /// находит в репозитарии информаицю об этом приложении удалает ее если она есть
        /// и сохраняет вновь всю информацию на диске
        /// </summary>
        /// <param name="location">ссылка на объект Dev Hub</param>
        /// <param name="selectedApp">пару с коротким именем приложения и его полным идентифкатором</param>
        /// <param name="storyList">список пар пользовательских историй</param>
        public  void ImportStorysFromAppQs( ILocation location, NameAndIdPair selectedApp,
            IList<NameAndIdPair> storyList)
        {
            if (!Directory.Exists(_programOptions.RepositoryPath))
            {
                
                return;
            }

            string mNameSelectedApp = Path.GetFileNameWithoutExtension(selectedApp.Name);

            string nowString = DateTimeUtlis.NowToString();

            string mFileApp = _programOptions.RepositoryPath + @"\" + mNameSelectedApp +"_"+ nowString + ".txt";

            string seachFile = "";
            try
            {
                seachFile = FindFiles.SearchFileAppInStore(_programOptions.RepositoryPath, mNameSelectedApp,"*.txt");
            }
            catch
            {
                // ignored
            }


            if (seachFile.Length > 0)
            {
                DeleteHeadierOfAppFromDisk(seachFile);
            }
            WriteHiderOfAppToFile( location, mFileApp, selectedApp, storyList);
            
        }

       

        /// <summary>
        /// Получает в качестве аргументов полное имя файла приложения в хранилище с меткой
        /// времени, папку с репозитарием, читает инфорамацию в файле и удаляет все пользовательские
        /// истории из хранилища после чего удаляет сам txt файл с меткой времени из хранилища
        /// </summary>
        /// <param name="mFileApp">полное наименование файла приложения с меткой времени в хрнаилище</param>
        private  void DeleteHeadierOfAppFromDisk(string mFileApp)
        {

            Encoding utf8 = Encoding.GetEncoding(65001);
            using StreamReader readerFileApp = new StreamReader(mFileApp, utf8);
            string line;

            readerFileApp.ReadLine();
            readerFileApp.ReadLine();
            readerFileApp.ReadLine();

            int i = 0;
            while ((line = readerFileApp.ReadLine()) != null)
            {


                i++;

                if (i % 4 == 0)
                {
                    var names = line.Split('=');
                    if (Directory.Exists(names[1]))
                    {
                        foreach (var file in Directory.GetFiles(names[1]))
                        {
                            File.Delete(file);
                        }
                        Directory.Delete(names[1]);
                    }
                }

            }

            readerFileApp.Close();
            readerFileApp.Dispose();

            string mFile = Path.GetFileNameWithoutExtension(mFileApp);


            if (Directory.Exists(_programOptions.RepositoryPath+ "\\"+ mFile + "\\" + "UserStories"))
                Directory.Delete(_programOptions.RepositoryPath + "\\" + mFile + "\\" + "UserStories");

            if (Directory.Exists(_programOptions.RepositoryPath + "\\" + mFile))
                Directory.Delete(_programOptions.RepositoryPath + "\\" + mFile);


            File.Delete(mFileApp);



        }


/*
        private static void WriteHiderOfAppToFile(string rootPathStories, string fileapp, NameAndIdPair selectedApp,
            IList<NameAndIdPair> storyList)
        {
            WriteHiderOfAppToFile(rootPathStories, null, fileapp, selectedApp, storyList);
        }
*/

        // ReSharper disable once UnusedParameter.Local
        /// <summary>
        /// Получает в качестве параметров путь на хранилище, ссылку на объект связи с Dev Hub,
        /// новое полное имя с меткой времени txt файла, пару короткое имя приложения и его идентификатор
        /// и список пар историй которые надо сохранить на диске
        /// и сохранаяет всю информацию на диске
        /// </summary>
        /// <param name="location">ссылку на объект Dev Hub</param>
        /// <param name="fileapp">полное новое наименование с меткой времени файла инфорамции о приложении</param>
        /// <param name="selectedApp">пара корокоем файла приложения и его полный идентификатор</param>
        /// <param name="storyList">список пар историй</param>
        private void WriteHiderOfAppToFile(ILocation location, string fileapp, NameAndIdPair selectedApp,
            IList<NameAndIdPair> storyList)
        {
            if (!File.Exists(fileapp))
            {

                var fileComments = _programOptions.RepositoryPath +"\\"+ Path.GetFileNameWithoutExtension(fileapp)+".cmt";
                
                using var writerHeaderApp = new AppEntryWriter(fileapp);
                using var writerCmt = new AppEntryWriter(fileComments);
                writerHeaderApp.Writer.WriteLine("[App]");
                writerHeaderApp.Writer.WriteLine("app_name=" + selectedApp.Name);
                writerHeaderApp.Writer.WriteLine("app_id=" + selectedApp.Id);

                int iStory = 0;

                string mNameSelectedApp = Path.GetFileNameWithoutExtension(fileapp);

                string mStorysHistoryPath = _programOptions.RepositoryPath + "\\" + mNameSelectedApp + "\\"+ "UserStories";

                Directory.CreateDirectory(mStorysHistoryPath);

                foreach (var story in storyList)
                {
                    iStory++;
                    writerHeaderApp.Writer.WriteLine("[Story" + iStory + "]");
                    writerHeaderApp.Writer.WriteLine("story_name=" + story.Name);
                    writerCmt.Writer.WriteLine("story_name " + story.Name);
                    writerCmt.Writer.WriteLine("Сохранено " + DateTimeUtlis.NowToNormalString());

                    writerHeaderApp.Writer.WriteLine("story_id=" + story.Id);
                    writerHeaderApp.Writer.WriteLine("story_path="+ mStorysHistoryPath + "\\" + story.Id);

                    Directory.CreateDirectory(mStorysHistoryPath + "\\" + story.Id);
                    
                    using var writerHist = new AppEntryWriter(mStorysHistoryPath + "\\" + story.Id+"\\"+ story.Id +".txt");
                    
                    writerHist.Writer.WriteLine("[Story" + iStory + "]");
                    writerHist.Writer.WriteLine("story_name=" + story.Name);
                    writerHist.Writer.WriteLine("story_id=" + story.Id);
                    writerHist.Writer.WriteLine("story_path=" + mStorysHistoryPath + "\\" + story.Id);
                }
            }
        }


    }

    
}
