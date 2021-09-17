using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using Newtonsoft.Json;
using Qlik.Engine;
using QixClassBase = Qlik.Engine.QixClassBase;
using QlikConnection = Qlik.Engine.Communication.QlikConnection;

namespace MyBookmark
{
    internal class QixClassBaseConverter : JsonConverter
    {
        private static readonly ConcurrentDictionary<Type, QixClassCreator> Cache = new ConcurrentDictionary<Type, QixClassBaseConverter.QixClassCreator>();
        private readonly QlikConnection _session;

        public QixClassBaseConverter(QlikConnection session) => this._session = session;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();

        public override object ReadJson(
          JsonReader reader,
          Type objectType,
          object existingValue,
          JsonSerializer serializer)
        {
            QixClassBase qixClassBase = QixClassBaseConverter.Create(objectType);
            qixClassBase.ReadJson(reader, this._session);
            //if (typeof(IApp).IsAssignableFrom(objectType))
            //{
            //    ((App)qixClassBase).Hub = this._session.Hub;
            //    this._session.App = qixClassBase as IApp;
            //}
            //else if (typeof(IHub).IsAssignableFrom(objectType))
            //    this._session.Hub = qixClassBase as IHub;
            return qixClassBase;
        }

        public override bool CanConvert(Type objectType) => objectType.IsSubclassOf(typeof(QixClassBase));

        internal static QixClassBase Create(Type objectType)
        {
            ConstructorInfo constructor = objectType.GetConstructor(new Type[1]
            {
        typeof (int)
            });
            if (constructor == (ConstructorInfo)null)
                throw new ArgumentException("Type '" + objectType.FullName + "' does not have a default constructor", nameof(objectType));
            return (QixClassBaseConverter.Cache.GetOrAdd(objectType,
                        (Func<Type, QixClassBaseConverter.QixClassCreator>) (
                            type =>
                                QixClassBaseConverter
                                    .AbstractStructureCreateMethod(
                                        constructor)))() ??
                    throw new ArgumentException(
                        "Type '" + objectType.FullName + "' is not a supported AbstractStructure type",
                        nameof(objectType))) as QixClassBase;
        }

        private static QixClassCreator AbstractStructureCreateMethod(
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
            ilGenerator.Emit(OpCodes.Ldc_I4_0);
            ilGenerator.Emit(OpCodes.Newobj, constructor);
            ilGenerator.Emit(OpCodes.Ret);
            return (QixClassBaseConverter.QixClassCreator)dynamicMethod.CreateDelegate(typeof(QixClassBaseConverter.QixClassCreator));
        }

        private delegate object QixClassCreator();
    }
}