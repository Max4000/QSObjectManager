using System;
using System.Xml;
using UtilClasses.ProgramOptionsClasses;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class QsStoryRestorer :IProgramOptionsEvent, IRestoreInfoEvent
    {
        public ProgramOptions Options { get; } = new();
        private IConnect _location;

        private WriteStoryToDiskInfo _storyToDiskInfo = new();
        
        RestoreInfo _restoreInfo = new();

        public event NewProgramOptionsHandler NewProgramOptionsSend;
        public event NewRsstoreInfosHandler NewRestoreInfoSend;

        public QsStoryRestorer(IProgramOptionsEvent programOptions,
            IConnectionStatusInfoEvent connectionStatusInfoEvent, IRestoreInfoEvent restoreInfo)
        {
            IProgramOptionsEvent programOptionsEvent = programOptions;

            programOptionsEvent.NewProgramOptionsSend += NewProgramOptionsReceived;



        }

        private void NewProgramOptionsReceived(object sender, ProgramOptionsEventArgs e)
        {
            throw new NotImplementedException();
        }
    }

    public class RestoreStoryFromDiskInfo
    {
        public NameAndIdPair CurrentApp;
        public NameAndIdPair CurrentStory;
        public string StoreFolder;
        public XmlTextWriter CurrentXmlTextWriter;

        public void Copy(ref WriteStoryToDiskInfo anotherInfo)
        {
            anotherInfo.CurrentApp = CurrentApp.Copy();
            anotherInfo.CurrentStory = CurrentStory.Copy();
            anotherInfo.StoreFolder = StoreFolder;
            anotherInfo.CurrentXmlTextWriter = CurrentXmlTextWriter;
        }

    }

    public class RestoreStoryFromDiskEventArgs : EventArgs
    {

        public readonly RestoreStoryFromDiskInfo RestoreInfo;

        //Конструкторы
        public RestoreStoryFromDiskEventArgs(RestoreStoryFromDiskInfo record)
        {
            RestoreInfo = record;
        }
    }

    public interface IRestoreStoryFromDisk
    {
        event NewRestoreStoryFromDiskHandler NewRestoreStoryFromDiskSend;
    }

    public delegate void NewRestoreStoryFromDiskHandler(object sender, WriteStoryToDiskEventArgs e);

}
