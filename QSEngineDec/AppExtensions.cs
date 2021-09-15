// Decompiled with JetBrains decompiler
// Type: Qlik.Sense.Client.AppExtensions
// Assembly: Qlik.Sense.Client, Version=15.4.2.0, Culture=neutral, PublicKeyToken=1a848309662c81e5
// MVID: 0BCA0766-2D0C-4B49-A1C0-49218A5AE2DB
// Assembly location: C:\Users\Anatoliy\.nuget\packages\qliksense.netsdk\15.4.2\ref\netcoreapp2.1\Qlik.Sense.Client.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Qlik.Engine;
using Qlik.Engine.Communication;
using Qlik.Sense.Client;
using Qlik.Sense.Client.Snapshot;
using Qlik.Sense.Client.Storytelling;
using Qlik.Sense.Client.Visualizations;
using Qlik.Sense.Client.Visualizations.MapComponents;
using Qlik.Sense.JsonRpc;
using Bookmark = Qlik.Sense.Client.Bookmark;
using BookmarkList = Qlik.Sense.Client.BookmarkList;
using DimensionList = Qlik.Sense.Client.DimensionList;
using FieldList = Qlik.Sense.Client.FieldList;
using IBookmark = Qlik.Sense.Client.IBookmark;
using IBookmarkList = Qlik.Sense.Client.IBookmarkList;
using IDimensionList = Qlik.Sense.Client.IDimensionList;
using IFieldList = Qlik.Sense.Client.IFieldList;
using IMeasureList = Qlik.Sense.Client.IMeasureList;
using IUndoInfo = Qlik.Sense.Client.IUndoInfo;
using IVariable = Qlik.Sense.Client.IVariable;
using IVariableList = Qlik.Sense.Client.IVariableList;
using MeasureList = Qlik.Sense.Client.MeasureList;
using UndoInfo = Qlik.Sense.Client.UndoInfo;
using Variable = Qlik.Sense.Client.Variable;
using VariableList = Qlik.Sense.Client.VariableList;
#pragma warning disable 618

namespace QSEngineDec
{
  public static class AppExtensions
  {
    private const float SheetTitleSize = 36f;
    private const float LegtPaddingSize = 10f;
    private const float RightPaddingSize = 10f;
    private const float TopPaddingSize = 10f;
    private const float BottomPaddingSize = 10f;
    private const float TitleSize = 27f;
    private const float FootNoteSize = 22f;
    private const float SubtitleSize = 22f;
    private static SnapshotObjectSizeDef _screenSizeType;

    private static SnapshotObjectSizeDef GetScreenSize()
    {
      if (AppExtensions._screenSizeType != null)
        return AppExtensions._screenSizeType;
      return new SnapshotObjectSizeDef()
      {
        H = 1080f,
        W = 1920f
      };
    }

    private static void SetScreenSize(SnapshotObjectSizeDef sizeType)
    {
      if (sizeType == null)
        return;
      AppExtensions._screenSizeType = sizeType;
    }

    [Obsolete("Use GetGenericObject")]
    public static IMasterObject GetMasterObject(this IApp theApp, string id) => QlikConnection.AwaitResponse<IMasterObject>(theApp.GetMasterObjectAsync(id), nameof (GetMasterObject), theApp.Session.CancellationToken);

    [Obsolete("Use GetGenericObjectAsync")]
    public static Task<IMasterObject> GetMasterObjectAsync(
      this IApp theApp,
      string id)
    {
      return theApp.GetMasterObjectAsync((AsyncHandle) null, id);
    }

    [Obsolete("Use GetGenericObjectAsync")]
    public static Task<IMasterObject> GetMasterObjectAsync(
      this IApp theApp,
      AsyncHandle asyncHandle,
      string id)
    {
      return theApp.GetGenericObjectAsync<IMasterObject>(asyncHandle, new Func<Qlik.Engine.Communication.IO.Response, IMasterObject>(AppExtensions.OnGetMasterObjectAsync), id);
    }

    private static IMasterObject OnGetMasterObjectAsync(Qlik.Engine.Communication.IO.Response response) => (IMasterObject) response.Result<MasterObject>();

    [Obsolete("Use engine endpoints to perform this operation.")]
    public static IMasterObject CreateAndLinkMasterObject(
      this IApp theApp,
      string sourceId,
      string id,
      Qlik.Sense.Client.MasterObjectProperties properties)
    {
      return QlikConnection.AwaitResponse<IMasterObject>(theApp.CreateAndLinkMasterObjectAsync(sourceId, id, properties), nameof (CreateAndLinkMasterObject), theApp.Session.CancellationToken);
    }

    [Obsolete("Use engine endpoints to perform this operation.")]
    public static async Task<IMasterObject> CreateAndLinkMasterObjectAsync(
      this IApp theApp,
      string sourceId,
      string id,
      Qlik.Sense.Client.MasterObjectProperties properties)
    {
      IMasterObject masterObject = await theApp.CreateMasterObjectAsync(sourceId, id, properties).ConfigureAwait(false);
      await theApp.LinkMasterObjectAsync(sourceId, masterObject.Id).ConfigureAwait(false);
      IMasterObject masterObject1 = masterObject;
      masterObject = (IMasterObject) null;
      return masterObject1;
    }

    private static IMasterObject OnCreateAndLinkMasterObjectAsync(Qlik.Engine.Communication.IO.Response response) => (IMasterObject) response.Result<MasterObject>();

    [Obsolete("Use engine endpoints to perform this operation.")]
    public static IMasterObject CreateMasterObject(
      this IApp theApp,
      string sourceId,
      string id,
      Qlik.Sense.Client.MasterObjectProperties properties)
    {
      return QlikConnection.AwaitResponse<IMasterObject>(theApp.CreateMasterObjectAsync(sourceId, id, properties), nameof (CreateMasterObject), theApp.Session.CancellationToken);
    }

    [Obsolete("Use engine endpoints to perform this operation.")]
    public static async Task<IMasterObject> CreateMasterObjectAsync(
      this IApp theApp,
      string sourceId,
      string id,
      Qlik.Sense.Client.MasterObjectProperties properties)
    {
      List<NxPatch> patches;
      Qlik.Sense.Client.MasterObjectProperties prop = AppExtensions.MasterObjectProperties(id, properties, out patches);
      IMasterObject masterObject = await theApp.CreateGenericObjectAsync<IMasterObject>((AsyncHandle) null, new Func<Qlik.Engine.Communication.IO.Response, IMasterObject>(AppExtensions.OnCreateMasterObjectAsync), (GenericObjectProperties) prop).ConfigureAwait(false);
      ConfiguredTaskAwaitable configuredTaskAwaitable = masterObject.CopyFromAsync(sourceId).ConfigureAwait(false);
      await configuredTaskAwaitable;
      configuredTaskAwaitable = masterObject.ApplyPatchesAsync((IEnumerable<NxPatch>) patches.ToArray()).ConfigureAwait(false);
      await configuredTaskAwaitable;
      IMasterObject masterObject1 = masterObject;
      patches = (List<NxPatch>) null;
      prop = (Qlik.Sense.Client.MasterObjectProperties) null;
      masterObject = (IMasterObject) null;
      return masterObject1;
    }

    private static Qlik.Sense.Client.MasterObjectProperties MasterObjectProperties(
      string id,
      Qlik.Sense.Client.MasterObjectProperties properties,
      out List<NxPatch> patches)
    {
      Qlik.Sense.Client.MasterObjectProperties objectProperties = properties ?? new Qlik.Sense.Client.MasterObjectProperties();
      if (objectProperties.Info == null)
        objectProperties.Info = new NxInfo();
      if (objectProperties.Info.Id == null)
      {
        if (id != null)
          objectProperties.Info.Id = id.Replace(" ", "");
      }
      else
        objectProperties.Info.Id = objectProperties.Info.Id.Replace(" ", "");
      objectProperties.Info.Type = "masterobject";
      patches = new List<NxPatch>()
      {
        new NxPatch()
        {
          Op = NxPatchOperationType.Add,
          Path = "/masterVersion",
          Value = "0.96"
        },
        new NxPatch()
        {
          Op = NxPatchOperationType.Add,
          Path = "/qMetaDef/title",
          Value = "\"" + objectProperties.MetaDef.Title + "\""
        },
        new NxPatch()
        {
          Op = NxPatchOperationType.Add,
          Path = "/qMetaDef/description",
          Value = "\"" + objectProperties.MetaDef.Description + "\""
        }
      };
      string seed = "";
      if (objectProperties.MetaDef.Tags != null)
        seed = objectProperties.MetaDef.Tags.Aggregate<string, string>(seed, (Func<string, string, string>) ((current, t) => current + "\"" + t + "\",")).TrimEnd(',');
      patches.Add(new NxPatch()
      {
        Op = NxPatchOperationType.Add,
        Path = "/qMetaDef/tags",
        Value = "[ " + seed.TrimEnd(',') + " ]"
      });
      return objectProperties;
    }

    private static IMasterObject OnCreateMasterObjectAsync(Qlik.Engine.Communication.IO.Response response) => (IMasterObject) response.Result<MasterObject>();

    [Obsolete("Use engine endpoints to perform this operation.")]
    public static void LinkMasterObject(this IApp theApp, string sourceId, string masterId)
    {
      if (!theApp.LinkMasterObjectAsync(sourceId, masterId).Wait(RpcConnection.Timeout))
        throw new TimeoutException("Method \"LinkMasterObject\" timed out");
    }

    [Obsolete("Use engine endpoints to perform this operation.")]
    public static async Task LinkMasterObjectAsync(
      this IApp theApp,
      string sourceId,
      string masterId)
    {
      GenericObject obj = await theApp.GetGenericObjectAsync(sourceId).ConfigureAwait(false);
      GenericObjectProperties props = await obj.GetPropertiesAsync().ConfigureAwait(false);
      await obj.DestroyAllChildrenAsync(new GenericObjectProperties()
      {
        Info = props.Info,
        ExtendsId = masterId
      }).ConfigureAwait(false);
      obj = (GenericObject) null;
      props = (GenericObjectProperties) null;
    }

    [Obsolete("Use engine endpoints to perform this operation.")]
    public static void UnlinkMasterObject(this IApp theApp, string id)
    {
      if (!theApp.UnlinkMasterObjectAsync(id).Wait(RpcConnection.Timeout))
        throw new TimeoutException("Method \"UnlinkMasterObject\" timed out");
    }

