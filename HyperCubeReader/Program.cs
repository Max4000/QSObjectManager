using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using ObjectsForWorkWithQSEngine.MainObjectsForWork;
using Qlik.Engine;
using Qlik.Sense.Client.Snapshot;

namespace HyperCubeReader
{
    class HyperCubeReader 
    {
        static void Main()
        {
            //HyperCube unused =  ReadCube(
            //      @"D:\QS_STORE\Covid_Monitor_202006032256_AR23_20210913133241577\stories\c203046d-6026-4b75-96d3-ebbeef537a58\ZFepSG\Item1\ISnapshot.Properties.qHyperCube.json");
            IConnect loc = new LocalConnection("http://localhost:4848/");
        }

        static Size CreateSize(JToken mToken)
        {
            Size reult = new Size();
            foreach (JToken token in mToken.Children().Children())
            {
                switch (((JProperty) token).Name)
                {
                    case "qcx":
                    {
                        reult.cx = Convert.ToInt32(token.First.ToString());
                        break;
                    }
                    case "qcy":
                    {
                        reult.cy = Convert.ToInt32(token.First.ToString());
                        break;
                    }
                }
            }

            return reult;
        }

        static IEnumerable<NxAttrExprInfo> GetAttrExprInfos(JToken mToken)
        {
            IList<NxAttrExprInfo> resList = new List<NxAttrExprInfo>();
            foreach (JToken token in mToken.Children().Children())
            {
                NxAttrExprInfo eInfo = new NxAttrExprInfo();
                foreach (JToken tok in token.Children())
                {
                    switch (((JProperty) tok).Name)
                    {
                        case "qMin":
                        {
                            eInfo.Min = Convert.ToDouble(tok.First.ToString());
                            break;
                        }
                        case "qMax":
                        {
                            eInfo.Max = Convert.ToDouble(tok.First.ToString());
                            break;
                        }
                        case "qFallbackTitle":
                        {
                            eInfo.FallbackTitle = tok.First.ToString();
                            break;
                        }
                        case "qMinText":
                        {
                            eInfo.MinText = tok.First.ToString();
                            break;
                        }
                        case "qMaxText":
                        {
                            eInfo.MaxText = tok.First.ToString();
                            break;
                        }
                        case "qNumFormat":
                        {
                            eInfo.NumFormat = GetNumFormat(tok);
                            break;
                        }
                        case "qIsAutoFormat":
                        {
                            eInfo.IsAutoFormat = Convert.ToBoolean(tok.First.ToString());
                            break;
                        }
                    }
                }

                resList.Add(eInfo);
            }

            return resList;
        }

        static IEnumerable<NxMeasureInfo> CreateMeasureInfos(JToken mToken)
        {
            IList<NxMeasureInfo> miList = new List<NxMeasureInfo>();
            foreach (JToken tok2 in mToken.Children())
            {
                foreach (JToken tok3 in tok2.Children())
                {
                    NxMeasureInfo mi = new NxMeasureInfo();
                    foreach (JToken tok4 in tok3.Children())
                    {
                        switch (((JProperty) tok4).Name)
                        {
                            case "qFallbackTitle":
                            {
                                mi.FallbackTitle = tok4.First.ToString();
                                break;
                            }
                            case "qApprMaxGlyphCount":
                            {
                                mi.ApprMaxGlyphCount = Convert.ToInt32(tok4.First.ToString());
                                break;
                            }
                            case "qCardinal":
                            {
                                mi.Cardinal = Convert.ToInt32(tok4.First.ToString());
                                break;
                            }
                            case "qSortIndicator":
                            {
                                mi.SortIndicator = GetSortType(tok4.First.ToString());
                                break;
                            }
                            case "qNumFormat":
                            {
                                mi.NumFormat = GetNumFormat(tok4);
                                break;
                            }
                            case "qMin":
                            {
                                mi.Min = Convert.ToDouble(tok4.First.ToString());
                                break;
                            }
                            case "qMax":
                            {
                                mi.Max = Convert.ToDouble(tok4.First.ToString());
                                break;
                            }
                            case "qAttrExprInfo":
                            {
                                mi.AttrExprInfo = GetAttrExprInfos(tok4);
                                break;
                            }
                        }
                    }

                    miList.Add(mi);
                }
            }

            return miList;
        }

        static IEnumerable<int> CreateSotOrder(JToken mToken)
        {
            SnapshotProperties snapshotProperties = new SnapshotProperties();

            IList<int> res = new List<int>();
            foreach (JToken tk in mToken.Children())
            {
                foreach (var tk2 in tk.Children())
                {
                    res.Add(Convert.ToInt32(tk2.ToString()));
                }
            }

            return res;
        }

