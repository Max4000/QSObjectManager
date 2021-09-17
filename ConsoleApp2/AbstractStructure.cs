using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using InvalidPropertyAccessException = Qlik.Engine.InvalidPropertyAccessException;
using QlikConnection = Qlik.Engine.Communication.QlikConnection;

namespace MyBookmark
{

    public class AbstractStructure : IDisposable, IAbstractStructure
    {
        private bool _disposed;

        private static JsonSerializer _staticSerializer;

        private AbstractStructure.StateT State { get; set; }

        [JsonIgnore]
        public QlikConnection Session => this.State.Session;

        public string PrintStructure(Formatting formatting = Formatting.None) => this.ToString(formatting);

        public override string ToString() => this.PrintStructure(Formatting.Indented);

        public string ToString(Formatting formatting) => this.CopyFullStructure().ToString(formatting);

        static AbstractStructure()
        {
            JsonConvert.DefaultSettings = (Func<JsonSerializerSettings>) (() => new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                Converters = (IList<JsonConverter>) new JsonConverter[4]
                {
                    (JsonConverter) new QixEnumConverter(),
                    (JsonConverter) new CharToIntConverter(),
                    (JsonConverter) new DoubleConverter(),
                    (JsonConverter) new QixClassBaseConverter(null)
                }
            });
        }

        public AbstractStructure() => this.State = new AbstractStructure.StateT();

        public AbstractStructure(JObject jObject)
          : this()
        {
            this.State.Structure = (JToken)new JObject(jObject);
        }

        public JObject GetJObject()
        {
            JToken jtoken = this.CopyFullStructure();
            return jtoken is JObject ? jtoken as JObject : throw new ArgumentException("AbstractStructure instance does not wrap a JObject token.");
        }

        public T As<T>() where T : AbstractStructure, new()
        {
            if (!(this is T obj1))
            {
                T obj = new T();
                obj.State = this.State;
                obj1 = obj;
            }
            return obj1;
        }

        public T CloneAs<T>() where T : AbstractStructure, new()
        {
            AbstractStructure.StateT state = this.State;
            bool lockTaken = false;
            try
            {
                Monitor.Enter((object)state, ref lockTaken);
                T obj = new T();
                obj.State = new AbstractStructure.StateT()
                {
                    Session = this.Session,
                    Structure = this.CopyFullStructure()
                };
                return obj;
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit((object)state);
            }
        }

        public T Get<T>(string propertyName) => this.Get<T>(propertyName, default(T));

        public T Get<T>(string propertyName, T defaultValue)
        {
            AbstractStructure.StateT state = this.State;
            bool lockTaken = false;
            try
            {
                Monitor.Enter((object)state, ref lockTaken);
                if (propertyName == "")
                {
                    if (this.State.Structure is JValue)
                        return (T)this.CreateValue<T>(propertyName, (object)defaultValue);
                    throw new InvalidOperationException("Getting root object for non-JValue structure.");
                }
                object orAdd = this.State.Cache.GetOrAdd(propertyName, (Func<string, object>)(key => this.CreateValue<T>(propertyName, (object)(T)defaultValue)));
                if (orAdd == null)
                    return defaultValue;
                Type type1 = typeof(T);
                Type type2 = orAdd.GetType();
                if (type1 == type2)
                    return (T)orAdd;
                if (orAdd is AbstractStructure abstractStructure3 && type1.IsSubclassOf(typeof(AbstractStructure)))
                {
                    AbstractStructure abstractStructure = AbstractStructureConverter.Create(type1);
                    abstractStructure.State = abstractStructure3.State;
                    return (T)(object) abstractStructure;
                }
                try
                {
                    return (T)orAdd;
                }
                catch (InvalidCastException ex)
                {
                    throw new InvalidPropertyAccessException(propertyName, type2, type1);
                }
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit((object)state);
            }
        }

