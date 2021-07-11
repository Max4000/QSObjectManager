using System;
using Qlik.Engine;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class LocationObject : IDisposable
    {
        private readonly ILocation _location;

        public LocationObject(string addr)
        {
            var uri = new Uri(addr);

            _location = Location.FromUri(uri);
            _location.AsDirectConnectionToPersonalEdition();

        }

        public ILocation LocationPersonalEdition => _location;

        public void Dispose()
        {
            _location?.Dispose();
        }
    }
}