        static StateEnumType GetStateType(string state)
        {
            switch (state)
            {
                case "L":
                {
                    return StateEnumType.LOCKED;
                }
                case "S":
                {
                    return StateEnumType.SELECTED;
                }
                case "O":
                {
                    return StateEnumType.OPTION;
                }
                case "D":
                {
                    return StateEnumType.DESELECTED;
                }
                case "A":
                {
                    return StateEnumType.ALTERNATIVE;
                }
                case "X":
                {
                    return StateEnumType.EXCLUDED;
                }
                case "XS":
                {
                    return StateEnumType.EXCL_SELECTED;
                }
                case "XL":
                {
                    return StateEnumType.EXCL_LOCKED;
                }
            }

            return StateEnumType.NSTATES;
        }

        static NxCell GetCell(JToken mToken)
        {
            NxAttributeExpressionValues getAttributeExpressionValues(JToken tok)
            {
                NxAttributeExpressionValues values = new NxAttributeExpressionValues
                {
                    Values = new List<NxSimpleValue>()
                };
                foreach (JToken tk in tok.Children())
                {
                    foreach (JToken tk2 in tk.Children())
                    {
                        foreach (JToken tk3 in tk2.Children())
                        {
                            foreach (JToken tk4 in tk3.Children())
                            {
                                NxSimpleValue oneValue = new NxSimpleValue();
                                foreach (JToken tk5 in tk4.Children())
                                {
                                    switch (((JProperty) tk5).Name)
                                    {
                                        case "qText":
                                        {
                                            oneValue.Text = tk5.First.ToString();
                                            break;
                                        }
                                        case "qNum":
                                        {
                                            string num = tk5.First.ToString();
                                            if (string.CompareOrdinal(num, "NaN") != 0)
                                                oneValue.Num = Convert.ToDouble(num);
                                            break;
                                        }
                                    }
                                }

                                (values.Values as List<NxSimpleValue>)?.Add(oneValue);
                            }
                        }
                    }
                }

                return values;
            }

            NxCell result = new NxCell();
            foreach (JToken tok in mToken.Children())
            {
                switch (((JProperty) tok).Name)
                {
                    case "qText":
                    {
                        result.Text = tok.First.ToString();
                        break;
                    }
                    case "qNum":
                    {
                        string mString = tok.First.ToString();
                        if (string.CompareOrdinal(mString, "NaN") != 0) result.Num = Convert.ToDouble(mString);
                        break;
                    }
                    case "qElemNumber":
                    {
                        result.ElemNumber = Convert.ToInt32(tok.First.ToString());
                        break;
                    }
                    case "qState":
                    {
                        result.State = GetStateType(tok.First.ToString());
                        break;
                    }
                    case "qIsEmpty":
                    {
                        result.IsEmpty = Convert.ToBoolean(tok.First.ToString());
                        break;
                    }
                    case "qIsTotalCell":
                    {
                        result.IsTotalCell = Convert.ToBoolean(tok.First.ToString());
                        break;
                    }
                    case "qIsOtherCell":
                    {
                        result.IsOtherCell = Convert.ToBoolean(tok.First.ToString());
                        break;
                    }
                    case "qFrequency":
                    {
                        result.Frequency = tok.First.ToString();
                        break;
                    }
                    case "qAttrExps":
                    {
                        result.AttrExps = getAttributeExpressionValues(tok);
                        break;
                    }
                }
            }

            return result;
        }

        static IEnumerable<NxCell> CreateGrandTotalRow(JToken mToken)
        {
            IList<NxCell> resList = new List<NxCell>();
            foreach (JToken token in mToken.Children())
            {
                foreach (JToken token2 in token.Children())
                {
                    resList.Add(GetCell(token2));
                }
            }

            return resList;
        }

        static Rect GetRect(JToken mToken)
        {
            Rect result = new Rect();
            foreach (var tok2 in mToken.Children().Children())
            {
                switch (((JProperty) tok2).Name)
                {
                    case "qLeft":
                    {
                        result.Left = Convert.ToInt32(tok2.First.ToString());
                        break;
                    }
                    case "qTop":
                    {
                        result.Top = Convert.ToInt32(tok2.First.ToString());
                        break;
                    }
                    case "qWidth":
                    {
                        result.Width = Convert.ToInt32(tok2.First.ToString());
                        break;
                    }
                    case "qHeight":
                    {
                        result.Height = Convert.ToInt32(tok2.First.ToString());
                        break;
                    }
                }
            }

            return result;
        }

