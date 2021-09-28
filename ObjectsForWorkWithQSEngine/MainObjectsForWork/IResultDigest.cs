using System;
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

    public class ResultDigestInfoEventArgs : EventArgs
    {

        public readonly ResultDigestInfo ResultInfo;

        
        public ResultDigestInfoEventArgs(ResultDigestInfo record)
        {
            ResultInfo = record;
        }
    }

    public interface IResultDigest
    {
        event ResultDigestInfoHandler NewResultDigestInfoSend;
    }

    public delegate void ResultDigestInfoHandler(object sender, ResultDigestInfoEventArgs e);

}