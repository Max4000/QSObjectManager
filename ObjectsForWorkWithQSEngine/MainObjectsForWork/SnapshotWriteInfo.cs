using System;
using Qlik.Engine;
using Qlik.Sense.Client.Storytelling;
#pragma warning disable 618

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    
    public class SnapshotWriteInfoEventArgs : EventArgs
    {
        public readonly SnapshotWriteInfo ItemInfo;

        public SnapshotWriteInfoEventArgs(SnapshotWriteInfo item)
        {
            ItemInfo = item;
        }
    }

    public interface IWriteSnapshotToDisk
    {
        event WriteSnapshotToDisk NewSnapshotToDiskSend;
    }

    public delegate void WriteSnapshotToDisk(object sender, SnapshotWriteInfoEventArgs e);

    public class SnapshotWriteInfo
    {
        public IApp App;
        public string ItemFolder;
        public ISlideItem SlideItem;
        public void Copy(ref SnapshotWriteInfo anotherItem)
        {
            anotherItem.App = App;
            anotherItem.ItemFolder = ItemFolder;
            anotherItem.SlideItem = SlideItem;
        }

    }
}
