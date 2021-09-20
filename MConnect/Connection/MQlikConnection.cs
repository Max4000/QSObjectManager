using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qlik.Engine;
using Qlik.Engine.Communication;
using Qlik.Sense.JsonRpc;
using ISession = Qlik.Engine.ISession;

namespace MConnect.Connection
{
    public class MQlikConnection : QlikConnection
    {
        public MQlikConnection(IGenericLocation location, ISession session) : base(location, session)
        {
        }

        protected override JsonSerializer MakeInboundSerializer()
        {
            return base.MakeInboundSerializer();
        }

        protected override JsonSerializer MakeOutboundSerializer()
        {
            return base.MakeOutboundSerializer();
        }
    }

}