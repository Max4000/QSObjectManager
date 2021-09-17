using System.Collections.Generic;

namespace MyBookmark
{
    public interface IStructureDiff
    {
        StructureDiff.DiffType Type { get; }

        List<string> Path { get; }

        string OldValue { get; }

        string NewValue { get; }

        string OldType { get; }

        string NewType { get; }
    }
}