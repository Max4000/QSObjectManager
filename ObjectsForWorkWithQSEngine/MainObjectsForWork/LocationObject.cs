using System;
using System.Diagnostics.CodeAnalysis;
using Qlik.Engine;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{


    public class ConnectionStatusInfo
    {
        public LocationObject LocationObject;
        
        public ConnectionStatusInfo(LocationObject locationObject)
        {
            this.LocationObject = locationObject;
            
        }

/*
        public ConnectionStatusInfo()
        {

        }
*/
        // ReSharper disable once RedundantAssignment
        public void Copy(ref LocationObject anotherLocationObject)
        {
            anotherLocationObject = this.LocationObject;
            
        }

    }

    public class ConnectionStatusInfoEventArgs : EventArgs
    {

        public readonly ConnectionStatusInfo ConnectionStatusInfo;

        //Конструкторы
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



    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
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

        //public void SetAddress(string addr)
        //{
        //    _addr = addr;
        //}

        public bool Connect()
        {
            var uri = new Uri(_addr);
            
            _location = Location.FromUri(uri);
            _location.AsDirectConnectionToPersonalEdition();

            return IsConnected();

        }

/*
        public void Disconnect()
        {
            _location.Dispose();
            _location = null;
        }
*/

        public bool IsConnected()
        {
            return _location != null;
        }

        //public LocationObject()
        //{
        //    _location = null;
        //}


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
