using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Qlik.Sense.Client.Storytelling;
using UtilClasses.ProgramOptionsClasses;
using UtilClasses.ServiceClasses;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class QsAppWriterClass
    {
        private readonly WriteInfo _wrtWriteInfo = new WriteInfo();
        public ProgramOptions Options { get; } = new ProgramOptions();

        public LocationObject _Location;

        private XmlTextWriter xmlWriter;

        public QsAppWriterClass(IWriteInfoEvent form1 ,IProgramOptionsEvent form2, IConnectionStatusInfoEvent form3)
        {
            IWriteInfoEvent obj = form1;
            obj.NewWriteInfoSend += NewWriteInfoReceive;

            IProgramOptionsEvent obj2 = form2;
            obj2.NewProgramOptionsSend += NewProgramOptionsReceived;

            IConnectionStatusInfoEvent obj3 = form3;

            obj3.NewConnectionStatusInfoSend += NewConnectionStatusInfoReceived;

        }

        private void NewConnectionStatusInfoReceived(object sender, ConnectionStatusInfoEventArgs e)
        {
            e._ConnectionStatusInfo.Copy(ref this._Location);
        }

        private void DoWrite()
        {
            if (_Location == null)
                return;
            
            if (!Directory.Exists(Options.RepositoryPath))
            {

                return;
            }

            string mNameSelectedApp = Path.GetFileNameWithoutExtension(_wrtWriteInfo.SelectedApp.Name);


            string  seachFile = FindFiles.SearchFileAppInStore(Options.RepositoryPath, mNameSelectedApp, "*.xml");
            
            if (!string.IsNullOrEmpty(seachFile))
                DeleteHeadierOfAppFromDisk(seachFile);


            xmlWriter = new XmlTextWriter(GetNewNameAppXmLfile(), null)
            {
                Formatting = Formatting.Indented
            };

            //Write the XML delcaration.
            xmlWriter.WriteStartDocument();

            //Write the ProcessingInstruction node.
            var PItext = "type='text/xsl' href='application.xsl'";
            xmlWriter.WriteProcessingInstruction("xml-stylesheet", PItext);

            //Write the DocumentType node.
            xmlWriter.WriteDocType("book", null, null, "<!ENTITY h 'hardcover'>");

            xmlWriter.WriteComment("Файл содержит описание приложения "+ _wrtWriteInfo.SelectedApp.Name);

            //Write a root element.
            xmlWriter.WriteStartElement("application");

            
                xmlWriter.WriteStartElement("properties");

                    xmlWriter.WriteElementString("name", _wrtWriteInfo.SelectedApp.Name);

            
                    xmlWriter.WriteElementString("id", _wrtWriteInfo.SelectedApp.Id);

                    xmlWriter.WriteStartElement("stories");


                        foreach (var story in this._wrtWriteInfo.SelectedStories)
                        {
                            xmlWriter.WriteStartElement("story");
                                xmlWriter.WriteElementString("storyName", story.Name); 
                                xmlWriter.WriteElementString("id",story.Id);
                            xmlWriter.WriteEndElement();
                        }

                    xmlWriter.WriteEndElement();
                
                xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();


            xmlWriter.WriteEndDocument();

            xmlWriter.Flush();
            xmlWriter.Close();

        }


        private void DeleteHeadierOfAppFromDisk(string seachFile)
        {
            File.Delete(seachFile);
        }

        private string GetNewNameAppXmLfile()
        {
            string mNameSelectedApp = Path.GetFileNameWithoutExtension(_wrtWriteInfo.SelectedApp.Name);

            return  Options.RepositoryPath + "\\" + mNameSelectedApp + "_" + DateTimeUtlis.NowToString() + ".xml";

        }

        private void NewProgramOptionsReceived(object sender, ProgramOptionsEventArgs e)
        {
            e.ProgramOptions.Copy(this.Options);
        }

        private void NewWriteInfoReceive(object sender, WriteInfoEventArgs e)
        {
            e.WriteInfo.Copy(_wrtWriteInfo);
            DoWrite();
        }
    }

    public class WriteInfo
    {
        public NameAndIdPair SelectedApp;
        public IList<NameAndIdPair> SelectedStories;

        public WriteInfo(NameAndIdPair selectedApp, IList<NameAndIdPair> selectedStories)
        {
            this.SelectedApp = selectedApp;
            this.SelectedStories = selectedStories;
        }

        public WriteInfo()
        {

        }
        public void Copy(WriteInfo anotherWriteInfo)
        {
            anotherWriteInfo.SelectedApp = this.SelectedApp.Copy();
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
