using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Qlik.Engine;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class ImportUtilClass
    {
        public static void ImportStorysFromAppQs(string rootPathStorys, ILocation location, NameAndIdPair selectedApp,
            IList<NameAndIdPair> storyList)
        {
            if (!Directory.Exists(rootPathStorys))
            {
                
                return;
            }

            string mNameSelectedApp = Path.GetFileNameWithoutExtension(selectedApp.Name);

            string nowString = DateTimeUtlis.NowToString();

            string mFileApp = rootPathStorys + @"\" + mNameSelectedApp +"_"+ nowString + ".txt";

            string seachFile = SeachFileAppInStore(rootPathStorys, mNameSelectedApp);
            
            if (seachFile.Length > 0)
            {
                DeleteHieaderOfAppFromDisk(rootPathStorys, seachFile);
            }
            WriteHiderOfAppToFile(rootPathStorys,location,mFileApp,selectedApp,storyList);
            
        }


        public static string SeachFileAppInStore(string rootPathStorys,string selectedAppNmae)
        {
            foreach (var file in Directory.GetFiles(rootPathStorys,"*.txt"))
            {

                var mFile = Path.GetFileNameWithoutExtension(file);

                var mFile2 = mFile.Substring(0, mFile.Length - 18);

                if (String.CompareOrdinal(selectedAppNmae, mFile2) == 0)
                {
                    return file;
                }

            }

            return "";

        }


        private static void DeleteHieaderOfAppFromDisk(string rootPathStory, string mFileApp)
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


            if (Directory.Exists(rootPathStory+ "\\"+ mFile + "\\" + "UserStories"))
                Directory.Delete(rootPathStory + "\\" + mFile + "\\" + "UserStories");

            if (Directory.Exists(rootPathStory + "\\" + mFile))
                Directory.Delete(rootPathStory + "\\" + mFile);


            File.Delete(mFileApp);



        }


/*
        private static void WriteHiderOfAppToFile(string rootPathStorys, string fileapp, NameAndIdPair selectedApp,
            IList<NameAndIdPair> storyList)
        {
            WriteHiderOfAppToFile(rootPathStorys, null, fileapp, selectedApp, storyList);
        }
*/

        // ReSharper disable once UnusedParameter.Local
        private static void WriteHiderOfAppToFile(string rootPathStorys, ILocation location, string fileapp, NameAndIdPair selectedApp,
            IList<NameAndIdPair> storyList)
        {
            if (!File.Exists(fileapp))
            {

                var fileComments = rootPathStorys +"\\"+ Path.GetFileNameWithoutExtension(fileapp)+".cmt";
                
                using var writerHeaderApp = new AppEntryWriter(fileapp);
                using var writerCmt = new AppEntryWriter(fileComments);
                writerHeaderApp.Writer.WriteLine("[App]");
                writerHeaderApp.Writer.WriteLine("app_name=" + selectedApp.Name);
                writerHeaderApp.Writer.WriteLine("app_id=" + selectedApp.Id);

                int iStory = 0;

                string mNameSelectedApp = Path.GetFileNameWithoutExtension(fileapp);

                string mStorysHistoryPath = rootPathStorys + "\\" + mNameSelectedApp + "\\"+ "UserStories";

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

    public class AppEntryWriter : IDisposable
    {

        public StreamWriter Writer { get; }

        public AppEntryWriter(string mfile)
        {
            Writer = new StreamWriter(new FileStream(mfile, FileMode.Create),
                encoding: Encoding.UTF8);
        }

        ~AppEntryWriter()
        {
            Dispose(false);
        }

        private void ReleaseUnmanagedResources()
        {
            // уЕшьTssODO release unmanaged resources here
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                Writer?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
