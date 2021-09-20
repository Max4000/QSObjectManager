using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using Newtonsoft.Json;
using Qlik.Engine;
using QlikConnection = Qlik.Engine.Communication.QlikConnection;

namespace MConnect
{
    public class AbstractStructureConverter : JsonConverter
    {
        private static readonly ConcurrentDictionary<Type, AbstractStructureConverter.AbstractStructureCreator> Cache =
            new ConcurrentDictionary<Type, AbstractStructureCreator>();
        private readonly QlikConnection _session;

        public AbstractStructureConverter(QlikConnection session) => this._session = session;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is Qlik.Engine.AbstractStructure abstractStructure))
                writer.WriteNull();
            else
                abstractStructure.WriteJson(writer);
        }

        public override object ReadJson(
          JsonReader reader,
          Type objectType,
          object existingValue,
          JsonSerializer serializer)
        {
            AbstractStructure abstractStructure = AbstractStructureConverter.Create(objectType);
            abstractStructure.ReadJson(reader, this._session);
            return (object)abstractStructure;
        }

        internal static AbstractStructure Create(Type objectType)
        {
            ConstructorInfo constructor = objectType.GetConstructor(Type.EmptyTypes);
            if (constructor == (ConstructorInfo)null)
                throw new ArgumentException("Type '" + objectType.FullName + "' does not have a default constructor", nameof(objectType));
            return (AbstractStructureConverter.Cache.GetOrAdd(objectType,
                        (Func<Type, AbstractStructureCreator>) (type =>
                            AbstractStructureCreateMethod(
                                    constructor)))() ??
                    throw new ArgumentException(
                        "Type '" + objectType.FullName + "' is not a supported AbstractStructure type",
                        nameof(objectType))) as AbstractStructure;
        }

        public override bool CanConvert(Type objectType)
        {
            bool ok = objectType.IsSubclassOf(typeof(AbstractStructure));
            return  ok;
        }

        private static AbstractStructureCreator AbstractStructureCreateMethod(
          ConstructorInfo constructor)
        {
            string empty = string.Empty;
            Type returnType = typeof(object);
            Type[] emptyTypes = Type.EmptyTypes;
            Type owner = constructor.DeclaringType;
            if ((object)owner == null)
                owner = typeof(object);
            DynamicMethod dynamicMethod = new DynamicMethod(empty, returnType, emptyTypes, owner);
            ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Newobj, constructor);
            ilGenerator.Emit(OpCodes.Ret);
            return (AbstractStructureConverter.AbstractStructureCreator)dynamicMethod.CreateDelegate(typeof(AbstractStructureConverter.AbstractStructureCreator));
        }

        private delegate object AbstractStructureCreator();
    }
}