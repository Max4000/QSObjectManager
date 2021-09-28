using System.Collections.Generic;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class RestoreInfo
    {
        public NameAndIdAndLastReloadTime SourceApp;

        public IList<NameAndIdAndLastReloadTime> SelectedStories;
        public NameAndIdAndLastReloadTime TargetApp;


        public RestoreInfo(NameAndIdAndLastReloadTime sourceApp, IList<NameAndIdAndLastReloadTime> selectedStories, NameAndIdAndLastReloadTime targetApp)
        {
            SourceApp = sourceApp;
            TargetApp = targetApp;
            SelectedStories = selectedStories;
        }

        public RestoreInfo()
        {

        }
        public void Copy(RestoreInfo anotherWriteInfo)
        {
            anotherWriteInfo.TargetApp = TargetApp.Copy();
            anotherWriteInfo.SourceApp = SourceApp.Copy();
            anotherWriteInfo.SelectedStories = new List<NameAndIdAndLastReloadTime>();
            foreach (var story in this.SelectedStories)
            {
                anotherWriteInfo.SelectedStories.Add(story.Copy());
            }
        }

    }
}