        // ReSharper disable once UnusedParameter.Local
        static IEnumerable<NxDataPage> CreateNxDataPages(JToken mToken)
        {
            IEnumerable<NxCellRows> getMatrix(JToken mTok)
            {
                IList<NxCellRows> list = new List<NxCellRows>();
                foreach (var tok0 in mTok.Children())
                {
                    foreach (var tok in tok0.Children())
                    {
                        NxCellRows cellRows = new NxCellRows();
                        foreach (var tok2 in tok.Children())
                        {
                            NxCell cell = GetCell(tok2);
                            cellRows.Add(cell);
                        }

                        list.Add(cellRows);
                    }
                }

                return list;
            }

            IList<NxDataPage> resList = new List<NxDataPage>();
            foreach (var tok in mToken.Children().Children())
            {
                NxDataPage dataPage = new NxDataPage();
                foreach (var tok2 in tok.Children())
                {
                    switch (((JProperty) tok2).Name)
                    {
                        case "qMatrix":
                        {
                            dataPage.Matrix = getMatrix(tok2);
                            break;
                        }
                        case "qTails":
                        {
                            //fa.nDec = Convert.ToInt32(tok.First.ToString());
                            break;
                        }
                        case "qArea":
                        {
                            dataPage.Area = GetRect(tok2);
                            break;
                        }
                        case "qIsReduced":
                        {
                            //fa.Fmt = tok.First.ToString();
                            break;
                        }
                    }
                }

                resList.Add(dataPage);
            }

            return resList;
        }

        static NxSortIndicatorType GetSortType(string sortMode)
        {
            switch (sortMode)
            {
                case "A":
                {
                    return NxSortIndicatorType.NX_SORT_INDICATE_ASC;
                }
                case "D":
                {
                    return NxSortIndicatorType.NX_SORT_INDICATE_DESC;
                }
                case "N":
                {
                    return NxSortIndicatorType.NX_SORT_INDICATE_NONE;
                }
            }

            return NxSortIndicatorType.NX_SORT_INDICATE_ASC;
        }

        static FieldAttributes GetNumFormat(JToken rToken)
        {
            FieldAttrType getFieldAttrType(string fat)
            {
                switch (fat)
                {
                    case "U":
                    {
                        return FieldAttrType.UNKNOWN;
                    }
                    case "A":
                    {
                        return FieldAttrType.ASCII;
                    }
                    case "R":
                    {
                        return FieldAttrType.REAL;
                    }
                    case "D":
                    {
                        return FieldAttrType.DATE;
                    }
                    case "T":
                    {
                        return FieldAttrType.TIME;
                    }
                    case "TS":
                    {
                        return FieldAttrType.TIMESTAMP;
                    }
                    case "IV":
                    {
                        return FieldAttrType.INTERVAL;
                    }
                    case "I":
                    {
                        return FieldAttrType.INTEGER;
                    }
                    case "F":
                    {
                        return FieldAttrType.FIX;
                    }
                    case "M":
                    {
                        return FieldAttrType.MONEY;
                    }
                }

                return FieldAttrType.UNKNOWN;
            }

            FieldAttributes fa = new FieldAttributes();
            foreach (var tok in rToken.Children().Children())
            {
                switch (((JProperty) tok).Name)
                {
                    case "qType":
                    {
                        fa.Type = getFieldAttrType(tok.First.ToString());
                        break;
                    }
                    case "qnDec":
                    {
                        fa.nDec = Convert.ToInt32(tok.First.ToString());
                        break;
                    }
                    case "qUseThou":
                    {
                        fa.UseThou = Convert.ToInt32(tok.First.ToString());
                        break;
                    }
                    case "qFmt":
                    {
                        fa.Fmt = tok.First.ToString();
                        break;
                    }
                    case "qDec":
                    {
                        fa.Dec = tok.First.ToString();
                        break;
                    }
                    case "qThou":
                    {
                        fa.Thou = tok.First.ToString();
                        break;
                    }
                }
            }

            return fa;
        }

        static IEnumerable<string> GetGroupFallbackTitles(JToken rToken)
        {
            IList<string> res = new List<string>();
            
            foreach (JToken tk in rToken.Children())
            {
                foreach (var tk2 in tk.Children())
                {
                    res.Add(tk2.ToString());
                }
            }

            return res;
        }

