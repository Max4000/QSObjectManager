namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public interface IRestoreSnapshotsFromDisk
    {
        event SnapshotRestoreInfo NewSnapshotFromDiskSend;
    }

    public delegate void SnapshotRestoreInfo(object sender, SnapshotWriteInfoEventArgs e);
}
