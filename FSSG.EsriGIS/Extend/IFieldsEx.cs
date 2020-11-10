using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using FSSG.EsriGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSSG.EsriGIS.Extend
{
    public static class IFieldsEx
    {
        /// <summary>
        /// 添加字段
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="field"></param>
        public static void AddField(this IFields fields, IField field)
        {
            IFieldsEdit fieldsEdit = (IFieldsEdit)fields;
            fieldsEdit.AddField(field);
        }
        /// <summary>
        /// 添加字段
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="name">字段名</param>
        /// <param name="alias">字段别名</param>
        /// <param name="type">字段类型</param>
        /// <param name="length">长度</param>
        /// <param name="precision">精度</param>
        /// <param name="nullable">是否可为空</param>
        /// <param name="editable">是否可编辑</param>
        /// <param name="defaultvalue">默认值</param>
        public static void AddField(this IFields fields, string name, string alias, esriFieldType type, int length = 0, int precision = 0, bool nullable = true, bool editable = true, object dValue = null)
        {
            IField field = IFieldEx.CreateField(name, alias, type, length, precision, nullable, editable, dValue);
            fields.AddField(field);
        }
        /// <summary>
        /// 添加图形字段
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="geoType"></param>
        /// <param name="name"></param>
        public static void AddShapeField(this IFields fields, esriGeometryType geoType, string name = "SHAPE")
        {
            fields.AddShapeField(geoType, XSpatialReference.Unknown, name);
        }
        /// <summary>
        /// 添加图形字段
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="geoType"></param>
        /// <param name="sr"></param>
        /// <param name="name"></param>
        public static void AddShapeField(this IFields fields, esriGeometryType geoType, ISpatialReference sr, string name = "SHAPE")
        {
            IField field  = IFieldEx.CreateShapeField(geoType, sr, name);
            fields.AddField(field);
        }
        /// <summary>
        /// 克隆
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IFields Clone(this IFields fields, bool filter = true) {
            IFields newFields = new FieldsClass();
            for (int i = 0, l = fields.FieldCount; i < l; i++) {
                IField field = fields.Field[i].Clone();
                newFields.AddField(field);
            }
            return newFields;
        }
    }
}