    [Obsolete("Use engine endpoints to perform this operation.")]
    public static async Task UnlinkMasterObjectAsync(this IApp theApp, string id)
    {
      GenericObject obj = await theApp.GetGenericObjectAsync(id).ConfigureAwait(false);
      await obj.CopyFromAsync(obj.Properties.ExtendsId).ConfigureAwait(false);
      obj = (GenericObject) null;
    }

    [Obsolete("Use engine endpoints to perform this operation.")]
    public static bool RemoveMasterObject(this IApp theApp, string id) => QlikConnection.AwaitResponse<bool>(theApp.RemoveMasterObjectAsync(id), nameof (RemoveMasterObject), theApp.Session.CancellationToken);

    [Obsolete("Use engine endpoints to perform this operation.")]
    public static async Task<bool> RemoveMasterObjectAsync(this IApp theApp, string id = null)
    {
      if (id == null)
        return false;
      MasterObject obj = await theApp.GetObjectAsync<MasterObject>(id).ConfigureAwait(false);
      IEnumerable<NxLinkedObjectInfo> linkedObjects = await obj.GetLinkedObjectsAsync().ConfigureAwait(false);
      IEnumerable<Task> allTasks = linkedObjects.Select<NxLinkedObjectInfo, Task>((Func<NxLinkedObjectInfo, Task>) (async o =>
      {
        GenericObject genericObject = await theApp.GetObjectAsync<GenericObject>(o.Info.Id).ConfigureAwait(false);
        await genericObject.CopyFromAsync(id).ConfigureAwait(false);
        genericObject = (GenericObject) null;
      }));
      await Task.WhenAll(allTasks).ConfigureAwait(false);
      bool flag = await theApp.DestroyGenericObjectAsync(id).ConfigureAwait(false);
      return flag;
    }

    [Obsolete("Use GetGenericDimension")]
    public static IDimension GetDimension(this IApp theApp, string id) => QlikConnection.AwaitResponse<IDimension>(theApp.GetDimensionAsync(id), nameof (GetDimension), theApp.Session.CancellationToken);

    [Obsolete("Use GetGenericDimensionAsync")]
    public static Task<IDimension> GetDimensionAsync(this IApp theApp, string id) => theApp.GetDimensionAsync((AsyncHandle) null, id);

    [Obsolete("Use GetGenericDimensionAsync")]
    public static Task<IDimension> GetDimensionAsync(
      this IApp theApp,
      AsyncHandle asyncHandle,
      string id)
    {
      return theApp.GetGenericDimensionAsync<IDimension>(asyncHandle, new Func<Qlik.Engine.Communication.IO.Response, IDimension>(AppExtensions.OnGetDimensionAsync), id);
    }

    private static IDimension OnGetDimensionAsync(Qlik.Engine.Communication.IO.Response response) => (IDimension) response.Result<Dimension>();

    [Obsolete("Use CreateGenericDimension")]
    public static IDimension CreateDimension(
      this IApp theApp,
      string id,
      DimensionProperties properties)
    {
      return QlikConnection.AwaitResponse<IDimension>(theApp.CreateDimensionAsync(id, properties), nameof (CreateDimension), theApp.Session.CancellationToken);
    }

    [Obsolete("Use CreateGenericDimensionAsync")]
    public static Task<IDimension> CreateDimensionAsync(
      this IApp theApp,
      string id,
      DimensionProperties properties)
    {
      DimensionProperties dimensionProperties = properties ?? new DimensionProperties();
      if (dimensionProperties.Info == null)
        dimensionProperties.Info = new NxInfo();
      if (dimensionProperties.Info.Id == null)
      {
        if (id != null)
          dimensionProperties.Info.Id = id.Replace(" ", "");
      }
      else
        dimensionProperties.Info.Id = dimensionProperties.Info.Id.Replace(" ", "");
      dimensionProperties.Info.Type = "dimension";
      if (dimensionProperties.MetaDef.Tags == null)
        dimensionProperties.MetaDef.Tags = (IEnumerable<string>) new string[0];
      if (dimensionProperties.Dim == null)
        dimensionProperties.Dim = new NxLibraryDimensionDef();
      if (dimensionProperties.Dim.FieldDefs == null)
        dimensionProperties.Dim.FieldDefs = (IEnumerable<string>) new string[0];
      if (dimensionProperties.Dim.FieldLabels == null)
        dimensionProperties.Dim.FieldLabels = (IEnumerable<string>) new string[0];
      return theApp.CreateGenericDimensionAsync<IDimension>((AsyncHandle) null, new Func<Qlik.Engine.Communication.IO.Response, IDimension>(AppExtensions.OnCreateDimensionAsync), (GenericDimensionProperties) dimensionProperties);
    }

    private static IDimension OnCreateDimensionAsync(Qlik.Engine.Communication.IO.Response response) => (IDimension) response.Result<Dimension>();

    [Obsolete("Use CreateGenericDimension")]
    public static IDimension CreateDimension(
      this IApp theApp,
      string id,
      string dimension)
    {
      return theApp.CreateDimension(id, (DimensionProperties) AppExtensions.CreateDimensionProperties(id, (IEnumerable<string>) new string[1]
      {
        dimension
      }));
    }

    [Obsolete("Use CreateGenericDimensionAsync")]
    public static Task<IDimension> CreateDimensionAsync(
      this IApp theApp,
      string id,
      string dimension)
    {
      return theApp.CreateDimensionAsync(id, (DimensionProperties) AppExtensions.CreateDimensionProperties(id, (IEnumerable<string>) new string[1]
      {
        dimension
      }));
    }

    [Obsolete("Use CreateGenericDimension")]
    public static IDimension CreateDimension(
      this IApp theApp,
      string id,
      IEnumerable<string> fields)
    {
      return theApp.CreateDimension(id, (DimensionProperties) AppExtensions.CreateDimensionProperties(id, fields));
    }

    [Obsolete("Use CreateGenericDimensionAsync")]
    public static Task<IDimension> CreateDimensionAsync(
      this IApp theApp,
      string id,
      IEnumerable<string> fields)
    {
      return theApp.CreateDimensionAsync(id, (DimensionProperties) AppExtensions.CreateDimensionProperties(id, fields));
    }

    private static IDimensionProperties CreateDimensionProperties(
      string id,
      IEnumerable<string> dimensions)
    {
      DimensionProperties dimensionProperties1 = new DimensionProperties();
      dimensionProperties1.Info = new NxInfo();
      dimensionProperties1.MetaDef = new MetaAttributesDef();
      DimensionProperties dimensionProperties2 = dimensionProperties1;
      dimensionProperties2.Info.Id = id != null ? id.Replace(" ", "") : ClientExtension.GetCid();
      dimensionProperties2.Info.Type = "dimension";
      dimensionProperties2.MetaDef.Tags = (IEnumerable<string>) new string[0];
      dimensionProperties2.MetaDef.Title = id;
      dimensionProperties2.MetaDef.Title = id;
      DimensionProperties dimensionProperties3 = dimensionProperties2;
      NxLibraryDimensionDef libraryDimensionDef;
      if (dimensions == null)
        libraryDimensionDef = new NxLibraryDimensionDef()
        {
          FieldDefs = (IEnumerable<string>) new string[0],
          FieldLabels = (IEnumerable<string>) new string[0]
        };
      else
        libraryDimensionDef = new NxLibraryDimensionDef()
        {
          FieldDefs = dimensions,
          FieldLabels = (IEnumerable<string>) new string[0]
        };
      dimensionProperties3.Dim = libraryDimensionDef;
      return (IDimensionProperties) dimensionProperties2;
    }

    [Obsolete("Use DestroyGenericDimension")]
    public static bool RemoveDimension(this IApp theApp, string id) => QlikConnection.AwaitResponse<bool>(theApp.RemoveDimensionAsync(id), nameof (RemoveDimension), theApp.Session.CancellationToken);

    [Obsolete("Use DestroyGenericDimensionAsync")]
    public static Task<bool> RemoveDimensionAsync(this IApp theApp, string id = null) => theApp.DestroyGenericDimensionAsync(id);

    [Obsolete("Use GetGenericMeasure")]
    public static IMeasure GetMeasure(this IApp theApp, string id) => QlikConnection.AwaitResponse<IMeasure>(theApp.GetMeasureAsync(id), nameof (GetMeasure), theApp.Session.CancellationToken);

    [Obsolete("Use GetGenericMeasureAsync")]
    public static Task<IMeasure> GetMeasureAsync(this IApp theApp, string id) => theApp.GetMeasureAsync((AsyncHandle) null, id);

    [Obsolete("Use GetGenericMeasureAsync")]
    public static Task<IMeasure> GetMeasureAsync(
      this IApp theApp,
      AsyncHandle asyncHandle,
      string id)
    {
      return theApp.GetGenericMeasureAsync<IMeasure>(asyncHandle, new Func<Qlik.Engine.Communication.IO.Response, IMeasure>(AppExtensions.OnGetMeasureAsync), id);
    }

    private static IMeasure OnGetMeasureAsync(Qlik.Engine.Communication.IO.Response response) => (IMeasure) response.Result<Measure>();

    [Obsolete("Use CreateGenericMeasure")]
    public static IMeasure CreateMeasure(
      this IApp theApp,
      string id,
      MeasureProperties properties)
    {
      return QlikConnection.AwaitResponse<IMeasure>(theApp.CreateMeasureAsync(id, properties), nameof (CreateMeasure), theApp.Session.CancellationToken);
    }

    [Obsolete("Use CreateGenericMeasureAsync")]
    public static Task<IMeasure> CreateMeasureAsync(
      this IApp theApp,
      string id,
      MeasureProperties properties)
    {
      MeasureProperties measureProperties = properties ?? new MeasureProperties();
      if (measureProperties.Info == null)
        measureProperties.Info = new NxInfo();
      if (measureProperties.Info.Id == null)
      {
        if (id != null)
          measureProperties.Info.Id = id.Replace(" ", "");
      }
      else
        measureProperties.Info.Id = measureProperties.Info.Id.Replace(" ", "");
      if (measureProperties.MetaDef.Tags == null)
        measureProperties.MetaDef.Tags = (IEnumerable<string>) new string[0];
      measureProperties.Info.Type = "measure";
      return theApp.CreateGenericMeasureAsync<IMeasure>((AsyncHandle) null, new Func<Qlik.Engine.Communication.IO.Response, IMeasure>(AppExtensions.OnCreateMeasureAsync), (GenericMeasureProperties) measureProperties);
    }

