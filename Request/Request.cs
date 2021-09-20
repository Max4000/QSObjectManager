// Decompiled with JetBrains decompiler
// Type: Qlik.Sense.JsonRpc.Request
// Assembly: Qlik.Sense.JsonRpc, Version=15.5.0.0, Culture=neutral, PublicKeyToken=1a848309662c81e5
// MVID: 5B7ED1B4-25A8-4F03-9A92-3FFE69D0FD73
// Assembly location: C:\Users\Anatoliy\.nuget\packages\qliksense.netsdk\15.5.0\ref\netcoreapp2.1\Qlik.Sense.JsonRpc.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MyRequest
{
    public class Request
    {
        private static readonly object Lock = new object();
        private static int _id;

        internal Dictionary<string, object> Call { get; }

        internal static void ResetId()
        {
            object obj = Request.Lock;
            bool lockTaken = false;
            try
            {
                Monitor.Enter(obj, ref lockTaken);
                Request._id = 0;
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit(obj);
            }
        }

        public int Id => (int)this.Call["id"];

        public Request(
          int wsHandle,
          string wsMethod,
          IEnumerable<string> argumentNames = null,
          params object[] arguments)
          : this(wsHandle, wsMethod, new bool?(), argumentNames, arguments)
        {
        }

        private static int GetNewRequestId()
        {
            object obj = Request.Lock;
            bool lockTaken = false;
            try
            {
                Monitor.Enter(obj, ref lockTaken);
                return Request._id++;
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit(obj);
            }
        }

        public Request(
          int wsHandle,
          string wsMethod,
          bool? wsDelta,
          IEnumerable<string> argumentNames = null,
          params object[] arguments)
        {
            arguments = ((IEnumerable<object>)arguments).Reverse<object>().SkipWhile<object>((Func<object, bool>)(o => o == null)).Reverse<object>().ToArray<object>();
            object obj;
            if (((IEnumerable<object>)arguments).All<object>((Func<object, bool>)(o => o != null)))
            {
                obj = (object)arguments;
            }
            else
            {
                if (argumentNames == null)
                    throw new ArgumentException("Call to method " + wsMethod + " requires positional argument names.");
                obj = (object)argumentNames.Zip((IEnumerable<object>)arguments, (k, v) => new
                {
                    k = k,
                    v = v
                }).Where(x => x.v != null).ToDictionary(x => x.k, x => x.v);
            }
            this.Call = new Dictionary<string, object>()
      {
        {
          "jsonrpc",
          (object) "2.0"
        },
        {
          "id",
          (object) Request.GetNewRequestId()
        },
        {
          "method",
          (object) wsMethod
        },
        {
          "handle",
          (object) wsHandle
        },
        {
          "params",
          obj
        }
      };
            if (!wsDelta.HasValue)
                return;
            this.Call.Add("delta", (object)wsDelta.Value);
        }

        internal Request(Request src)
        {
            this.Call = new Dictionary<string, object>((IDictionary<string, object>)src.Call);
            this.Call["id"] = (object)Request.GetNewRequestId();
        }
    }
}
