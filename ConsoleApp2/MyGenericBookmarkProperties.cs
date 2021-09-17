using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qlik.Engine;
using NxInfo = Qlik.Engine.NxInfo;
using NxMetaDef = Qlik.Engine.NxMetaDef;
using GenericBookmarkProperties  = Qlik.Engine.GenericBookmarkProperties;
using QlikConnection = Qlik.Engine.Communication.QlikConnection;




namespace MyBookmark
{
    [JsonObject]
    [ValueClass]
    [QixName("qProp")]
    public class MyGenericBookmarkProperties : GenericBookmarkProperties
    {
        private AbstractStructure _abstractStructure ;
        private QlikConnection _connection;

        public MyGenericBookmarkProperties(string filename)
        {
            _abstractStructure =new AbstractStructure(JObject.Load(new JsonTextReader(new StreamReader(new FileStream(filename, FileMode.Open), Encoding.UTF8))));

        }




        public string PrintStructure(Formatting formatting = Formatting.None)
        {
            return _abstractStructure.PrintStructure(Formatting.Indented);
        }

        public override string ToString()
        {
            return this.PrintStructure(Formatting.Indented);
        }

        public T As<T>() where T : AbstractStructure, new()
        {
            

            return _abstractStructure.As<T>();
        }

        public T CloneAs<T>() where T : AbstractStructure, new()
        {
            
            return _abstractStructure.CloneAs<T>();
        }

        public T CloneSubstructureAs<T>(string path)
        {
            
            return _abstractStructure.CloneSubstructureAs<T>(path);
        }

        public T Get<T>(string propertyName)
        {
            
            return _abstractStructure.Get<T>(propertyName);
        }

        public T Get<T>(string propertyName, T defaultValue)
        {
            
            return _abstractStructure.Get<T>(propertyName,defaultValue);
        }

        public void Set<T>(string propertyName, T value)
        {
            
            _abstractStructure.Set<T>(propertyName,value);
        }

        public bool IsSet(string propertyName)
        {
            
            return _abstractStructure.IsSet(propertyName);
        }

        public IEnumerable<string> GetAllProperties(bool recursive = false)
        {
            
            return _abstractStructure.GetAllProperties(recursive);
        }

        public IEnumerable<string> GetAllPropertyPaths(bool recursive = false)
        {
            
            return _abstractStructure.GetAllPropertyPaths(recursive);
        }

        public QlikConnection Session
        {
            get => _abstractStructure.Session;
        }

        public NxInfo Info
        {
            get
            {
                
                return _abstractStructure.Get<NxInfo>("qInfo");
            }

            set
            {

                _abstractStructure.Set("NxInfo",value);
            }
            
        }

        public NxMetaDef MetaDef
        {
            get
            {
                
                return _abstractStructure.Get<NxMetaDef>("qMetaDef");
            }
            set
            {
                _abstractStructure.Set("qMetaDef",value);
            }
        }
    }
}
