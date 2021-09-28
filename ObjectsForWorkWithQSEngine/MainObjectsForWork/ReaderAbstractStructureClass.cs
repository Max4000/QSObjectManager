using Newtonsoft.Json;
using Qlik.Engine;

namespace ObjectsForWorkWithQSEngine.MainObjectsForWork
{
    public class ReaderAbstractStructureClass
    {
        public static void ReadObjectFromJsonFile(AbstractStructure absObject, string jsonFile)
        {

            JsonTextReader reader = Utils.MakeTextReader(jsonFile);


            absObject.ReadJson(reader);

            reader.Close();
        }
    }
}