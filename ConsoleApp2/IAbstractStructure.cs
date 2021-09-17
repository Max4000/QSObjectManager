using System.Collections.Generic;
using Newtonsoft.Json;
using QlikConnection = Qlik.Engine.Communication.QlikConnection;

namespace MyBookmark
{
    public interface IAbstractStructure
    {
        QlikConnection Session { get; }

        string PrintStructure(Formatting formatting = Formatting.None);

        T As<T>() where T : AbstractStructure, new();

        T CloneAs<T>() where T : AbstractStructure, new();

        T CloneSubstructureAs<T>(string path);

        T Get<T>(string propertyName);

        T Get<T>(string propertyName, T defaultValue);

        void Set<T>(string propertyName, T value);

        bool IsSet(string propertyName);

        IEnumerable<string> GetAllProperties(bool recursive = false);

        IEnumerable<string> GetAllPropertyPaths(bool recursive = false);
    }
}