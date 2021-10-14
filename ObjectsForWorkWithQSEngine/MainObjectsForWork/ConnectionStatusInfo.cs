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

    

}