    private static IMeasure OnCreateMeasureAsync(Qlik.Engine.Communication.IO.Response response) => (IMeasure) response.Result<Measure>();

    [Obsolete("Use CreateGenericMeasure")]
    public static IMeasure CreateMeasure(
      this IApp theApp,
      string id,
      string stringExpression)
    {
      return theApp.CreateMeasure(id, (MeasureProperties) AppExtensions.CreateMeasureProperties(id, stringExpression));
    }

    [Obsolete("Use CreateGenericMeasureAsync")]
    public static Task<IMeasure> CreateMeasureAsync(
      this IApp theApp,
      string id,
      string stringExpression)
    {
      return theApp.CreateMeasureAsync(id, (MeasureProperties) AppExtensions.CreateMeasureProperties(id, stringExpression));
    }

    private static IMeasureProperties CreateMeasureProperties(
      string id,
      string measure)
    {
      MeasureProperties measureProperties1 = new MeasureProperties();
      measureProperties1.Info = new NxInfo();
      measureProperties1.MetaDef = new MetaAttributesDef();
      MeasureProperties measureProperties2 = measureProperties1;
      measureProperties2.Info.Id = id != null ? id.Replace(" ", "") : ClientExtension.GetCid();
      measureProperties2.Info.Type = nameof (measure);
      measureProperties2.MetaDef.Title = id;
      measureProperties2.Measure = new NxLibraryMeasureDef()
      {
        Def = measure,
        Label = measure
      };
      return (IMeasureProperties) measureProperties2;
    }

    [Obsolete("Use DestroyGenericMeasure")]
    public static bool RemoveMeasure(this IApp theApp, string id) => QlikConnection.AwaitResponse<bool>(theApp.RemoveMeasureAsync(id), nameof (RemoveMeasure), theApp.Session.CancellationToken);

    [Obsolete("Use DestroyGenericMeasureAsync")]
    public static Task<bool> RemoveMeasureAsync(this IApp theApp, string id = null) => theApp.DestroyGenericMeasureAsync(id);

    [Obsolete("Use Qlik.Engine.IApp.GetVariableById")]
    public static IVariable GetVariableById(this IApp theApp, string id) => QlikConnection.AwaitResponse<IVariable>(AppExtensions.GetVariableByIdAsync(theApp, id), nameof (GetVariableById), theApp.Session.CancellationToken);

    [Obsolete("Use Qlik.Engine.IApp.GetVariableByIdAsync")]
    public static Task<IVariable> GetVariableByIdAsync(this IApp theApp, string id) => AppExtensions.GetVariableByIdAsync(theApp, (AsyncHandle) null, id);

    [Obsolete("Use Qlik.Engine.IApp.GetVariableByIdAsync")]
    public static Task<IVariable> GetVariableByIdAsync(
      this IApp theApp,
      AsyncHandle asyncHandle,
      string id)
    {
      return theApp.GetVariableByIdAsync<IVariable>(asyncHandle, new Func<Qlik.Engine.Communication.IO.Response, IVariable>(AppExtensions.OnGetVariableAsync), id);
    }

    private static IVariable OnGetVariableAsync(Qlik.Engine.Communication.IO.Response response) => (IVariable) response.Result<Variable>();

    [Obsolete("Use Qlik.Engine.IApp.GetVariableByName")]
    public static IVariable GetVariableByName(this IApp theApp, string variableName) => QlikConnection.AwaitResponse<IVariable>(AppExtensions.GetVariableByNameAsync(theApp, variableName), nameof (GetVariableByName), theApp.Session.CancellationToken);

    [Obsolete("Use Qlik.Engine.IApp.GetVariableByNameAsync")]
    public static Task<IVariable> GetVariableByNameAsync(
      this IApp theApp,
      string variableName)
    {
      return AppExtensions.GetVariableByIdAsync(theApp, (AsyncHandle) null, variableName);
    }

    [Obsolete("Use Qlik.Engine.IApp.GetVariableByNameAsync")]
    public static Task<IVariable> GetVariableByNameAsync(
      this IApp theApp,
      AsyncHandle asyncHandle,
      string variableName)
    {
      return theApp.GetVariableByNameAsync<IVariable>(asyncHandle, new Func<Qlik.Engine.Communication.IO.Response, IVariable>(AppExtensions.OnGetVariableAsync), variableName);
    }

    [Obsolete("Use Qlik.Engine.IApp.DestroyVariableByName")]
    public static bool DestroyVariableById(this IApp theApp, string id) => QlikConnection.AwaitResponse<bool>(theApp.DestroyVariableByIdAsync(id), nameof (DestroyVariableById), theApp.Session.CancellationToken);

    [Obsolete("Use Qlik.Engine.IApp.DestroyVariableByNameAsync")]
    public static Task<bool> DestroyVariableByIdAsync(this IApp theApp, string id = null) => theApp.DestroyVariableByIdAsync(id);

    [Obsolete("Use Qlik.Engine.IApp.DestroyVariableByName")]
    public static bool DestroyVariableByName(this IApp theApp, string variableName) => QlikConnection.AwaitResponse<bool>(theApp.DestroyVariableByNameAsync(variableName), nameof (DestroyVariableByName), theApp.Session.CancellationToken);

    [Obsolete("Use Qlik.Engine.IApp.DestroyVariableByNameAsync")]
    public static Task<bool> DestroyVariableByNameAsync(this IApp theApp, string variableName = null) => theApp.DestroyVariableByNameAsync(variableName);

    [Obsolete("Use Qlik.Engine.IApp.DestroySessionVariable")]
    public static bool DestroySessionVariableById(this IApp theApp, string id) => QlikConnection.AwaitResponse<bool>(theApp.DestroySessionVariableAsync(id), "DestroyVariableByName", theApp.Session.CancellationToken);

    [Obsolete("Use Qlik.Engine.IApp.DestroySessionVariableAsync")]
    public static Task<bool> DestroySessionVariableByIdAsync(this IApp theApp, string id = null) => theApp.DestroySessionVariableAsync(id);

    [Obsolete("Use Qlik.Engine.IApp.CreateSessionVariable")]
    public static IVariable CreateSessionVariable(
      this IApp theApp,
      string id,
      GenericVariableProperties properties)
    {
      return QlikConnection.AwaitResponse<IVariable>(theApp.CreateSessionVariableAsync(id, properties), nameof (CreateSessionVariable), theApp.Session.CancellationToken);
    }

    [Obsolete("Use Qlik.Engine.IApp.CreateSessionVariableAsync")]
    public static Task<IVariable> CreateSessionVariableAsync(
      this IApp theApp,
      string id,
      GenericVariableProperties properties)
    {
      GenericVariableProperties prop = AppExtensions.SetGenericVariableProperties(id, properties);
      return theApp.CreateSessionVariableAsync<IVariable>((AsyncHandle) null, new Func<Qlik.Engine.Communication.IO.Response, IVariable>(AppExtensions.OnCreateVariableAsync), prop);
    }

    [Obsolete("Use Qlik.Engine.IApp.CreateVariableEx")]
    public static IVariable CreateVariableExpression(
      this IApp theApp,
      string id,
      GenericVariableProperties properties)
    {
      return QlikConnection.AwaitResponse<IVariable>(theApp.CreateVariableExpressionAsync(id, properties), nameof (CreateVariableExpression), theApp.Session.CancellationToken);
    }

    [Obsolete("Use Qlik.Engine.IApp.CreateVariableExAsync")]
    public static Task<IVariable> CreateVariableExpressionAsync(
      this IApp theApp,
      string id,
      GenericVariableProperties properties)
    {
      GenericVariableProperties prop = AppExtensions.SetGenericVariableProperties(id, properties);
      return theApp.CreateVariableExAsync<IVariable>((AsyncHandle) null, new Func<Qlik.Engine.Communication.IO.Response, IVariable>(AppExtensions.OnCreateVariableAsync), prop);
    }

    private static GenericVariableProperties SetGenericVariableProperties(
      string id,
      GenericVariableProperties properties)
    {
      GenericVariableProperties variableProperties = properties ?? (GenericVariableProperties) new VariableProperties();
      if (variableProperties.Info == null)
        variableProperties.Info = new NxInfo();
      if (variableProperties.Info.Id == null)
      {
        if (id != null)
          variableProperties.Info.Id = id.Replace(" ", "");
      }
      else
        variableProperties.Info.Id = variableProperties.Info.Id.Replace(" ", "");
      variableProperties.Info.Type = "variable";
      return variableProperties;
    }

    private static IVariable OnCreateVariableAsync(Qlik.Engine.Communication.IO.Response response) => (IVariable) response.Result<Variable>();

    [Obsolete("Use Qlik.Engine.IApp.GetGenericBookmark")]
    public static IBookmark GetBookmark(this IApp theApp, string id) => QlikConnection.AwaitResponse<IBookmark>(theApp.GetBookmarkAsync(id), nameof (GetBookmark), theApp.Session.CancellationToken);

    [Obsolete("Use Qlik.Engine.IApp.GetGenericBookmarkAsync")]
    public static Task<IBookmark> GetBookmarkAsync(this IApp theApp, string id) => theApp.GetBookmarkAsync((AsyncHandle) null, id);

    [Obsolete("Use Qlik.Engine.IApp.GetGenericBookmarkAsync")]
    public static Task<IBookmark> GetBookmarkAsync(
      this IApp theApp,
      AsyncHandle asyncHandle,
      string id)
    {
      return theApp.GetGenericBookmarkAsync<IBookmark>(asyncHandle, new Func<Qlik.Engine.Communication.IO.Response, IBookmark>(AppExtensions.OnGetBookmarkAsync), id);
    }

    private static IBookmark OnGetBookmarkAsync(Qlik.Engine.Communication.IO.Response response) => (IBookmark) response.Result<Bookmark>();

    [Obsolete("Use Qlik.Engine.IApp.CreateGenericBookmark")]
    public static IBookmark CreateBookmark(
      this IApp theApp,
      string id,
      BookmarkProperties properties)
    {
      return QlikConnection.AwaitResponse<IBookmark>(theApp.CreateBookmarkAsync(id, properties), nameof (CreateBookmark), theApp.Session.CancellationToken);
    }

