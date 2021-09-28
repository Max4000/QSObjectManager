using System;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class RestoreInfoEventArgs : EventArgs
    {

        public readonly RestoreInfo RestoreInfo;

        public RestoreInfoEventArgs(RestoreInfo record)
        {
            RestoreInfo = record;
        }
    }

    public interface IRestoreInfoEvent
    {
        event RestoreInfoHandler NewRestoreInfoSend;
    }

    public delegate void RestoreInfoHandler(object sender, RestoreInfoEventArgs e);
}