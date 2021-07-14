using System;
using System.Collections.Generic;
using Qlik.Engine;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{


    public class ConnectionStatusInfo
    {
        public LocationObject _LocationObject;
        
        public ConnectionStatusInfo(LocationObject locationObject)
        {
            this._LocationObject = locationObject;
            
        }

        public ConnectionStatusInfo()
        {

        }
        public void Copy(ref LocationObject anotherLocationObject)
        {
            anotherLocationObject = this._LocationObject;
            
        }

    }

    public class ConnectionStatusInfoEventArgs : EventArgs
    {

        public readonly ConnectionStatusInfo _ConnectionStatusInfo;

        //Конструкторы
        public ConnectionStatusInfoEventArgs(ConnectionStatusInfo refLocatioobject)
        {
            _ConnectionStatusInfo = refLocatioobject;
        }
    }

    public interface IConnectionStatusInfoEvent
    {
        event ConnectionStatusInfoHandler NewConnectionStatusInfoSend;
    }

    public delegate void ConnectionStatusInfoHandler(object sender, ConnectionStatusInfoEventArgs e);



    public class LocationObject : IDisposable
    {
        private ILocation _location;

        private string _addr;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        public LocationObject(string addr)
        {
            _addr = addr;
         
        }

        public void SetAddress(string addr)
        {
            _addr = addr;
        }

        public bool Connect()
        {
            var uri = new Uri(_addr);
            
            _location = Location.FromUri(uri);
            _location.AsDirectConnectionToPersonalEdition();

            return IsConnected();

        }

        public void Disconnect()
        {
            _location.Dispose();
            _location = null;
        }

        public bool IsConnected()
        {
            return _location != null;
        }

        public LocationObject()
        {
            _location = null;
        }


        /// <summary>
        /// 
        /// </summary>
        public ILocation LocationPersonalEdition => _location;

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _location?.Dispose();
        }
    }
}