    [Obsolete("Use Qlik.Engine.IApp.CreateGenericBookmarkAsync")]
    public static Task<IBookmark> CreateBookmarkAsync(
      this IApp theApp,
      string id,
      BookmarkProperties properties)
    {
      BookmarkProperties bookmarkProperties = properties ?? new BookmarkProperties();
      if (bookmarkProperties.Info == null)
        bookmarkProperties.Info = new NxInfo();
      if (bookmarkProperties.Info.Id == null)
      {
        if (id != null)
          bookmarkProperties.Info.Id = id.Replace(" ", "");
      }
      else
        bookmarkProperties.Info.Id = bookmarkProperties.Info.Id.Replace(" ", "");
      bookmarkProperties.Info.Type = "bookmark";
      return theApp.CreateGenericBookmarkAsync<IBookmark>((AsyncHandle) null, new Func<Qlik.Engine.Communication.IO.Response, IBookmark>(AppExtensions.OnCreateBookmarkAsync), (GenericBookmarkProperties) bookmarkProperties);
    }

    public static IBookmark OnCreateBookmarkAsync(Qlik.Engine.Communication.IO.Response response) => (IBookmark) response.Result<Bookmark>();

    [Obsolete("Use Qlik.Engine.IApp.DestroyGenericBookmark")]
    public static bool RemoveBookmark(this IApp theApp, string id) => QlikConnection.AwaitResponse<bool>(theApp.RemoveBookmarkAsync(id), nameof (RemoveBookmark), theApp.Session.CancellationToken);

    [Obsolete("Use Qlik.Engine.IApp.DestroyGenericBookmarkAsync")]
    public static Task<bool> RemoveBookmarkAsync(this IApp theApp, string id = null) => theApp.DestroyGenericBookmarkAsync(id);

    [Obsolete("Use Qlik.Engine.IApp.ApplyGenericBookmark")]
    public static bool ApplyBookmark(this IApp theApp, string id) => QlikConnection.AwaitResponse<bool>(theApp.ApplyBookmarkAsync(id), nameof (ApplyBookmark), theApp.Session.CancellationToken);

    [Obsolete("Use Qlik.Engine.IApp.ApplyGenericBookmarkAsync")]
    public static Task<bool> ApplyBookmarkAsync(this IApp theApp, string id = null) => theApp.ApplyGenericBookmarkAsync(id);

    [Obsolete("Storytelling related methods will be removed from this SDK in future releases. Use GenericBookmark and AbstractStructure to interact with this client functionality.")]
    public static ISnapshot GetSnapshot(this IApp theApp, string id) => QlikConnection.AwaitResponse<ISnapshot>(theApp.GetSnapshotAsync(id), nameof (GetSnapshot), theApp.Session.CancellationToken);

    [Obsolete("Storytelling related methods will be removed from this SDK in future releases. Use GenericBookmark and AbstractStructure to interact with this client functionality.")]
    public static Task<ISnapshot> GetSnapshotAsync(this IApp theApp, string id) => theApp.GetSnapshotAsync((AsyncHandle) null, id);

    [Obsolete("Storytelling related methods will be removed from this SDK in future releases. Use GenericBookmark and AbstractStructure to interact with this client functionality.")]
    public static Task<ISnapshot> GetSnapshotAsync(
      this IApp theApp,
      AsyncHandle asyncHandle,
      string id)
    {
      return theApp.GetGenericBookmarkAsync<ISnapshot>(asyncHandle, new Func<Qlik.Engine.Communication.IO.Response, ISnapshot>(AppExtensions.OnGetSnapshotAsync), id);
    }

    private static ISnapshot OnGetSnapshotAsync(Qlik.Engine.Communication.IO.Response response) => (ISnapshot) response.Result<Qlik.Sense.Client.Snapshot.Snapshot>();

    [Obsolete("Storytelling related methods will be removed from this SDK in future releases. Use GenericBookmark and AbstractStructure to interact with this client functionality.")]
    public static bool RemoveSnapshot(this IApp theApp, string id) => QlikConnection.AwaitResponse<bool>(theApp.RemoveSnapshotAsync(id), nameof (RemoveSnapshot), theApp.Session.CancellationToken);

    [Obsolete("Storytelling related methods will be removed from this SDK in future releases. Use GenericBookmark and AbstractStructure to interact with this client functionality.")]
    public static Task<bool> RemoveSnapshotAsync(this IApp theApp, string id = null) => theApp.DestroyGenericBookmarkAsync(id);

    [Obsolete("Storytelling related methods will be removed from this SDK in future releases. Use GenericBookmark and AbstractStructure to interact with this client functionality.")]
    public static Task<ISnapshot> CreateSnapshotAsync(
      this IApp theApp,
      string id,
      SnapshotProperties properties)
    {
      SnapshotProperties snapshotProperties = properties ?? new SnapshotProperties();
      if (snapshotProperties.Info == null)
        snapshotProperties.Info = new NxInfo();
      if (snapshotProperties.Info.Id == null)
      {
        if (id != null)
          snapshotProperties.Info.Id = id.Replace(" ", "");
      }
      else
        snapshotProperties.Info.Id = snapshotProperties.Info.Id.Replace(" ", "");
      snapshotProperties.Info.Type = "snapshot";
      return theApp.CreateGenericBookmarkAsync<ISnapshot>((AsyncHandle) null, new Func<Qlik.Engine.Communication.IO.Response, ISnapshot>(AppExtensions.OnCreateSnapshotAsync), (GenericBookmarkProperties) snapshotProperties);
    }

    private static ISnapshot OnCreateSnapshotAsync(Qlik.Engine.Communication.IO.Response response) => (ISnapshot) response.Result<Qlik.Sense.Client.Snapshot.Snapshot>();

    [Obsolete("Storytelling related methods will be removed from this SDK in future releases. Use GenericBookmark and AbstractStructure to interact with this client functionality.")]
    public static ISnapshot CreateSnapshot(
      this IApp theApp,
      string id,
      SnapshotProperties properties)
    {
      Task<ISnapshot> snapshotAsync = theApp.CreateSnapshotAsync(id, properties);
      if (snapshotAsync.Wait(RpcConnection.Timeout, theApp.Session.CancellationToken))
        return snapshotAsync.Result;
      theApp.Session.CancellationToken.ThrowIfCancellationRequested();
      throw new TimeoutException("Method \"CreateSnapshot\" timed out");
    }

    [Obsolete("Storytelling related methods will be removed from this SDK in future releases. Use GenericBookmark and AbstractStructure to interact with this client functionality.")]
    public static ISnapshot CreateSnapshot(
      this IApp theApp,
      string snapshotLibId,
      string sheetId,
      string objectId,
      SnapshotObjectSizeDef screenSize = null)
    {
      GenericObject genericObject = theApp.GetObject<GenericObject>(objectId);
      string str = AppExtensions.GetvisualizationType(genericObject);
      SnapshotProperties snapshotProperties = genericObject.GetLayout().CloneAs<SnapshotProperties>();
      AppExtensions.SetScreenSize(screenSize);
      switch (str)
      {
        case "barchart":
        case "combochart":
        case "linechart":
          snapshotProperties = theApp.SetScrollableChartsSnapshotProperties(snapshotLibId, sheetId, objectId, snapshotProperties);
          break;
        case "gauge":
          snapshotProperties = theApp.SetGaugeSnapshotProperties(snapshotLibId, sheetId, objectId, snapshotProperties);
          break;
        case "kpi":
          snapshotProperties = theApp.SetKpiSnapshotProperties(snapshotLibId, sheetId, objectId, snapshotProperties);
          break;
        case "map":
          snapshotProperties = theApp.SetMapSnapshotProperties(snapshotLibId, sheetId, objectId, snapshotProperties);
          break;
        case "piechart":
          snapshotProperties = theApp.SetPiechartSnapshotProperties(snapshotLibId, sheetId, objectId, snapshotProperties);
          break;
        case "scatterplot":
          snapshotProperties = theApp.SetScatterplotSnapshotProperties(snapshotLibId, sheetId, objectId, snapshotProperties);
          break;
        case "table":
          snapshotProperties = theApp.SetTableSnapshotProperties(snapshotLibId, sheetId, objectId, snapshotProperties);
          break;
        case "text-image":
          snapshotProperties = theApp.SetTextImageSnapshotProperties(snapshotLibId, sheetId, objectId, snapshotProperties);
          break;
        case "treemap":
          snapshotProperties = theApp.SetTreemapSnapshotProperties(snapshotLibId, sheetId, objectId, snapshotProperties);
          break;
      }
      return theApp.CreateSnapshot(snapshotLibId, snapshotProperties);
    }

    private static SnapshotProperties SetScrollableChartsSnapshotProperties(
      this IApp theApp,
      string snapshotLibId,
      string sheetId,
      string objectId,
      SnapshotProperties snapshotProperties)
    {
      SnapshotProperties snapshotProperties1 = theApp.SetSnapshotProperties(snapshotLibId, sheetId, objectId, snapshotProperties);
      snapshotProperties1.SnapshotData.Content.ChartData.MapRect = (SnapshotMapRectDef) null;
      snapshotProperties1.SnapshotData.Content.ChartData.HasMiniChart = true;
      snapshotProperties1.SnapshotData.Content.ChartData.LegendScrollOffset = 0.0f;
      snapshotProperties1.SnapshotData.Content.ChartData.ScrollOffset = 0.0f;
      snapshotProperties1.SnapshotData.Content.ChartData.DiscreteSpacing = 0.0f;
      snapshotProperties1.SnapshotData.Content.ChartData.AxisInnerOffset = 0.0f;
      return snapshotProperties1;
    }