        public void Set<T>(string propertyName, T value)
        {
            AbstractStructure.StateT state = this.State;
            bool lockTaken = false;
            try
            {
                Monitor.Enter((object)state, ref lockTaken);
                if (propertyName == "")
                {
                    this.State.Structure = this.AsToken<T>(value);
                }
                else
                {
                    if (!(this.State.Structure is JObject))
                        this.State.Structure = (JToken)new JObject();
                    this.State.Cache.AddOrUpdate(propertyName, (Func<string, object>)(key => this.AddValue<T>(propertyName, value)), (Func<string, object, object>)((key, oldValue) => (object)(T)value));
                }
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit((object)state);
            }
        }

        public bool IsSet(string propertyName)
        {
            AbstractStructure.StateT state = this.State;
            bool lockTaken = false;
            try
            {
                Monitor.Enter((object)state, ref lockTaken);
                object obj;
                if (!this.State.Cache.TryGetValue(propertyName, out obj))
                    obj = (object)this.State.Structure.SelectToken(propertyName, false);
                return obj != null;
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit((object)state);
            }
        }

        public IEnumerable<string> GetAllProperties(bool recursive = false)
        {
            JObject jobject = this.CopyFullStructure().ToObject<JObject>();
            return (recursive ? jobject.Descendants().OfType<JProperty>() : jobject.Properties()).Select<JProperty, string>((Func<JProperty, string>)(p => p.Name));
        }

        public IEnumerable<string> GetAllPropertyPaths(bool recursive = false)
        {
            JObject jobject = this.CopyFullStructure().ToObject<JObject>();
            return (recursive ? jobject.Descendants().OfType<JProperty>() : jobject.Properties()).Select<JProperty, string>((Func<JProperty, string>)(p => AbstractStructure.ToQixPath(p.Path)));
        }

        public T CloneSubstructureAs<T>(string path) => (T)this.CloneAs<DynamicStructure>().CreateValue<T>(AbstractStructure.FromQixPath(path));

        private static string ToQixPath(string path) => path.Replace("]", "").Replace('[', '.').Replace('.', '/').Insert(0, "/");

