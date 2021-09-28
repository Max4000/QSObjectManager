using System;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class RestoreStoryFromDiskEventArgs : EventArgs
    {

        public readonly RestoreStoryFromDiskInfo RestoreInfo;


        public RestoreStoryFromDiskEventArgs(RestoreStoryFromDiskInfo record)
        {
            RestoreInfo = record;
        }
    }

    public interface IRestoreStoryFromDisk
    {
        event RestoreStoryFromDiskHandler NewRestoreStoryFromDiskSend;
    }

    public delegate void RestoreStoryFromDiskHandler(object sender, RestoreStoryFromDiskEventArgs e);
}