    private static SnapshotProperties SetScatterplotSnapshotProperties(
      this IApp theApp,
      string snapshotLibId,
      string sheetId,
      string objectId,
      SnapshotProperties snapshotProperties)
    {
      GenericObject genericObject = theApp.GetObject<GenericObject>(objectId);
      SnapshotProperties snapshotProperties1 = theApp.SetSnapshotProperties(snapshotLibId, sheetId, objectId, snapshotProperties);
      snapshotProperties1.SnapshotData.Content.ChartData.MapRect = (SnapshotMapRectDef) null;
      snapshotProperties1.SnapshotData.Content.ChartData.LegendScrollOffset = 0.0f;
      HyperCube hyperCube = snapshotProperties1.Get<HyperCube>("qHyperCube");
      Size size = hyperCube.Size;
      int cx = size.cx;
      int cy = size.cy;
      List<NxPage> nxPageList = new List<NxPage>()
      {
        new NxPage() { Height = cy, Width = cx, Left = 0, Top = 0 }
      };
      IEnumerable<NxDataPage> hyperCubeData = genericObject.GetHyperCubeData("/qHyperCubeDef", (IEnumerable<NxPage>) nxPageList);
      hyperCube.DataPages = hyperCubeData;
      hyperCube.Set<bool>("isBigData", false);
      hyperCube.Set<bool>("isReduced", false);
      return snapshotProperties1;
    }

    private static SnapshotProperties SetPiechartSnapshotProperties(
      this IApp theApp,
      string snapshotLibId,
      string sheetId,
      string objectId,
      SnapshotProperties snapshotProperties)
    {
      SnapshotProperties snapshotProperties1 = theApp.SetSnapshotProperties(snapshotLibId, sheetId, objectId, snapshotProperties);
      snapshotProperties1.SnapshotData.Content.ChartData.MapRect = (SnapshotMapRectDef) null;
      snapshotProperties1.SnapshotData.Content.ChartData.LegendScrollOffset = 0.0f;
      snapshotProperties1.SnapshotData.Content.ChartData.Rotation = 0.0f;
      return snapshotProperties1;
    }

    private static SnapshotProperties SetMapSnapshotProperties(
      this IApp theApp,
      string snapshotLibId,
      string sheetId,
      string objectId,
      SnapshotProperties snapshotProperties)
    {
      GenericObject genericObject = theApp.GetObject<GenericObject>(objectId);
      SnapshotProperties snapshotProperties1 = theApp.SetSnapshotProperties(snapshotLibId, sheetId, objectId, snapshotProperties);
      Polygon3LayerDef polygon3LayerDef = snapshotProperties1.Get<IEnumerable<Polygon3LayerDef>>("layers").First<Polygon3LayerDef>();
      if (polygon3LayerDef.Get<LayerType>("type") == LayerType.Polygon)
      {
        HyperCube hyperCube = polygon3LayerDef.Get<Polygon3LayerGeodata>("geodata").Get<HyperCube>("qHyperCube");
        Size size = hyperCube.Size;
        int cx = size.cx;
        int cy = size.cy;
        List<NxPage> nxPageList = new List<NxPage>()
        {
          new NxPage() { Height = cy, Width = cx, Left = 0, Top = 0 }
        };
        IEnumerable<NxDataPage> hyperCubeData = genericObject.GetHyperCubeData("/layers/0/geodata/qHyperCubeDef", (IEnumerable<NxPage>) nxPageList);
        hyperCube.DataPages = hyperCubeData;
      }
      snapshotProperties1.SnapshotData.Content.ChartData.LegendScrollOffset = 0.0f;
      return snapshotProperties1;
    }

    private static SnapshotProperties SetTableSnapshotProperties(
      this IApp theApp,
      string snapshotLibId,
      string sheetId,
      string objectId,
      SnapshotProperties snapshotProps)
    {
      GenericObject genericObject = theApp.GetObject<GenericObject>(objectId);
      theApp.SetSnapshotPropertiesWithoutContent(snapshotLibId, sheetId, objectId, snapshotProps);
      TableProperties tableProperties = genericObject.Properties.CloneAs<TableProperties>();
      int count = tableProperties.HyperCubeDef.Dimensions.Count<TableHyperCubeDimensionDef>() + tableProperties.HyperCubeDef.Measures.Count<TableHyperCubeMeasureDef>();
      float h = theApp.GetObjectSize(sheetId, objectId).H;
      Tuple<float, float, float> objectTitlesSize = theApp.GetObjectTitlesSize(objectId);
      float num1 = objectTitlesSize.Item1;
      float num2 = objectTitlesSize.Item2;
      float num3 = objectTitlesSize.Item3;
      float num4 = (float) (((double) h - (double) num1 - (double) num2 - (double) num3) / 21.0);
      List<NxPage> nxPageList = new List<NxPage>();
      for (int index = 0; index < count; ++index)
        nxPageList.Add(new NxPage()
        {
          Height = (int) num4,
          Width = 1,
          Left = index,
          Top = 0
        });
      HyperCube hyperCube = snapshotProps.Get<HyperCube>("qHyperCube");
      IEnumerable<NxDataPage> hyperCubeData = genericObject.GetHyperCubeData("/qHyperCubeDef", (IEnumerable<NxPage>) nxPageList);
      hyperCube.Set<List<int>>("columnOrder", Enumerable.Range(0, count).ToList<int>());
      hyperCube.DataPages = hyperCubeData;
      for (int index = 0; index < hyperCubeData.Count<NxDataPage>(); ++index)
        hyperCube.DataPages.ElementAt<NxDataPage>(index).Tails = (IEnumerable<NxGroupTail>) new List<NxGroupTail>();
      snapshotProps.SnapshotData.Content = (SnapshotContentDef) null;
      return snapshotProps;
    }

    private static SnapshotProperties SetGaugeSnapshotProperties(
      this IApp theApp,
      string snapshotLibId,
      string sheetId,
      string objectId,
      SnapshotProperties snapshotProperties)
    {
      SnapshotProperties snapshotProperties1 = theApp.SetSnapshotProperties(snapshotLibId, sheetId, objectId, snapshotProperties);
      snapshotProperties1.SnapshotData.Content.ChartData.MapRect = (SnapshotMapRectDef) null;
      return snapshotProperties1;
    }

    private static SnapshotProperties SetKpiSnapshotProperties(
      this IApp theApp,
      string snapshotLibId,
      string sheetId,
      string objectId,
      SnapshotProperties snapshotProperties)
    {
      SnapshotProperties snapshotProperties1 = theApp.SetSnapshotPropertiesWithoutContent(snapshotLibId, sheetId, objectId, snapshotProperties);
      snapshotProperties1.SnapshotData.Content = (SnapshotContentDef) null;
      snapshotProperties1.SnapshotData.IsZoomed = false;
      snapshotProperties1.SnapshotData.ElementRatio = 0.0f;
      return snapshotProperties1;
    }

    private static SnapshotProperties SetTextImageSnapshotProperties(
      this IApp theApp,
      string snapshotLibId,
      string sheetId,
      string objectId,
      SnapshotProperties snapshotProperties)
    {
      SnapshotProperties snapshotProperties1 = theApp.SetSnapshotPropertiesWithoutContent(snapshotLibId, sheetId, objectId, snapshotProperties);
      snapshotProperties1.SnapshotData.ScrollTopPercent = 0.0f;
      snapshotProperties1.SnapshotData.VerticalScrollWidth = 0.0f;
      return snapshotProperties1;
    }

    private static SnapshotProperties SetTreemapSnapshotProperties(
      this IApp theApp,
      string snapshotLibId,
      string sheetId,
      string objectId,
      SnapshotProperties snapshotProperties)
    {
      SnapshotProperties snapshotProperties1 = theApp.SetSnapshotProperties(snapshotLibId, sheetId, objectId, snapshotProperties);
      snapshotProperties1.SnapshotData.Content.ChartData.MapRect = (SnapshotMapRectDef) null;
      snapshotProperties1.SnapshotData.Content.ChartData.LegendScrollOffset = 0.0f;
      return snapshotProperties1;
    }

    private static SnapshotProperties SetSnapshotProperties(
      this IApp theApp,
      string snapshotLibId,
      string sheetId,
      string objectId,
      SnapshotProperties snapshotProps)
    {
      theApp.SetGeneralSnapshotProperties(snapshotLibId, sheetId, objectId, snapshotProps);
      theApp.SetSnapshotParentSize(sheetId, (ISnapshotProperties) snapshotProps);
      theApp.SetSnapshotObjectSize(sheetId, objectId, snapshotProps);
      theApp.SetSnapshotContentSize(sheetId, objectId, snapshotProps);
      return snapshotProps;
    }

    private static SnapshotProperties SetSnapshotPropertiesWithoutContent(
      this IApp theApp,
      string snapshotLibId,
      string sheetId,
      string objectId,
      SnapshotProperties snapshotProps)
    {
      theApp.SetGeneralSnapshotProperties(snapshotLibId, sheetId, objectId, snapshotProps);
      theApp.SetSnapshotParentSize(sheetId, (ISnapshotProperties) snapshotProps);
      theApp.SetSnapshotObjectSize(sheetId, objectId, snapshotProps);
      snapshotProps.SnapshotData.Content = (SnapshotContentDef) null;
      return snapshotProps;
    }

    private static void SetGeneralSnapshotProperties(
      this IApp theApp,
      string snapshotLibId,
      string sheetId,
      string objectId,
      SnapshotProperties snapshotProps)
    {
      GenericObject genericObject = theApp.GetObject<GenericObject>(objectId);
      snapshotProps.SheetId = sheetId;
      snapshotProps.SourceObjectId = objectId;
      snapshotProps.MetaDef = new MetaAttributesDef()
      {
        Title = snapshotProps.Title
      };
      snapshotProps.Info = new NxInfo()
      {
        Id = snapshotLibId,
        Type = "snapshot"
      };
      snapshotProps.VisualizationType = AppExtensions.GetvisualizationType(genericObject);
      snapshotProps.Timestamp = (float) DateTime.Now.ToJavascriptTimestamp();
      snapshotProps.IsClone = false;
    }

    private static void SetSnapshotParentSize(
      this IApp theApp,
      string sheetId,
      ISnapshotProperties snapshotProps)
    {
      SnapshotObjectSizeDef parentSize = theApp.GetParentSize(sheetId);
      if (snapshotProps.SnapshotData == null)
        snapshotProps.SnapshotData = new SnapshotDataDef();
      snapshotProps.SnapshotData.Parent.H = parentSize.H;
      snapshotProps.SnapshotData.Parent.W = parentSize.W;
    }