        private static string FromQixPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Path is null or empty.", nameof(path));
            return path[0] == '/' ? Regex.Replace(path, "/(\\d+)(/|$)", "[$1]$2").Replace("/", ".").Substring(1) : throw new ArgumentException("Path argument does not start with '/'. Not a valid path: " + path, nameof(path));
        }

        private object CreateValue<T>(string propertyName, object defaultValue = null)
        {
            JToken jtoken = this.State.Structure.SelectToken(propertyName, false);
            if (jtoken == null)
            {
                if (defaultValue == null)
                    return (object)null;
                jtoken = this.AsToken<T>((T)defaultValue);
                if (jtoken is JContainer)
                    this.CreateCacheValue(propertyName, jtoken);
            }
            else
                AbstractStructure.ConsumeToken(jtoken);
            T obj;
            if (this.Session == null)
                obj = jtoken.ToObject<T>();
            else
                obj = this.Session.Deserialize<T>(jtoken);
            if (obj is AbstractStructure abstractStructure)
                abstractStructure.State.Structure = jtoken;
            return (object)obj;
        }

        private static void ConsumeToken(JToken token)
        {
            JContainer parent = token.Parent;
            if (parent == null)
                return;
            if (parent is JProperty)
            {
                if (parent.Parent == null)
                    return;
                token.Parent.Remove();
            }
            else
                token.Remove();
        }

        private object AddValue<T>(string propertyName, T value)
        {
            JToken token = this.State.Structure.SelectToken(propertyName, false);
            if (token != null)
                AbstractStructure.ConsumeToken(token);
            return (object)value;
        }

        private void CreateCacheValue(string propertyName, JToken newToken)
        {
            if (this.State.Structure is JObject structure)
                structure.Add(propertyName, newToken);
            else
                this.State.Structure = (JToken)new JObject((object)new JProperty(propertyName, (object)newToken));
        }

        //private JToken AsToken<T>(T value) =>


        //             JToken.FromObject((object) value)

        //        ;

        internal static JsonSerializer StaticSerializer
        {
            get
            {
                //object staticSerializerLock = QlikConnection.StaticSerializerLock;
                bool lockTaken = false;
                try
                {
                    //Monitor.Enter(staticSerializerLock, ref lockTaken);
                    //if (QlikConnection._staticSerializer != null)
                    //    return QlikConnection._staticSerializer;
                    _staticSerializer = new JsonSerializer()
                    {
                        ObjectCreationHandling = ObjectCreationHandling.Replace,
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    ((Collection<JsonConverter>)_staticSerializer.Converters).Add((JsonConverter)new QixEnumConverter());
                    ((Collection<JsonConverter>)_staticSerializer.Converters).Add((JsonConverter)new AbstractStructureConverter((QlikConnection)null));
                    ((Collection<JsonConverter>)_staticSerializer.Converters).Add((JsonConverter)new QixClassBaseConverter((QlikConnection)null));
                    ((Collection<JsonConverter>)_staticSerializer.Converters).Add((JsonConverter)new DoubleConverter(true));
                    ((Collection<JsonConverter>)_staticSerializer.Converters).Add((JsonConverter)new CharToIntConverter());
                    return _staticSerializer;
                }
                finally
                {
                    //if (lockTaken)
                    //    Monitor.Exit(staticSerializerLock);
                }
            }
        }

        private JToken AsToken<T>(T value)
        {
            if ((object) value == null)
                return (JToken) new JValue((object) default(T));
            else if (!(value is AbstractStructure abstractStructure))
                if (this.Session == null)
                    return JToken.FromObject((object) value, StaticSerializer);
                //else
                //    return this.Session.Serialize((object) value);
                else
                {
                    return null;
                }
            else
                return abstractStructure.CopyFullStructure();
        }

        private JToken CopyFullStructure()
        {
            JToken jtoken = this.State.Structure == null ? (JToken)new JObject() : this.State.Structure.DeepClone();
            if (!(jtoken is JObject jobject))
                return jtoken;
            foreach (KeyValuePair<string, object> keyValuePair in this.State.Cache.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>)(entry => entry.Value != null)))
                jobject.Add(keyValuePair.Key, this.AsToken<object>(keyValuePair.Value));
            return (JToken)jobject;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        private void Dispose(bool disposing)
        {
            if (!this._disposed && disposing)
            {
                this.State.Cache.Clear();
                this.State.Structure = (JToken)null;
                this.State.Session = (QlikConnection)null;
            }
            this._disposed = true;
        }

        public void WriteJson(JsonWriter writer)
        {
            AbstractStructure.StateT state = this.State;
            bool lockTaken = false;
            JToken jtoken;
            try
            {
                Monitor.Enter((object)state, ref lockTaken);
                jtoken = this.CopyFullStructure();
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit((object)state);
            }
            jtoken.WriteTo(writer);
        }

        public void ReadJson(JsonReader reader, QlikConnection session = null)
        {
            AbstractStructure.StateT state = this.State;
            bool lockTaken = false;
            try
            {
                Monitor.Enter((object)state, ref lockTaken);
                this.State.Structure = JToken.ReadFrom(reader);
                this.State.Session = session;
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit((object)state);
            }
        }

        public static IEnumerable<IStructureDiff> Diff(
          AbstractStructure left,
          AbstractStructure right)
        {
            return (IEnumerable<IStructureDiff>)StructureDiff.Diff(left.CopyFullStructure(), right.CopyFullStructure());
        }

        private class StateT
        {
            private JToken _structure;

            public QlikConnection Session { get; set; }

            public ConcurrentDictionary<string, object> Cache { get; private set; }

            public JToken Structure
            {
                get => this._structure;
                set
                {
                    this.Cache.Clear();
                    if (this._structure == null || this._structure.Parent == null)
                        this._structure = value;
                    else
                        this._structure.Replace(value);
                }
            }

            public StateT()
            {
                this.Cache = new ConcurrentDictionary<string, object>();
                this.Structure = (JToken)new JObject();
            }
        }
    }
}