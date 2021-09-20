using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qlik.Engine;
using Qlik.Engine.Communication;
using NxInfo = Qlik.Engine.NxInfo;
using NxMetaDef = Qlik.Engine.NxMetaDef;
using GenericBookmarkProperties  = Qlik.Engine.GenericBookmarkProperties;
using QlikConnection = Qlik.Engine.Communication.QlikConnection;




namespace MyBookmark2
{
    [JsonObject]
    [ValueClass]
    public class MyGenericBookmarkProperties : GenericBookmarkProperties, IAbstractStructure
    {
        
        private QlikConnection _connection;
        private AbstractStructure _wrap ;
        


        public MyGenericBookmarkProperties(string filename,IQlikConnection connect)
        {
            _connection = connect as QlikConnection;
            
            _wrap = new AbstractStructure(JObject.Load(
                new JsonTextReader(new StreamReader(new FileStream(filename, FileMode.Open), Encoding.UTF8))));

        }

       

        private AbstractStructure GetWrap()
        {
            //return _wrap.Get<AbstractStructure>("qProp");
            return _wrap;
        }

        public string PrintStructure(Formatting formatting = Formatting.None)
        {
            return _wrap.PrintStructure(Formatting.Indented);
        }

        public override string ToString()
        {
            return this.PrintStructure(Formatting.Indented);
        }

        public T As<T>() where T : AbstractStructure, new()
        {
            

            return GetWrap().As<T>();
        }

        public void WriteJson(JsonWriter writer)
        {
            GetWrap().WriteJson(writer);
        }

        public T CloneAs<T>() where T : AbstractStructure, new()
        {
            
            return GetWrap().CloneAs<T>();
        }

        public T CloneSubstructureAs<T>(string path)
        {
            
            return GetWrap().CloneSubstructureAs<T>(path);
        }

        public T Get<T>(string propertyName)
        {
            
            return GetWrap().Get<T>(propertyName);
        }

        public T Get<T>(string propertyName, T defaultValue)
        {
            
            return GetWrap().Get<T>(propertyName,defaultValue);
        }

        public void Set<T>(string propertyName, T value)
        {

            GetWrap().Set<T>(propertyName,value);
        }

        public bool IsSet(string propertyName)
        {
            
            return GetWrap().IsSet(propertyName);
        }

        public IEnumerable<string> GetAllProperties(bool recursive = false)
        {
            
            return GetWrap().GetAllProperties(recursive);
        }

        public IEnumerable<string> GetAllPropertyPaths(bool recursive = false)
        {
            
            return GetWrap().GetAllPropertyPaths(recursive);
        }

        public QlikConnection Session
        {
            get => this._connection;
        }
        [JsonProperty("qInfo")]
        public NxInfo Info
        {
            get
            {
                
                return GetWrap().Get<NxInfo>("qInfo");
            }

            set
            {

                GetWrap().Set("NxInfo",value);
            }
            
        }
        [JsonProperty("qMetaDef")]
        public NxMetaDef MetaDef
        {
            get
            {
                
                return GetWrap().Get<NxMetaDef>("qMetaDef");
            }
            set
            {
                GetWrap().Set("qMetaDef",value);
            }
        }
        

    }
}
