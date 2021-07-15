using System.Collections.Generic;
using Qlik.Engine;
using Qlik.Sense.Client;
using Qlik.Sense.Client.Storytelling;

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


                var mStory = app?.GetStory(item.Info.Id);

                if (mStory != null)
                {
                    string name = mStory.Layout.Meta.Title;
                    lstResult.Add(new NameAndIdPair(name,item.Info.Id));
                }
            }

            return lstResult;
        }

        public static IStory GetStoryFromApp(ILocation location, string appid,string storeId)
        {
            
            IAppIdentifier appId = location.AppWithId(appid);
            
            using IApp app = location.App(appId);

            return app?.GetStory(storeId);

        }
        


    }


}
