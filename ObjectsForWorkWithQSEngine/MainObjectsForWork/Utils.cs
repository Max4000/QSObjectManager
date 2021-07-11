using System.Collections.Generic;
using Qlik.Engine;
using Qlik.Sense.Client;
#pragma warning disable 618

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class Utils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static IList<NameAndIdPair> GetApps(ILocation location)
        {
            IList<NameAndIdPair> arr = new List<NameAndIdPair>();

            foreach ( var app in location.GetAppIdentifiers())
            {
               arr.Add(new NameAndIdPair(app.AppName,app.AppId)); 
            }

            return arr;
        }

        /// <summary>
        /// Returns list of stories for full id application
        /// </summary>
        /// <param name="location">object location for Dev Hub</param>
        /// <param name="appid">full id app</param>
        /// <returns>list of stories</returns>
        public static IList<NameAndIdPair> GetStorys(ILocation location, string appid)
        {
            IList<NameAndIdPair> lstResult = new List<NameAndIdPair>();

            IAppIdentifier appId = location.AppWithId(appid);
            
            using var app = location.App(appId);
            
            var listOfStories = app.GetStoryList();
            
            foreach (var item in listOfStories.Items)
            {

#pragma warning disable 618
                var mStory = app?.GetStory(item.Info.Id);
#pragma warning restore 618
                if (mStory != null)
                {
                    string name = mStory.Layout.Meta.Title;
                    lstResult.Add(new NameAndIdPair(name,item.Info.Id));
                }
            }

            return lstResult;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NameAndIdPair
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name;

        /// <summary>
        /// 
        /// </summary>
        public string Id;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public NameAndIdPair(string name, string id)
        {
            Name = name;
            Id = id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
