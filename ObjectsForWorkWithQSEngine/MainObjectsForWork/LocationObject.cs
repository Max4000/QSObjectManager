using System;
using System.Diagnostics.CodeAnalysis;
using Qlik.Engine;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{

    public interface IConnect : IDisposable
    {
        public bool Connect();

        public void Disconnect();
        public bool IsConnected();
        
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
        
        private readonly string _address;


        public override bool Connect()
        {
            var uri = new Uri(_address);

            Location = Qlik.Engine.Location.FromUri(uri);
            Location.AsDirectConnectionToPersonalEdition();

            return IsConnected(); 
        }

        public LocalConnection(string address)
        {
            _address = address;

        }
    }

    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    public class RemoteConnection : Connection
    {
        private readonly string  _address;
        

        public override bool Connect()
        {
            var uri = new Uri(_address);

            Location = Qlik.Engine.Location.FromUri(uri);
            Location.AsNtlmUserViaProxy(); 

            return IsConnected(); 
        }

        

        public RemoteConnection(string address)
        {
            _address = address; 
        }
    }



   
}
