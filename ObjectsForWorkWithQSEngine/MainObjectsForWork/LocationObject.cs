using System;
using Qlik.Engine;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class LocationObject : IDisposable
    {
        private readonly ILocation _location;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        public LocationObject(string addr)
        {
            var uri = new Uri(addr);

            _location = Location.FromUri(uri);
            _location.AsDirectConnectionToPersonalEdition();

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