    private static void SetSnapshotObjectSize(
      this IApp theApp,
      string sheetId,
      string objectId,
      SnapshotProperties snapshotProps)
    {
      SnapshotObjectSizeDef objectSize = theApp.GetObjectSize(sheetId, objectId);
      if (snapshotProps.SnapshotData == null)
        snapshotProps.SnapshotData = new SnapshotDataDef();
      snapshotProps.SnapshotData.Object.Size.H = objectSize.H;
      snapshotProps.SnapshotData.Object.Size.W = objectSize.W;
    }

    private static void SetSnapshotContentSize(
      this IApp theApp,
      string sheetId,
      string objectId,
      SnapshotProperties snapshotProps)
    {
      if (snapshotProps.SnapshotData == null)
        snapshotProps.SnapshotData = new SnapshotDataDef();
      SnapshotObjectSizeDef objectContentSize = theApp.GetObjectContentSize(sheetId, objectId);
      snapshotProps.SnapshotData.Content.Size.H = objectContentSize.H;
      snapshotProps.SnapshotData.Content.Size.W = objectContentSize.W;
    }

    private static string GetvisualizationType(GenericObject obj)
    {
      string str = obj.GetType().Name.ToLowerInvariant();
      if (str.Equals("textimage"))
        str = "text-image";
      return str;
    }

    private static SnapshotObjectSizeDef GetObjectSize(
      this IApp theApp,
      string sheetId,
      string objectId)
    {
      SnapshotObjectSizeDef gridCellSize = AppExtensions.GetGridCellSize();
      SheetCell sheetCell = theApp.GetSheet(sheetId).Properties.Cells.First<SheetCell>((Func<SheetCell, bool>) (c => c.Name.Equals(objectId)));
      float num1 = (float) sheetCell.Rowspan * gridCellSize.H;
      float num2 = (float) sheetCell.Colspan * gridCellSize.W;
      return new SnapshotObjectSizeDef()
      {
        H = num1,
        W = num2
      };
    }

    private static SnapshotObjectSizeDef GetParentSize(
      this IApp theApp,
      string sheetId)
    {
      SnapshotObjectSizeDef gridCellSize = AppExtensions.GetGridCellSize();
      SheetProperties properties = theApp.GetSheet(sheetId).Properties;
      float num1 = (float) ((double) properties.Rows * (double) gridCellSize.H + 36.0 + 10.0 + 10.0);
      float num2 = (float) ((double) properties.Columns * (double) gridCellSize.W + 10.0 + 10.0);
      return new SnapshotObjectSizeDef()
      {
        H = num1,
        W = num2
      };
    }

    private static SnapshotObjectSizeDef GetGridCellSize()
    {
      SnapshotObjectSizeDef screenSize = AppExtensions.GetScreenSize();
      float num1 = 0.03955078f * screenSize.W;
      float num2 = 0.05576923f * screenSize.H;
      return new SnapshotObjectSizeDef()
      {
        H = num2,
        W = num1
      };
    }

    private static SnapshotObjectSizeDef GetObjectContentSize(
      this IApp theApp,
      string sheetId,
      string objectId)
    {
      Tuple<float, float, float> objectTitlesSize = theApp.GetObjectTitlesSize(objectId);
      float num1 = objectTitlesSize.Item1;
      float num2 = objectTitlesSize.Item2;
      float num3 = objectTitlesSize.Item3;
      SnapshotObjectSizeDef objectSize = theApp.GetObjectSize(sheetId, objectId);
      float num4 = objectSize.H - num1 - num2 - num3;
      float w = objectSize.W;
      return new SnapshotObjectSizeDef() { H = num4, W = w };
    }

    private static Tuple<float, float, float> GetObjectTitlesSize(
      this IApp theApp,
      string objectId)
    {
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      VisualizationBaseLayout visualizationBaseLayout = AppExtensions.GetActiveProperties(theApp.GetObject<GenericObject>(objectId)).As<VisualizationBaseLayout>();
      if (!visualizationBaseLayout.Title.Equals(""))
        flag1 = true;
      if (!visualizationBaseLayout.Subtitle.Equals(""))
        flag2 = true;
      if (!visualizationBaseLayout.Footnote.Equals(""))
        flag3 = true;
      return Tuple.Create<float, float, float>(flag1 ? 27f : 0.0f, flag2 ? 22f : 0.0f, flag3 ? 22f : 0.0f);
    }

    private static AbstractStructure GetActiveProperties(
      GenericObject genericObject)
    {
      GenericObjectProperties properties = genericObject.Properties;
      return (AbstractStructure) (properties.ExtendsObject?.Properties ?? properties);
    }

    private static long ToJavascriptTimestamp(this DateTime input)
    {
      DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
      return input.Subtract(new TimeSpan(dateTime.Ticks)).Ticks / 10000L;
    }

    public static ISheet GetSheet(this IApp theApp, string id) => QlikConnection.AwaitResponse<ISheet>(theApp.GetSheetAsync(id), nameof (GetSheet), theApp.Session.CancellationToken);

    public static Task<ISheet> GetSheetAsync(this IApp theApp, string id) => theApp.GetObjectAsync<Sheet, ISheet>(id);

    public static ISheet OnGetSheetAsync(Qlik.Engine.Communication.IO.Response response) => (ISheet) response.Result<Sheet>();

    public static ISheet CreateSheet(this IApp theApp, string id, SheetProperties properties) => QlikConnection.AwaitResponse<ISheet>(theApp.CreateSheetAsync(id, properties), nameof (CreateSheet), theApp.Session.CancellationToken);

    public static Task<ISheet> CreateSheetAsync(
      this IApp theApp,
      string id,
      SheetProperties properties)
    {
      SheetProperties sheetProperties = properties ?? new SheetProperties();
      if (sheetProperties.Info == null)
        sheetProperties.Info = new NxInfo();
      if (sheetProperties.Info.Id == null)
      {
        if (id != null)
          sheetProperties.Info.Id = id.Replace(" ", "");
      }
      else
        sheetProperties.Info.Id = sheetProperties.Info.Id.Replace(" ", "");
      sheetProperties.Info.Type = "sheet";
      if (sheetProperties.MetaDef == null)
        sheetProperties.MetaDef = new MetaAttributesDef();
      if (string.IsNullOrEmpty(sheetProperties.MetaDef.Title))
        sheetProperties.MetaDef.Title = "My new sheet";
      if (sheetProperties.Cells == null)
        sheetProperties.Cells = (IEnumerable<SheetCell>) new SheetCell[0];
      return theApp.CreateGenericObjectAsync<ISheet>((AsyncHandle) null, new Func<Qlik.Engine.Communication.IO.Response, ISheet>(AppExtensions.OnCreateSheetAsync), (GenericObjectProperties) sheetProperties);
    }

    public static ISheet OnCreateSheetAsync(Qlik.Engine.Communication.IO.Response response) => (ISheet) response.Result<Sheet>();

    public static bool RemoveSheet(this IApp theApp, string id) => QlikConnection.AwaitResponse<bool>(theApp.RemoveSheetAsync(id), nameof (RemoveSheet), theApp.Session.CancellationToken);

    public static Task<bool> RemoveSheetAsync(this IApp theApp, string id = null) => theApp.DestroyGenericObjectAsync(id);

    public static IEnumerable<ISheet> GetSheets(this IApp theApp)
    {
      using (ISheetList sheetList = theApp.GetSheetList())
        return (IEnumerable<ISheet>) sheetList.Layout.AppObjectList.Items.Select<SheetObjectViewListContainer, ISheet>((Func<SheetObjectViewListContainer, ISheet>) (container => theApp.GetSheet(container.Info.Id))).ToArray<ISheet>();
    }

    //public static async Task<IEnumerable<Task<ISheet>>> GetSheetsAsync(
    //  this IApp theApp)
    //{
    //  ISheetList sheetList = await theApp.GetSheetListAsync().ConfigureAwait(false);
    //  GenericObjectLayout layout = await sheetList.GetLayoutAsync().ConfigureAwait(false);
    //  string[] sheetIds = layout.As<SheetListLayout>().AppObjectList.Items.Select<SheetObjectViewListContainer, string>((Func<SheetObjectViewListContainer, string>) (container => container.Info.Id)).ToArray<string>();
    //  Task<bool> destroyTask = theApp.DestroyGenericSessionObjectAsync(sheetList.Id);
    //  Task<ISheet>[] sheetTasks = ((IEnumerable<string>) sheetIds).Select<string, Task<ISheet>>(new Func<string, Task<ISheet>>(((AppExtensions) theApp).GetSheetAsync)).ToArray<Task<ISheet>>();
    //  int num = await destroyTask.ConfigureAwait(false) ? 1 : 0;
    //  IEnumerable<Task<ISheet>> tasks = (IEnumerable<Task<ISheet>>) sheetTasks;
    //  sheetList = (ISheetList) null;
    //  layout = (GenericObjectLayout) null;
    //  sheetIds = (string[]) null;
    //  destroyTask = (Task<bool>) null;
    //  sheetTasks = (Task<ISheet>[]) null;
    //  return tasks;
    //}

    [Obsolete("Storytelling related methods will be removed from this SDK in future releases. Use GenericObject and AbstractStructure to interact with this client functionality.")]
    public static IStory GetStory(this IApp theApp, string id) => QlikConnection.AwaitResponse<IStory>(theApp.GetStoryAsync(id), "GetStoryAsync", theApp.Session.CancellationToken);

    [Obsolete("Storytelling related methods will be removed from this SDK in future releases. Use GenericObject and AbstractStructure to interact with this client functionality.")]
    public static Task<IStory> GetStoryAsync(this IApp theApp, string id) => theApp.GetStoryAsync((AsyncHandle) null, id);

    [Obsolete("Storytelling related methods will be removed from this SDK in future releases. Use GenericObject and AbstractStructure to interact with this client functionality.")]
    public static Task<IStory> GetStoryAsync(
      this IApp theApp,
      AsyncHandle asyncHandle,
      string id)
    {
      return theApp.GetGenericObjectAsync<IStory>((AsyncHandle) null, new Func<Qlik.Engine.Communication.IO.Response, IStory>(AppExtensions.OnGetStoryAsync), id);
    }

    [Obsolete("Storytelling related methods will be removed from this SDK in future releases. Use GenericObject and AbstractStructure to interact with this client functionality.")]
    public static IStory OnGetStoryAsync(Qlik.Engine.Communication.IO.Response response) => (IStory) response.Result<Story>();

