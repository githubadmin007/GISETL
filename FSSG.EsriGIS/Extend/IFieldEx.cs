using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZJH.BaseTools.BasicExtend;

namespace FSSG.EsriGIS.Extend
{
    public static class IFieldEx
    {
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public static IField Clone(this IField field)
        {
            if (field.Type == esriFieldType.esriFieldTypeGeometry)
            {
                IGeometryDef pGeoDefEdit = field.GeometryDef;
                return CreateShapeField(pGeoDefEdit.GeometryType, pGeoDefEdit.SpatialReference, field.Name);
            }
            else {
                return CreateField(field.Name, field.AliasName, field.Type, field.Length, field.Precision, field.IsNullable, field.Editable, field.DefaultValue);
            }
        }
        /// <summary>
        /// 将JToken转为适合该字段的格式
        /// </summary>
        /// <param name="field"></param>
        /// <param name="valueObj"></param>
        /// <returns></returns>
        public static object Convert(this IField field, JToken valueObj) {
            if (valueObj == null || valueObj.ToString() == "") return null;
            switch (field.Type)
            {
                case esriFieldType.esriFieldTypeDate:
                    DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
                    return startTime.AddMilliseconds(valueObj.ToInt64());
                case esriFieldType.esriFieldTypeDouble:
                    return valueObj.ToDouble();
                case esriFieldType.esriFieldTypeInteger:
                    return valueObj.ToInt32();
                case esriFieldType.esriFieldTypeOID:
                    return valueObj.ToInt32();
                case esriFieldType.esriFieldTypeSingle:
                    return valueObj.ToInt16();
                case esriFieldType.esriFieldTypeSmallInteger:
                    return valueObj.ToInt16();
                case esriFieldType.esriFieldTypeString:
                    string str = valueObj.ToString();//.Replace("\n     ", "").Replace("\n    ", "").Replace("\n ", "");
                    if (str.Length > field.Length)
                    {
                        str = str.Substring(0, field.Length);
                    }
                    return str;
                case esriFieldType.esriFieldTypeBlob:
                case esriFieldType.esriFieldTypeGeometry:
                case esriFieldType.esriFieldTypeGlobalID:
                case esriFieldType.esriFieldTypeGUID:
                case esriFieldType.esriFieldTypeRaster:
                case esriFieldType.esriFieldTypeXML:
                default:
                    return valueObj.ToString();
            }
        }


        /// <summary>
        /// 创建字段
        /// </summary>
        /// <param name="name"></param>
        /// <param name="alias"></param>
        /// <param name="type"></param>
        /// <param name="length"></param>
        /// <param name="precision"></param>
        /// <param name="nullable"></param>
        /// <param name="editable"></param>
        /// <param name="dValue"></param>
        /// <returns></returns>
        public static IField CreateField(string name, string alias, esriFieldType type, int length = 0, int precision = 0, bool nullable = true, bool editable = true, object dValue = null)
        {
            IField field = new Field();
            IFieldEdit fieldEdit = (IFieldEdit)field;
            fieldEdit.Name_2 = name;
            fieldEdit.AliasName_2 = alias;
            fieldEdit.Type_2 = type;
            fieldEdit.IsNullable_2 = nullable;
            fieldEdit.Editable_2 = editable;
            if (length > 0) fieldEdit.Length_2 = length;
            if (precision > 0) fieldEdit.Precision_2 = precision;
            if (dValue != null) fieldEdit.DefaultValue_2 = dValue;
            return field;
        }
        /// <summary>
        /// 创建图形字段
        /// </summary>
        /// <param name="geoType"></param>
        /// <param name="sr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IField CreateShapeField(esriGeometryType geoType, ISpatialReference sr = null, string name = "SHAPE")
        {
            IField field = new FieldClass();
            IFieldEdit pFieldEdit = (IFieldEdit)field;
            pFieldEdit.Name_2 = name;
            pFieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
            //设置图形类型
            IGeometryDefEdit pGeoDef = new GeometryDefClass();
            IGeometryDefEdit pGeoDefEdit = pGeoDef as IGeometryDefEdit;
            pGeoDefEdit.GeometryType_2 = geoType;
            if (sr != null)
            {
                pGeoDefEdit.SpatialReference_2 = sr;
            }
            pFieldEdit.GeometryDef_2 = pGeoDef;
            return field;
        }
    }
}
