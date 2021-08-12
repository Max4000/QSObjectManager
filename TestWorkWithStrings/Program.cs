using System;
using Qlik.Engine;
using Qlik.Engine.Communication;

namespace TestWorkWithStrings
{
    class Program
    {
        static void Main()
        {
            var uri = new Uri("https://localhost:4747");
            var certs = CertificateManager.LoadCertificateFromStore();

            ILocation location = Qlik.Engine.Location.FromUri(uri);
            location.AsDirectConnection("DESKTOP-ULSENNT", "Anatoliy", certificateCollection: certs);

            using (var hub = location.Hub())
            {
                Console.WriteLine(hub.EngineVersion().ComponentVersion);
            }
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
