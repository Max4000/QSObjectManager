using System;
using System.Diagnostics.CodeAnalysis;
using Qlik.Engine;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{


    public class ConnectionStatusInfo
    {
        public IConnect LocationObject;
        
        public ConnectionStatusInfo(IConnect locationObject)
        {
            LocationObject = locationObject;
            
        }

        public void Copy(ref IConnect anotherLocationObject)
        {
            anotherLocationObject = LocationObject;
        }

    }

    public class ConnectionStatusInfoEventArgs : EventArgs
    {

        public readonly ConnectionStatusInfo ConnectionStatusInfo;

        public ConnectionStatusInfoEventArgs(ConnectionStatusInfo refLocatioobject)
        {
            ConnectionStatusInfo = refLocatioobject;
        }
    }

    public interface IConnectionStatusInfoEvent
    {
        event ConnectionStatusInfoHandler NewConnectionStatusInfoSend;
    }

    public delegate void ConnectionStatusInfoHandler(object sender, ConnectionStatusInfoEventArgs e);

    public interface IConnect : IDisposable
    {
        public bool Connect();

        public void Disconnect();
        public bool IsConnected();
        
        // ReSharper disable once UnusedMember.Global
        ILocation GetConnection();
    }

    public abstract class Connection : IConnect
    {
        protected ILocation Location;

        public abstract bool Connect();
        
        public bool IsConnected()
        {
            return Location != null;
        }

        public ILocation GetConnection()
        {
            return Location;
        }

        public void Dispose()
        {
            Location?.Dispose();
        }

        public void Disconnect()
        {
            Location = null;
        }
    }

    public class LocalConnection: Connection
    {
        
        private string _addr;


        public override bool Connect()
        {
            var uri = new Uri(_addr);

            Location = Qlik.Engine.Location.FromUri(uri);
            Location.AsDirectConnectionToPersonalEdition();

            return IsConnected(); ;
        }

        public LocalConnection(string addr)
        {
            _addr = addr;

        }
    }

    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    public class RemoteConnection : Connection
    {
        private readonly string  _addr;
        private string  _domen;
        private string _userId;

        public override bool Connect()
        {
            var uri = new Uri(_addr);

            Location = Qlik.Engine.Location.FromUri(uri);
            Location.AsDirectConnectionToPersonalEdition();

            return IsConnected(); 
        }

        public RemoteConnection(string addr, string domen, string userId)
        {
            
            _domen = domen;
            _userId = userId;
        }

        public RemoteConnection(string addr)
        {
            _addr = addr; ;
        }
    }



    //[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
    //public class LocationObject : IDisposable
    //{
    //    private ILocation _location;

    //    private string _addr;

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="addr"></param>
    //    public LocationObject(string addr)
    //    {
    //        _addr = addr;

    //    }


    //    public bool Connect()
    //    {
    //        var uri = new Uri(_addr);

    //        _location = Location.FromUri(uri);
    //        _location.AsDirectConnectionToPersonalEdition();

    //        return IsConnected();

    //    }


    //    public bool IsConnected()
    //    {
    //        return _location != null;
    //    }


    //    public ILocation LocationPersonalEdition => _location;


    //    public void Dispose()
    //    {
    //        _location?.Dispose();
    //    }
    //}
}
