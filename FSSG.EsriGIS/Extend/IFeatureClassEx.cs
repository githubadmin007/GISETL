using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using FSSG.EsriGIS.Geodatabase;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FSSG.EsriGIS.Extend
{
    public static class IFeatureClassEx
    {
        public static void Dispose(this IFeatureClass featureClass) {
            if (featureClass != null)
            {
                Marshal.ReleaseComObject(featureClass);
            }
        }


        /// <summary>
        /// 获取所有的字段名
        /// </summary>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        public static List<string> GetAllFieldName(this IFeatureClass featureClass) {
            List<string> lst = new List<string>();
            IFields fields = featureClass.Fields;
            for (int i = 0, l = fields.FieldCount; i < l; i++)
            {
                lst.Add(fields.Field[i].Name);
            }
            return lst;
        }
        /// <summary>
        /// 获取所有的字段别名
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllFieldAliasName(this IFeatureClass featureClass)
        {
            List<string> lst = new List<string>();
            IFields fields = featureClass.Fields;
            for (int i = 0; i < fields.FieldCount; i++)
            {
                string name = fields.Field[i].AliasName;
                lst.Add(string.IsNullOrWhiteSpace(name) ? fields.Field[i].Name : name);
            }
            return lst;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="name">字段名</param>
        /// <param name="alias">字段别名</param>
        /// <param name="type">字段类型</param>
        /// <param name="length">长度</param>
        /// <param name="precision">精度</param>
        /// <param name="nullable">是否可为空</param>
        /// <param name="editable">是否可编辑</param>
        /// <param name="defaultvalue">默认值</param>
        public static void AddField(this IFeatureClass featureClass, string name, string alias, esriFieldType type, int length = 0, int precision = 0, bool nullable = true, bool editable = true, object defaultvalue = null) {
            featureClass.Fields.AddField(name, alias, type, length, precision, nullable, editable, defaultvalue);
        }
        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="newName"></param>
        public static void ReName(this IFeatureClass featureClass, string newName)
        {
            IDataset ds = featureClass as IDataset;
            if (ds.CanRename())
            {
                ds.Rename(newName);
            }
        }



        /// <summary>
        /// 是否存在记录
        /// </summary>
        /// <param name="where"></param>
        /// <param name="zGeo"></param>
        /// <param name="rel"></param>
        /// <returns></returns>
        public static bool HasRecord(this IFeatureClass featureClass, string where = "1=1", IGeometry geo = null, esriSpatialRelEnum rel = esriSpatialRelEnum.esriSpatialRelIntersects)
        {
            IQueryFilter filter = XFilter.CreateFilter(where, geo, featureClass.ShapeFieldName, rel);
            return featureClass.HasRecord(filter);
        }
        /// <summary>
        /// 是否存在记录
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static bool HasRecord(this IFeatureClass featureClass, IQueryFilter filter)
        {
            int n = featureClass.FeatureCount(filter);
            return n > 0;
        }
        /// <summary>
        /// The number of features selected by the specified query.
        /// </summary>
        /// <param name="where"></param>
        /// <param name="zGeo"></param>
        /// <param name="rel"></param>
        /// <returns></returns>
        public static int FeatureCount(this IFeatureClass featureClass, string where = "1=1", IGeometry geo = null, esriSpatialRelEnum rel = esriSpatialRelEnum.esriSpatialRelIntersects)
        {
            IQueryFilter filter = XFilter.CreateFilter(where, geo, featureClass.ShapeFieldName, rel);
            return featureClass.FeatureCount(filter);
        }

        /// <summary>
        /// 空间属性查询
        /// </summary>
        /// <param name="where"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static IFeatureCursor Search(this IFeatureClass featureClass, string where = "1=1", IGeometry geo = null, esriSpatialRelEnum rel = esriSpatialRelEnum.esriSpatialRelIntersects, bool Recycling = false)
        {
            IQueryFilter filter = XFilter.CreateFilter(where, geo, featureClass.ShapeFieldName, rel);
            return featureClass.Search(filter, Recycling);
        }
        /// <summary>
        /// Returns a cursor that can be used to update features selected by the specified
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="useBuffering"></param>
        /// <returns></returns>
        public static IFeatureCursor Update(this IFeatureClass featureClass, string where = "1=1", IGeometry geo = null, esriSpatialRelEnum rel = esriSpatialRelEnum.esriSpatialRelIntersects, bool useBuffering = true)
        {
            IQueryFilter filter = XFilter.CreateFilter(where, geo, featureClass.ShapeFieldName, rel);
            return featureClass.Update(filter, useBuffering);
        }
        /// <summary>
        /// Delete the Rows in the database selected by the specified query.
        /// </summary>
        /// <param name="where"></param>
        /// <param name="zGeo"></param>
        /// <param name="rel"></param>
        public static bool Delete(this IFeatureClass featureClass, string where = "1=1", IGeometry geo = null, esriSpatialRelEnum rel = esriSpatialRelEnum.esriSpatialRelIntersects)
        {
            IQueryFilter filter = XFilter.CreateFilter(where, geo, featureClass.ShapeFieldName, rel);
            return featureClass.Delete(filter);
        }
        /// <summary>
        /// Delete the Rows in the database selected by the specified query.
        /// </summary>
        /// <param name="filter"></param>
        public static bool Delete(this IFeatureClass featureClass, IQueryFilter filter)
        {
            try
            {
                ITable table = featureClass as ITable;
                table.DeleteSearchedRows(filter);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 复制一条记录并插入
        /// </summary>
        /// <param name="zFeature"></param>
        public static void CopyAndInsert(this IFeatureClass featureClass, IFeature feature)
        {
            IFeatureCursor cursor = featureClass.Insert(false);
            IFeature newFeature = featureClass.CreateFeature();
            newFeature.Shape = feature.ShapeCopy;
            int n = newFeature.Fields.FieldCount;
            for (int i = 0; i < n; i++)
            {
                IField field = newFeature.Fields.Field[i];
                if (field.Editable)
                {
                    newFeature.Value[i] = feature.GetValue(i);
                }
            }
            newFeature.Store();
        }
        /// <summary>
        /// 从查询结果复制数据(数据结构相同)
        /// </summary>
        /// <param name="cursor"></param>
        public static void CopyFrom(this IFeatureClass featureClass, IFeatureCursor otherCursor)
        {
            IFeatureCursor thisCursor = featureClass.Insert(true);
            IFeature otherFeature = null;
            while (null != (otherFeature = otherCursor.NextFeature()))
            {
                IFeatureBuffer thisBuffer = featureClass.CreateFeatureBuffer();
                // thisBuffer.Shape = otherFeature.Shape;
                for (int i = 0; i < featureClass.Fields.FieldCount; i++)
                {
                    IField thisField = featureClass.Fields.get_Field(i);
                    if (thisField.Editable)
                    {
                        int otherFieldIndex = otherCursor.Fields.FindField(thisField.Name);
                        if (otherFieldIndex > -1) {
                            thisBuffer.Value[i] = otherFeature.GetValue(otherFieldIndex);
                        }
                    }
                }
                thisCursor.InsertFeature(thisBuffer);
            }
            thisCursor.Flush();
        }
        /// <summary>
        /// 从另一张表复制数据(数据结构相同)
        /// </summary>
        /// <param name="ZFClass"></param>
        /// <param name="clear">是否清空原数据</param>
        public static void CopyFrom(this IFeatureClass featureClass, IFeatureClass otherFeatureClass, bool clear = false)
        {
            if (clear)
            {
                featureClass.Delete();
            }
            //MergeStructFrom
            IFeatureCursor cursor = otherFeatureClass.Search();
            featureClass.CopyFrom(cursor);
        }
        /// <summary>
        /// 复制数据到另一张表
        /// </summary>
        /// <param name="outZWorkspace">目标工作空间</param>
        /// <param name="name">目标图层名</param>
        /// <param name="filter">查询条件</param>
        public static void CopyTo(this IFeatureClass featureClass, IWorkspace outWorkspace, string name, IQueryFilter filter)
        {
            // 数据源
            IFeatureClassName inputFClassName = ((IDataset)featureClass).FullName as IFeatureClassName;
            // 目标数据集
            IDataset outDataSet = outWorkspace as IDataset;
            IWorkspaceName outWorkspaceName = outDataSet.FullName as IWorkspaceName;
            // 目标图层
            IFeatureClassName outputFClassName = new FeatureClassNameClass();
            IDatasetName dataSetName = outputFClassName as IDatasetName;
            dataSetName.WorkspaceName = outWorkspaceName;
            dataSetName.Name = name;
            // 导出
            FeatureDataConverterClass converter = new FeatureDataConverterClass();
            converter.ConvertFeatureClass(inputFClassName, filter, null, outputFClassName, null, featureClass.Fields, "", 100, 0);
            // 释放资源
            Marshal.ReleaseComObject(converter);
        }
        /// <summary>
        /// 插入从MapServer获取到的JSON数据
        /// </summary>
        /// <param name="features"></param>
        /// <returns></returns>
        public static int InsertFeaturesByJson(this IFeatureClass featureClass, JArray features)
        {
            int featureNum = 0;
            try
            {
                IFeatureCursor cursor = featureClass.Insert(true);
                foreach (JObject feature in features)
                {
                    try
                    {
                        IFeatureBuffer buffer = featureClass.CreateFeatureBuffer();
                        buffer.SetShapeByJson(feature.Value<JObject>("geometry"));
                        buffer.SetAttrsByJson(feature.Value<JObject>("attributes"));
                        cursor.InsertFeature(buffer);
                    }
                    catch (Exception ex)
                    {
                        // Logger.log("ZFeatureClass.InsertFeaturesByJson", ex);
                    }
                    featureNum++;
                }
                cursor.Flush();
            }
            catch (Exception ex)
            {
                throw new Exception($"将JSON数据插入图层【{featureClass.AliasName}】时报错", ex); 
            }
            return featureNum;
        }
    }
}