        static IEnumerable<NxDimensionInfo> CreateDimensionInfo(JToken mToken)
        {
            IEnumerable<string> GetTags(JToken rToken)
            {
                return GetGroupFallbackTitles(rToken);
            }

            IEnumerable<string> GetGroupFieldsDef(JToken rToken)
            {
                return GetGroupFallbackTitles(rToken);
            }

            NxGrpType getGroupType(string grType)
            {
                switch (grType)
                {
                    case "N":
                    {
                        return NxGrpType.GRP_NX_NONE;
                    }
                    case "H":
                    {
                        return NxGrpType.GRP_NX_HIEARCHY;
                    }
                    case "C":
                    {
                        return NxGrpType.GRP_NX_COLLECTION;
                    }
                }

                return NxGrpType.GRP_NX_NONE;
            }

            NxDimensionType GetDimensionType(string dimType)
            {
                switch (dimType)
                {
                    case "D":
                    {
                        return NxDimensionType.NX_DIMENSION_TYPE_DISCRETE;
                    }
                    case "N":
                    {
                        return NxDimensionType.NX_DIMENSION_TYPE_NUMERIC;
                    }
                    case "T":
                    {
                        return NxDimensionType.NX_DIMENSION_TYPE_TIME;
                    }
                }

                return NxDimensionType.NX_DIMENSION_TYPE_NUMERIC;
            }

            NxCardinalities GetCardinality(JToken rToken)
            {
                NxCardinalities ca = new NxCardinalities();
                foreach (var tok in rToken.Children().Children())
                {
                    switch (((JProperty) tok).Name)
                    {
                        case "qCardinal":
                        {
                            ca.Cardinal = Convert.ToInt32(tok.First.ToString());
                            break;
                        }
                        case "qHypercubeCardinal":
                        {
                            ca.HypercubeCardinal = Convert.ToInt32(tok.First.ToString());
                            break;
                        }
                        case "qAllValuesCardinal":
                        {
                            ca.AllValuesCardinal = Convert.ToInt32(tok.First.ToString());
                            break;
                        }
                    }
                }

                return ca;
            }

            NxStateCounts getNxStateCounts(JToken rToken)
            {
                NxStateCounts state = new NxStateCounts();
                foreach (var tok in rToken.Children().Children())
                {
                    switch (((JProperty) tok).Name)
                    {
                        case "qLocked":
                        {
                            state.Locked = Convert.ToInt32(tok.First.ToString());
                            break;
                        }
                        case "qSelected":
                        {
                            state.Selected = Convert.ToInt32(tok.First.ToString());
                            break;
                        }
                        case "qOption":
                        {
                            state.Option = Convert.ToInt32(tok.First.ToString());
                            break;
                        }
                        case "qDeselected":
                        {
                            state.Deselected = Convert.ToInt32(tok.First.ToString());
                            break;
                        }
                        case "qAlternative":
                        {
                            state.Alternative = Convert.ToInt32(tok.First.ToString());
                            break;
                        }
                        case "qExcluded":
                        {
                            state.Excluded = Convert.ToInt32(tok.First.ToString());
                            break;
                        }
                        case "qSelectedExcluded":
                        {
                            state.SelectedExcluded = Convert.ToInt32(tok.First.ToString());
                            break;
                        }
                        case "qLockedExcluded":
                        {
                            state.LockedExcluded = Convert.ToInt32(tok.First.ToString());
                            break;
                        }
                    }
                }

                return state;
            }

            IList<NxDimensionInfo> result = new List<NxDimensionInfo>();
            
            foreach (JToken tok2 in mToken.Children())
            {
                NxDimensionInfo dimensionInfo = new NxDimensionInfo();
                foreach (JToken tok3 in tok2.Children())
                {
                    foreach (JToken tok4 in tok3.Children())
                    {
                        switch (((JProperty) tok4).Name)
                        {
                            case "qFallbackTitle":
                            {
                                dimensionInfo.FallbackTitle = tok4.First.ToString();
                                break;
                            }
                            case "qApprMaxGlyphCount":
                            {
                                dimensionInfo.ApprMaxGlyphCount = Convert.ToInt32(tok4.First.ToString());
                                break;
                            }
                            case "qCardinal":
                            {
                                dimensionInfo.Cardinal = Convert.ToInt32(tok4.First.ToString());
                                break;
                            }
                            case "qLocked":
                            {
                                dimensionInfo.Locked = Convert.ToBoolean(tok4.First.ToString());
                                break;
                            }
                            case "qSortIndicator":
                            {
                                dimensionInfo.SortIndicator = GetSortType(tok4.First.ToString());
                                break;
                            }
                            case "qGroupFallbackTitles":
                            {
                                dimensionInfo.GroupFallbackTitles = GetGroupFallbackTitles(tok4);
                                break;
                            }
                            case "qGroupPos":
                            {
                                dimensionInfo.GroupPos = Convert.ToInt32(tok4.First.ToString());
                                break;
                            }
                            case "qStateCounts":
                            {
                                dimensionInfo.StateCounts = getNxStateCounts(tok4);
                                break;
                            }
                            case "qTags":
                            {
                                dimensionInfo.Tags = GetTags(tok4);
                                break;
                            }
                            case "qDimensionType":
                            {
                                dimensionInfo.DimensionType = GetDimensionType(tok4.First.ToString());
                                break;
                            }
                            case "qGrouping":
                            {
                                dimensionInfo.Grouping = getGroupType(tok4.First.ToString());
                                break;
                            }
                            case "qNumFormat":
                            {
                                dimensionInfo.NumFormat = GetNumFormat(tok4);
                                break;
                            }
                            case "qIsAutoFormat":
                            {
                                dimensionInfo.IsAutoFormat = Convert.ToBoolean(tok4.First.ToString());
                                break;
                            }
                            case "qGroupFieldDefs":
                            {
                                dimensionInfo.GroupFieldDefs = GetGroupFieldsDef(tok4);
                                break;
                            }
                            case "qMin":
                            {
                                dimensionInfo.Min = Convert.ToDouble(tok4.First.ToString());
                                break;
                            }
                            case "qMax":
                            {
                                dimensionInfo.Max = Convert.ToDouble(tok4.First.ToString());
                                break;
                            }
                            case "qContinuousAxes":
                            {
                                dimensionInfo.ContinuousAxes = Convert.ToBoolean(tok4.First.ToString());
                                break;
                            }
                            case "qCardinalities":
                            {
                                dimensionInfo.Cardinalities = GetCardinality(tok4);
                                break;
                            }
                        }

                        
                    }
                    
                }
                result.Add(dimensionInfo);
            }

            return result;
        }

       