    [Obsolete("Storytelling related methods will be removed from this SDK in future releases. Use GenericObject and AbstractStructure to interact with this client functionality.")]
    public static IStory CreateStory(this IApp theApp, string id, StoryProperties properties) => QlikConnection.AwaitResponse<IStory>(theApp.CreateStoryAsync(id, properties), nameof (CreateStory), theApp.Session.CancellationToken);

    [Obsolete("Storytelling related methods will be removed from this SDK in future releases. Use GenericObject and AbstractStructure to interact with this client functionality.")]
    public static Task<IStory> CreateStoryAsync(
      this IApp theApp,
      string id,
      StoryProperties properties)
    {
      StoryProperties storyProperties = properties ?? new StoryProperties();
      if (storyProperties.ChildListDef == null)
        storyProperties.ChildListDef = new StoryChildListDef();
      if (storyProperties.Info == null)
        storyProperties.Info = new NxInfo();
      if (storyProperties.Info.Id == null)
      {
        if (id != null)
          storyProperties.Info.Id = id.Replace(" ", "");
      }
      else
        storyProperties.Info.Id = storyProperties.Info.Id.Replace(" ", "");
      storyProperties.Info.Type = "story";
      return theApp.CreateGenericObjectAsync<IStory>((AsyncHandle) null, new Func<Qlik.Engine.Communication.IO.Response, IStory>(AppExtensions.OnCreateStoryAsync), (GenericObjectProperties) storyProperties);
    }

    [Obsolete("Storytelling related methods will be removed from this SDK in future releases. Use GenericObject and AbstractStructure to interact with this client functionality.")]
    public static IStory OnCreateStoryAsync(Qlik.Engine.Communication.IO.Response response) => (IStory) response.Result<Story>();

    [Obsolete("Storytelling related methods will be removed from this SDK in future releases. Use GenericObject and AbstractStructure to interact with this client functionality.")]
    public static bool DestroyStory(this IApp theApp, string id) => QlikConnection.AwaitResponse<bool>(theApp.DestroyStoryAsync(id), nameof (DestroyStory), theApp.Session.CancellationToken);

    [Obsolete("Storytelling related methods will be removed from this SDK in future releases. Use GenericObject and AbstractStructure to interact with this client functionality.")]
    public static Task<bool> DestroyStoryAsync(this IApp theApp, string id = null) => theApp.DestroyGenericObjectAsync(id);

    [Obsolete("Storytelling related methods will be removed from this SDK in future releases. Use GenericObject and AbstractStructure to interact with this client functionality.")]
    public static IEnumerable<IStory> GetStories(this IApp theApp) => theApp.GetStoryList().Items.Select<StoryObjectViewListContainer, IStory>((Func<StoryObjectViewListContainer, IStory>) (container => theApp.GetStory(container.Info.Id)));

    [Obsolete("Storytelling related methods will be removed from this SDK in future releases. Use GenericObject and AbstractStructure to interact with this client functionality.")]
    public static Task<IEnumerable<IStory>> GetStoriesAsync(this IApp theApp) => theApp.GetStoryListAsync().ContinueWith<IEnumerable<IStory>>((Func<Task<IStoryList>, IEnumerable<IStory>>) (task => task.Result.Items.AsParallel<StoryObjectViewListContainer>().Select<StoryObjectViewListContainer, IStory>((Func<StoryObjectViewListContainer, IStory>) (container => theApp.GetStory(container.Info.Id))).AsSequential<IStory>()));

    public static IAppField GetAppField(
      this IApp theApp,
      string name,
      bool showAlternatives = false,
      int pageSize = 20)
    {
      return QlikConnection.AwaitResponse<IAppField>(theApp.GetAppFieldAsync(name, showAlternatives, pageSize), nameof (GetAppField), theApp.Session.CancellationToken);
    }

    public static Task<IAppField> GetAppFieldAsync(
      this IApp theApp,
      string name,
      bool showAlternatives = false,
      int pageSize = 20)
    {
      AppFieldProperties appFieldProperties1 = new AppFieldProperties();
      appFieldProperties1.Info = new NxInfo()
      {
        Id = name,
        Type = "appfield"
      };
      AppFieldProperties appFieldProperties2 = appFieldProperties1;
      ListObjectDef listObjectDef1 = new ListObjectDef();
      listObjectDef1.ShowAlternatives = showAlternatives;
      listObjectDef1.Def = new NxInlineDimensionDef()
      {
        FieldDefs = (IEnumerable<string>) new List<string>()
        {
          name
        },
        FieldLabels = (IEnumerable<string>) new List<string>()
        {
          name
        }
      };
      listObjectDef1.InitialDataFetch = (IEnumerable<NxPage>) new NxPage[1]
      {
        new NxPage() { Height = pageSize, Width = 1 }
      };
      ListObjectDef listObjectDef2 = listObjectDef1;
      appFieldProperties2.ListObjectDef = listObjectDef2;
      AppFieldProperties appFieldProperties3 = appFieldProperties1;
      return theApp.CreateGenericSessionObjectAsync<IAppField>((AsyncHandle) null, new Func<Qlik.Engine.Communication.IO.Response, IAppField>(AppExtensions.OnGetAppFieldAsync), (GenericObjectProperties) appFieldProperties3);
    }

    private static IAppField OnGetAppFieldAsync(Qlik.Engine.Communication.IO.Response response) => (IAppField) response.Result<AppField>();

    public static IEnumerable<IAppField> GetAppFields(this IApp theApp)
    {
      using (IFieldList fieldList = theApp.GetFieldList())
        return (IEnumerable<IAppField>) fieldList.Layout.FieldList.Items.Select<NxFieldDescription, IAppField>((Func<NxFieldDescription, IAppField>) (container => theApp.GetAppField(container.Name))).ToArray<IAppField>();
    }

    public static async Task<IEnumerable<IAppField>> GetAppFieldsAsync(
      this IApp theApp)
    {
      IFieldList fieldList = await theApp.GetFieldListAsync().ConfigureAwait(false);
      GenericObjectLayout layout = await fieldList.GetLayoutAsync().ConfigureAwait(false);
      string[] fieldNames = layout.As<FieldListLayout>().FieldList.Items.Select<NxFieldDescription, string>((Func<NxFieldDescription, string>) (container => container.Name)).ToArray<string>();
      Task<bool> destroyTask = theApp.DestroyGenericSessionObjectAsync(fieldList.Id);
      Task<IAppField>[] appFieldTasks = ((IEnumerable<string>) fieldNames).Select<string, Task<IAppField>>((Func<string, Task<IAppField>>) (name => theApp.GetAppFieldAsync(name))).ToArray<Task<IAppField>>();
      int num = await destroyTask.ConfigureAwait(false) ? 1 : 0;
      IAppField[] appFieldArray = await Task.WhenAll<IAppField>(appFieldTasks).ConfigureAwait(false);
      IEnumerable<IAppField> appFields = (IEnumerable<IAppField>) appFieldArray;
      fieldList = (IFieldList) null;
      layout = (GenericObjectLayout) null;
      fieldNames = (string[]) null;
      destroyTask = (Task<bool>) null;
      appFieldTasks = (Task<IAppField>[]) null;
      return appFields;
    }

    public static IBookmarkList GetBookmarkList(this IApp theApp) => QlikConnection.AwaitResponse<IBookmarkList>(theApp.GetBookmarkListAsync(), nameof (GetBookmarkList), theApp.Session.CancellationToken);

    public static Task<IBookmarkList> GetBookmarkListAsync(this IApp theApp)
    {
      BookmarkListProperties bookmarkListProperties = new BookmarkListProperties();
      bookmarkListProperties.Info = new NxInfo();
      bookmarkListProperties.Info.Id = "BookmarkList";
      bookmarkListProperties.Info.Type = "BookmarkList";
      bookmarkListProperties.BookmarkListDef = new BookmarkObjectViewListDef();
      bookmarkListProperties.BookmarkListDef.Data = new BookmarkObjectViewDef();
      return theApp.CreateGenericSessionObjectAsync<IBookmarkList>((AsyncHandle) null, new Func<Qlik.Engine.Communication.IO.Response, IBookmarkList>(AppExtensions.OnGetBookmarkListAsync), (GenericObjectProperties) bookmarkListProperties);
    }

    public static IBookmarkList OnGetBookmarkListAsync(Qlik.Engine.Communication.IO.Response response) => (IBookmarkList) response.Result<BookmarkList>();

    public static ICurrentSelection GetCurrentSelection(this IApp theApp) => QlikConnection.AwaitResponse<ICurrentSelection>(theApp.GetCurrentSelectionAsync(), nameof (GetCurrentSelection), theApp.Session.CancellationToken);

    public static Task<ICurrentSelection> GetCurrentSelectionAsync(
      this IApp theApp)
    {
      CurrentSelectionProperties selectionProperties = new CurrentSelectionProperties();
      selectionProperties.Info = new NxInfo();
      selectionProperties.Info.Id = "CurrentSelection";
      selectionProperties.Info.Type = "CurrentSelection";
      selectionProperties.SelectionObjectDef = new SelectionObjectDef();
      return theApp.CreateGenericSessionObjectAsync<ICurrentSelection>((AsyncHandle) null, new Func<Qlik.Engine.Communication.IO.Response, ICurrentSelection>(AppExtensions.OnGetCurrentSelectionAsync), (GenericObjectProperties) selectionProperties);
    }

    public static ICurrentSelection OnGetCurrentSelectionAsync(Qlik.Engine.Communication.IO.Response response) => (ICurrentSelection) response.Result<CurrentSelection>();

    public static IFieldList GetFieldList(this IApp theApp) => QlikConnection.AwaitResponse<IFieldList>(theApp.GetFieldListAsync(), nameof (GetFieldList), theApp.Session.CancellationToken);

    public static Task<IFieldList> GetFieldListAsync(this IApp theApp)
    {
      FieldListProperties fieldListProperties = new FieldListProperties();
      fieldListProperties.Info = new NxInfo();
      fieldListProperties.Info.Id = "FieldList";
      fieldListProperties.Info.Type = "FieldList";
      fieldListProperties.FieldListDef = new FieldListDef();
      return theApp.CreateGenericSessionObjectAsync<IFieldList>((AsyncHandle) null, new Func<Qlik.Engine.Communication.IO.Response, IFieldList>(AppExtensions.OnGetFieldListAsync), (GenericObjectProperties) fieldListProperties);
    }

