using System;
using System.Collections.Generic;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    
   
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