        // ReSharper disable once UnusedParameter.Local
        static NxHypercubeMode CreateNxHypercubeMode(JToken mToken)
        {
            return NxHypercubeMode.DATA_MODE_STRAIGHT;
        }

        

        public static HyperCube ReadCube(String file)
        {
            string mString = Utils.ReadJsonFile(file);
            JObject jObject = JObject.Parse(mString);
            HyperCube cube = new HyperCube();
            foreach (JToken token in jObject.Children())
            {
                switch (token.Path)
                {
                    case "qSize":
                    {
                        cube.Size = CreateSize(token);
                        break;
                    }
                    case "qDimensionInfo":
                    {
                        cube.DimensionInfo = CreateDimensionInfo(token);
                        break;
                    }
                    case "qMeasureInfo":
                    {
                        cube.MeasureInfo = CreateMeasureInfos(token);
                        break;
                    }
                    case "qEffectiveInterColumnSortOrder":
                    {
                        cube.EffectiveInterColumnSortOrder = CreateSotOrder(token);
                        break;
                    }
                    case "qGrandTotalRow":
                    {
                        cube.GrandTotalRow = CreateGrandTotalRow(token);
                        break;
                    }
                    case "qDataPages":
                    {
                        cube.DataPages = CreateNxDataPages(token);
                        break;
                    }


                    case "qMode":
                    {
                        cube.Mode = CreateNxHypercubeMode(token);
                        break;
                    }
                    case "qNoOfLeftDims":
                    {
                        cube.NoOfLeftDims = Convert.ToInt32(token.First.ToString());
                        break;
                    }

                    #region Пока закомментировано

                    //case "qIndentMode":
                    //{
                    //    qIndentMode = getIdentMode(token);
                    //    break;
                    //}
                    //case "qLastExpandedPos":
                    //{
                    //    cellPosition = getcellPosition(token);
                    //    break;
                    //}
                    //case "qHasOtherValues":
                    //{
                    //    qHasOtherValues = hasOtherValues(token);
                    //    break;
                    //}
                    //case "qTitle":
                    //{
                    //    qTitle = getTitle(token);
                    //    break;
                    //}
                    //case "qTreeNodesOnDim":
                    //{
                    //    qTreeNodesOnDim = getTreeNodesOnDim(token);
                    //    break;
                    //}
                    //case "qCalcCondMsg":
                    //{
                    //    qCalcCondMsg = getMessage(token);
                    //    break;
                    //}
                    //case "qColumnOrder":
                    //{
                    //    columnOrder = getColumnOrder(token);
                    //    break;
                    //}

                    #endregion
                }
            }

            return cube;
        }
    }
}
