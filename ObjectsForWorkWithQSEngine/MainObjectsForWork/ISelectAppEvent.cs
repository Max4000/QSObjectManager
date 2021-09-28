using System;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class SelectedAppEventArgs : EventArgs
    {

        public readonly NameAndIdAndLastReloadTime SelectedApp;


        public SelectedAppEventArgs(NameAndIdAndLastReloadTime record)
        {
            SelectedApp = record;
        }
    }

    public interface ISelectAppEvent
    {
        event AppSelectedHandler NewAppSelectedSend;
    }

    public delegate void AppSelectedHandler(object sender, SelectedAppEventArgs e);
}