    public static IFieldList OnGetFieldListAsync(Qlik.Engine.Communication.IO.Response response) => (IFieldList) response.Result<FieldList>();

    public static ISheetList GetSheetList(this IApp theApp) => QlikConnection.AwaitResponse<ISheetList>(theApp.GetSheetListAsync(), nameof (GetSheetList), theApp.Session.CancellationToken);

    public static Task<ISheetList> GetSheetListAsync(this IApp theApp)
    {
      SheetListProperties sheetListProperties = new SheetListProperties();
      sheetListProperties.Info = new NxInfo();
      sheetListProperties.Info.Id = "SheetList";
      sheetListProperties.Info.Type = "SheetList";
      sheetListProperties.AppObjectListDef = new SheetObjectViewListDef();
      sheetListProperties.AppObjectListDef.Data = new SheetObjectViewDef();
      return theApp.CreateGenericSessionObjectAsync<ISheetList>((AsyncHandle) null, new Func<Qlik.Engine.Communication.IO.Response, ISheetList>(AppExtensions.OnGetSheetListAsync), (GenericObjectProperties) sheetListProperties);
    }

    public static ISheetList OnGetSheetListAsync(Qlik.Engine.Communication.IO.Response response) => (ISheetList) response.Result<SheetList>();

    public static IDimensionList GetDimensionList(this IApp theApp) => QlikConnection.AwaitResponse<IDimensionList>(theApp.GetDimensionListAsync(), nameof (GetDimensionList), theApp.Session.CancellationToken);

    public static Task<IDimensionList> GetDimensionListAsync(this IApp theApp)
    {
      DimensionListProperties dimensionListProperties = new DimensionListProperties();
      dimensionListProperties.Info = new NxInfo();
      dimensionListProperties.Info.Id = "DimensionList";
      dimensionListProperties.Info.Type = "DimensionList";
      dimensionListProperties.DimensionListDef = new DimensionObjectViewListDef();
      dimensionListProperties.DimensionListDef.Data = new DimensionObjectViewDef();
      return theApp.CreateGenericSessionObjectAsync<IDimensionList>((AsyncHandle) null, new Func<Qlik.Engine.Communication.IO.Response, IDimensionList>(AppExtensions.OnGetDimensionListAsync), (GenericObjectProperties) dimensionListProperties);
    }

    public static IDimensionList OnGetDimensionListAsync(Qlik.Engine.Communication.IO.Response response) => (IDimensionList) response.Result<DimensionList>();

    public static IMeasureList GetMeasureList(this IApp theApp) => QlikConnection.AwaitResponse<IMeasureList>(theApp.GetMeasureListAsync(), nameof (GetMeasureList), theApp.Session.CancellationToken);

    public static Task<IMeasureList> GetMeasureListAsync(this IApp theApp)
    {
      MeasureListProperties measureListProperties = new MeasureListProperties();
      measureListProperties.Info = new NxInfo();
      measureListProperties.Info.Id = "MeasureList";
      measureListProperties.Info.Type = "MeasureList";
      measureListProperties.MeasureListDef = new MeasureObjectViewListDef();
      measureListProperties.MeasureListDef.Data = new MeasureObjectViewDef();
      return theApp.CreateGenericSessionObjectAsync<IMeasureList>((AsyncHandle) null, new Func<Qlik.Engine.Communication.IO.Response, IMeasureList>(AppExtensions.OnGetMeasureListAsync), (GenericObjectProperties) measureListProperties);
    }

    public static IMeasureList OnGetMeasureListAsync(Qlik.Engine.Communication.IO.Response response) => (IMeasureList) response.Result<MeasureList>();

    public static IUndoInfo GetUndoInfo(this IApp theApp) => QlikConnection.AwaitResponse<IUndoInfo>(theApp.GetUndoInfoAsync(), nameof (GetUndoInfo), theApp.Session.CancellationToken);

    public static Task<IUndoInfo> GetUndoInfoAsync(this IApp theApp)
    {
      UndoInfoProperties undoInfoProperties = new UndoInfoProperties();
      undoInfoProperties.Info = new NxInfo();
      undoInfoProperties.Info.Id = "UndoInfo";
      undoInfoProperties.Info.Type = "UndoInfo";
      undoInfoProperties.UndoInfoDef = new UndoInfoDef();
      return theApp.CreateGenericSessionObjectAsync<IUndoInfo>((AsyncHandle) null, new Func<Qlik.Engine.Communication.IO.Response, IUndoInfo>(AppExtensions.OnGetUndoInfoAsync), (GenericObjectProperties) undoInfoProperties);
    }

    public static IUndoInfo OnGetUndoInfoAsync(Qlik.Engine.Communication.IO.Response response) => (IUndoInfo) response.Result<UndoInfo>();

    public static IMasterObjectList GetMasterObjectList(this IApp theApp) => QlikConnection.AwaitResponse<IMasterObjectList>(theApp.GetMasterObjectListAsync(), nameof (GetMasterObjectList), theApp.Session.CancellationToken);

    public static Task<IMasterObjectList> GetMasterObjectListAsync(
      this IApp theApp)
    {
      MasterObjectListProperties objectListProperties = new MasterObjectListProperties();
      objectListProperties.Info = new NxInfo();
      objectListProperties.Info.Id = "MasterObjectList";
      objectListProperties.Info.Type = "MasterObjectList";
      objectListProperties.AppObjectListDef = new MasterObjectObjectViewListDef();
      objectListProperties.AppObjectListDef.Data = new MasterObjectObjectViewDef();
      return theApp.CreateGenericSessionObjectAsync<IMasterObjectList>((AsyncHandle) null, new Func<Qlik.Engine.Communication.IO.Response, IMasterObjectList>(AppExtensions.OnGetMasterObjectListAsync), (GenericObjectProperties) objectListProperties);
    }

    public static IMasterObjectList OnGetMasterObjectListAsync(Qlik.Engine.Communication.IO.Response response) => (IMasterObjectList) response.Result<MasterObjectList>();

    [Obsolete("Storytelling related classes will be removed from this SDK in future releases. Use GenericObject and AbstractStructure to interact with this client functionality.")]
    public static IStoryList GetStoryList(this IApp theApp) => QlikConnection.AwaitResponse<IStoryList>(theApp.GetStoryListAsync(), nameof (GetStoryList), theApp.Session.CancellationToken);

    [Obsolete("Storytelling related classes will be removed from this SDK in future releases. Use GenericObject and AbstractStructure to interact with this client functionality.")]
    public static Task<IStoryList> GetStoryListAsync(this IApp theApp)
    {
      StoryListProperties storyListProperties = new StoryListProperties();
      storyListProperties.Info = new NxInfo();
      storyListProperties.Info.Id = "StoryList";
      storyListProperties.Info.Type = "StoryList";
      storyListProperties.AppObjectListDef = new StoryObjectViewListDef();
      storyListProperties.AppObjectListDef.Data = new StoryObjectViewDef();
      return theApp.CreateGenericSessionObjectAsync<IStoryList>((AsyncHandle) null, new Func<Qlik.Engine.Communication.IO.Response, IStoryList>(AppExtensions.OnGetStoryListAsync), (GenericObjectProperties) storyListProperties);
    }

    [Obsolete("Storytelling related classes will be removed from this SDK in future releases. Use GenericObject and AbstractStructure to interact with this client functionality.")]
    public static IStoryList OnGetStoryListAsync(Qlik.Engine.Communication.IO.Response response) => (IStoryList) response.Result<StoryList>();

    [Obsolete("Storytelling related classes will be removed from this SDK in future releases. Use GenericObject and AbstractStructure to interact with this client functionality.")]
    public static ISnapshotList GetSnapshotList(this IApp theApp) => QlikConnection.AwaitResponse<ISnapshotList>(theApp.GetSnapshotListAsync(), nameof (GetSnapshotList), theApp.Session.CancellationToken);

    [Obsolete("Storytelling related classes will be removed from this SDK in future releases. Use GenericObject and AbstractStructure to interact with this client functionality.")]
    public static Task<ISnapshotList> GetSnapshotListAsync(this IApp theApp)
    {
      SnapshotListProperties snapshotListProperties = new SnapshotListProperties();
      snapshotListProperties.Info = new NxInfo();
      snapshotListProperties.Info.Id = "SnapshotList";
      snapshotListProperties.Info.Type = "SnapshotList";
      snapshotListProperties.BookmarkListDef = new SnapshotObjectViewListDef();
      snapshotListProperties.BookmarkListDef.Data = new SnapshotObjectViewDef();
      return theApp.CreateGenericSessionObjectAsync<ISnapshotList>((AsyncHandle) null, new Func<Qlik.Engine.Communication.IO.Response, ISnapshotList>(AppExtensions.OnGetSnapshotListAsync), (GenericObjectProperties) snapshotListProperties);
    }

    [Obsolete("Storytelling related classes will be removed from this SDK in future releases. Use GenericObject and AbstractStructure to interact with this client functionality.")]
    public static ISnapshotList OnGetSnapshotListAsync(Qlik.Engine.Communication.IO.Response response) => (ISnapshotList) response.Result<SnapshotList>();

    public static IVariableList GetVariableList(this IApp theApp) => QlikConnection.AwaitResponse<IVariableList>(theApp.GetVariableListAsync(), nameof (GetVariableList), theApp.Session.CancellationToken);

    public static Task<IVariableList> GetVariableListAsync(this IApp theApp)
    {
      VariableListProperties variableListProperties = new VariableListProperties();
      variableListProperties.Info = new NxInfo();
      variableListProperties.Info.Id = "VariableList";
      variableListProperties.Info.Type = "VariableList";
      variableListProperties.VariableListDef = new VariableObjectViewListDef();
      variableListProperties.VariableListDef.Data = new VariableObjectViewDef();
      return theApp.CreateGenericSessionObjectAsync<IVariableList>((AsyncHandle) null, new Func<Qlik.Engine.Communication.IO.Response, IVariableList>(AppExtensions.OnGetVariableListAsync), (GenericObjectProperties) variableListProperties);
    }

    public static IVariableList OnGetVariableListAsync(Qlik.Engine.Communication.IO.Response response) => (IVariableList) response.Result<VariableList>();
  }
}
