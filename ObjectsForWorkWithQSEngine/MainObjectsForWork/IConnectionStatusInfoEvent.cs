using System;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class ConnectionStatusInfoEventArgs : EventArgs
    {

        public readonly ConnectionStatusInfo ConnectionStatusInfo;

        public ConnectionStatusInfoEventArgs(ConnectionStatusInfo locationObject)
        {
            ConnectionStatusInfo = locationObject;
        }
    }

    public interface IConnectionStatusInfoEvent
    {
        event ConnectionStatusInfoHandler NewConnectionStatusInfoSend;
    }

    public delegate void ConnectionStatusInfoHandler(object sender, ConnectionStatusInfoEventArgs e);
}