using System;

namespace TestWorkWithStrings
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Hello World!");

            string unused = GetPath("/media/9af7cf0becf14ca4ca59d232dfcf8b77/n1/Qlik_default_feathersN1.png");
        }

        static string GetPath(string fromString)
        {
            int pos1 = fromString.IndexOf('/', 0);
            int pos2 = fromString.IndexOf('/', pos1 + 1);
            int pos3 = fromString.IndexOf('/', pos2 + 1);
            return fromString.Substring(pos3+1);
        }
    }
}
