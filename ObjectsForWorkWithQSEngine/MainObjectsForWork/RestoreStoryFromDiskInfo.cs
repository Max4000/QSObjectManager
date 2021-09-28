using System.Collections.Generic;
using Qlik.Engine;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class RestoreStoryFromDiskInfo
    {
        
        public IApp TargetApp;
        public NameAndIdAndLastReloadTime CurrentAppSource;
        public NameAndIdAndLastReloadTime CurrentAppTarget;
        public NameAndIdAndLastReloadTime CurrentStory;

        public string AppContentFolder;
        public string DefaultFolder;

        public string StoryFolder;

        public IList<string> AddContentList;
        public string FolderNameWithAddContent;


        public void Copy(RestoreStoryFromDiskInfo anotherInfo)
        {
            
            anotherInfo.TargetApp = TargetApp;
            anotherInfo.AppContentFolder = AppContentFolder;
            anotherInfo.DefaultFolder = DefaultFolder;
            anotherInfo.CurrentAppSource = CurrentAppSource.Copy();
            anotherInfo.CurrentAppTarget = CurrentAppTarget.Copy();
            anotherInfo.CurrentStory = CurrentStory.Copy();
            anotherInfo.StoryFolder = StoryFolder;
            anotherInfo.AddContentList = AddContentList;
            anotherInfo.FolderNameWithAddContent = FolderNameWithAddContent;
        }

    }

}