using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace MyBookmark
{
    public abstract class StructureDiff : IStructureDiff
    {
        private JToken _oldValue;
        private JToken _newValue;

        protected StructureDiff() => this.Path = new List<string>();

        public abstract StructureDiff.DiffType Type { get; }

        public List<string> Path { get; private set; }

        public string OldValue => this._oldValue == null ? (string)null : ((object)this._oldValue).ToString();

        public string NewValue => this._newValue == null ? (string)null : ((object)this._newValue).ToString();

        public string OldType => this._oldValue == null ? (string)null : this._oldValue.Type.ToString();

        public string NewType => this._newValue == null ? (string)null : this._newValue.Type.ToString();

        public void AddPropertyLevel(string property) => this.Path.Insert(0, property);

        public static IEnumerable<StructureDiff> Diff(JToken left, JToken right)
        {
            if (right == null)
                yield return (StructureDiff)new StructureDiff.RemoveDiff(left);
            else if (left == null)
                yield return (StructureDiff)new StructureDiff.AddDiff(right);
            else if (left.Type != right.Type)
                yield return (StructureDiff)new StructureDiff.TypeChangeDiff(left, right);
            else if (left is JArray && right is JArray)
            {
                JArray leftA = left as JArray;
                JArray rightA = right as JArray;
                int leftSize = leftA.Count;
                int rightSize = rightA.Count;
                for (int i = 0; i < Math.Max(leftSize, rightSize); ++i)
                {
                    JToken leftItem = i >= leftSize ? (JToken)null : leftA[i];
                    JToken rightItem = i >= rightSize ? (JToken)null : rightA[i];
                    foreach (StructureDiff diff in StructureDiff.Diff(leftItem, rightItem))
                    {
                        diff.AddPropertyLevel(i.ToString((IFormatProvider)CultureInfo.InvariantCulture));
                        yield return diff;
                    }
                    leftItem = (JToken)null;
                    rightItem = (JToken)null;
                }
            }
            else
            {
                IEnumerable<string> leftNames = ((IEnumerable<JProperty>)left.Children<JProperty>()).Select<JProperty, string>((Func<JProperty, string>)(p => p.Name));
                IEnumerable<string> rightNames = ((IEnumerable<JProperty>)right.Children<JProperty>()).Select<JProperty, string>((Func<JProperty, string>)(p => p.Name));
                string[] allNames = leftNames.Union<string>(rightNames).ToArray<string>();
                if (!((IEnumerable<string>)allNames).Any<string>())
                {
                    if (!JToken.DeepEquals(left, right))
                        yield return (StructureDiff)new StructureDiff.ChangeDiff(left, right);
                }
                else
                {
                    string[] strArray = allNames;
                    for (int index = 0; index < strArray.Length; ++index)
                    {
                        string name = strArray[index];
                        foreach (StructureDiff diff in StructureDiff.Diff(left[(object)name], right[(object)name]))
                        {
                            diff.AddPropertyLevel(name);
                            yield return diff;
                        }
                        name = (string)null;
                    }
                    strArray = (string[])null;
                }
            }
        }

        public enum DiffType
        {
            Add,
            Remove,
            Change,
            TypeChange,
        }

        private class AddDiff : StructureDiff
        {
            public AddDiff(JToken to) => this._newValue = to;

            public override StructureDiff.DiffType Type => StructureDiff.DiffType.Add;

            public override string ToString() => "Added: " + string.Join(".", (IEnumerable<string>)this.Path);
        }

        private class RemoveDiff : StructureDiff
        {
            public RemoveDiff(JToken from) => this._oldValue = from;

            public override StructureDiff.DiffType Type => StructureDiff.DiffType.Remove;

            public override string ToString() => "Removed: " + string.Join(".", (IEnumerable<string>)this.Path);
        }

        private class ChangeDiff : StructureDiff
        {
            public override StructureDiff.DiffType Type => StructureDiff.DiffType.Change;

            public ChangeDiff(JToken from, JToken to)
            {
                this._oldValue = from;
                this._newValue = to;
            }

            public override string ToString() => "Changed: " + string.Join(".", (IEnumerable<string>)this.Path) + " : " + this.OldValue + " => " + this.NewValue;
        }

        private class TypeChangeDiff : StructureDiff
        {
            public override StructureDiff.DiffType Type => StructureDiff.DiffType.TypeChange;

            public TypeChangeDiff(JToken from, JToken to)
            {
                this._oldValue = from;
                this._newValue = to;
            }

            public override string ToString() => "Type changed: " + string.Join(".", (IEnumerable<string>)this.Path) + " : " + this.OldType + " => " + this.NewType;
        }
    }
}