using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UtilClasses.ProgramOptionsClasses;
using UtilClasses.ServiceClasses;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class QsAppWriterClass
    {
        private readonly WriteInfo _wrtWriteInfo = new();
        public ProgramOptions Options { get; } = new();

        private LocationObject _location;

        private XmlTextWriter _xmlWriter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writeInfoEvent"></param>
        /// <param name="programOptionsEvent"></param>
        /// <param name="connectionStatusInfoEvent"></param>
        public QsAppWriterClass(IWriteInfoEvent writeInfoEvent ,IProgramOptionsEvent programOptionsEvent, IConnectionStatusInfoEvent connectionStatusInfoEvent)
        {
            IWriteInfoEvent obj = writeInfoEvent;
            obj.NewWriteInfoSend += NewWriteInfoReceive;

            IProgramOptionsEvent obj2 = programOptionsEvent;
            obj2.NewProgramOptionsSend += NewProgramOptionsReceived;

            IConnectionStatusInfoEvent obj3 = connectionStatusInfoEvent;

            obj3.NewConnectionStatusInfoSend += NewConnectionStatusInfoReceived;

        }

        private void NewConnectionStatusInfoReceived(object sender, ConnectionStatusInfoEventArgs e)
        {
            e.ConnectionStatusInfo.Copy(ref this._location);
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


            string  seachFile = FindFiles.SearchFileAppInStore(Options.RepositoryPath, mNameSelectedApp, "*.xml");
            
            if (!string.IsNullOrEmpty(seachFile))
                DeleteHeadierOfAppFromDisk(seachFile);


            _xmlWriter = new XmlTextWriter(GetNewNameAppXmLfile(), null)
            {
                Formatting = Formatting.Indented
            };

            //Write the XML delcaration.
            _xmlWriter.WriteStartDocument();

            //Write the ProcessingInstruction node.
            var pItext = "type='text/xsl' href='application.xsl'";
            _xmlWriter.WriteProcessingInstruction("xml-stylesheet", pItext);

            //Write the DocumentType node.
            _xmlWriter.WriteDocType("book", null, null, "<!ENTITY h 'hardcover'>");

            _xmlWriter.WriteComment("Файл содержит описание приложения "+ _wrtWriteInfo.SelectedApp.Name);

            //Write a root element.
            _xmlWriter.WriteStartElement("application");

            
                _xmlWriter.WriteStartElement("properties");

                    _xmlWriter.WriteElementString("name", _wrtWriteInfo.SelectedApp.Name);

            
                    _xmlWriter.WriteElementString("id", _wrtWriteInfo.SelectedApp.Id);

                    _xmlWriter.WriteStartElement("stories");


                        foreach (var story in this._wrtWriteInfo.SelectedStories)
                        {
                            _xmlWriter.WriteStartElement("story");
                                _xmlWriter.WriteElementString("storyName", story.Name); 
                                _xmlWriter.WriteElementString("id",story.Id);
                            _xmlWriter.WriteEndElement();
                        }

                    _xmlWriter.WriteEndElement();
                
                _xmlWriter.WriteEndElement();

            _xmlWriter.WriteEndElement();


            _xmlWriter.WriteEndDocument();

            _xmlWriter.Flush();
            _xmlWriter.Close();

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
