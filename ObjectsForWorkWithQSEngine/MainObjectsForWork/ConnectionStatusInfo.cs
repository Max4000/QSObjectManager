using System;
using MConnect.Location;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class ConnectionStatusInfo
    {
        public IConnect LocationObject;

        public ConnectionStatusInfo(IConnect locationObject)
        {
            LocationObject = locationObject;

        }

        public IConnect Copy()
        {
            return LocationObject;
        }

    }

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
