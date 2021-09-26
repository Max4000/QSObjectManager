using System;

namespace TestCurrentUser
{
    class Program
    {
        static void Main(string[] args)
        {
            string user = UtilClasses.CurrentUser.GetExplorerUser();
            Console.WriteLine("user");
        }

        

    }
}
