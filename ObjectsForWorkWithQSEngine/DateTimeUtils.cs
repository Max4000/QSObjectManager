using System;
using System.Text;

namespace ObjectsForWorkWithQSEngine
{
    public class DateTimeUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string NowToString()
        {
            DateTime now = DateTime.Now;

            int day   = now.Day;
            int month = now.Month;
            int year = now.Year;

            int howr = now.Hour;
            int min = now.Minute;
            int sec = now.Second;
            int msec = now.Millisecond;

            return LPad(year.ToString(), 4, "0")+LPad(month.ToString(),2,"0")+
                   LPad(day.ToString(),2,"0")+LPad(howr.ToString(),2,"0")+
                   LPad(min.ToString(),2,"0")+
                   LPad(sec.ToString(),2,"0")+LPad(msec.ToString(),3,"0");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string NowToNormalString()
        {
            DateTime now = DateTime.Now;

            int day = now.Day;
            int month = now.Month;
            int year = now.Year;

            int howr = now.Hour;
            int min = now.Minute;
            return LPad(day.ToString(), 2, "0") + "." + LPad(month.ToString(), 2, "0") + "." +
                   LPad(year.ToString(), 4, "0") +
                   " " + LPad(howr.ToString(), 2, "0") + ":" + LPad(min.ToString(), 2, "0");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="width"></param>
        /// <param name="pad"></param>
        /// <returns></returns>
        private static string LPad(string source, int width, string pad)
        {
            StringBuilder strResult = new StringBuilder(10);

            for (int i = 0; i < (width - source.Length); i++)
            {
                strResult.Append(pad);
            }

            strResult.Append(source);

            return strResult.ToString();
        }
    }
}
