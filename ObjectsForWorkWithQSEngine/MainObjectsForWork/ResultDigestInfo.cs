using System.Collections.Generic;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class ResultDigestInfo
    {
        public IList<string> AddContentList;
        public string FolderWithAddContent;

        public void Copy(ResultDigestInfo anotherInfo)
        {
            anotherInfo.AddContentList = AddContentList;
            anotherInfo.FolderWithAddContent = FolderWithAddContent;
        }
    }

}