using ESRI.ArcGIS.Geodatabase;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSSG.EsriGIS.Extend
{
    public static class IRowBufferEx
    {
        /// <summary>
        /// 通过json设置属性
        /// </summary>
        /// <param name="attributes"></param>
        public static void SetAttrsByJson(this IRowBuffer buffer, JObject attrs)
        {
            for (int i = 0; i < buffer.Fields.FieldCount; i++)
            {
                if (buffer.Fields.Field[i].Editable)
                {
                    string name = buffer.Fields.Field[i].Name;
                    if (name == "SHAPE" || name == "SHAPE_Area" || name == "SHAPE_Length" || name == "OBJECTID" || name == "GLOBALID" || name == "FID") continue;
                    buffer._SetAttrByJson(i, attrs.GetValue(name));
                }
            }
        }
        /// <summary>
        /// 通过json设置属性
        /// </summary>
        /// <param name="attributes"></param>
        public static void _SetAttrByJson(this IRowBuffer buffer, int index, JToken token) {
            object value = buffer.Fields.Field[index].Convert(token);
            buffer.SetValue(index, value);
        }
        /// <summary>
        /// 通过json设置属性
        /// </summary>
        /// <param name="attributes"></param>
        public static void SetAttrByJson(this IRowBuffer buffer, string name, JToken token)
        {
            int index = buffer.Fields.FindField(name);
            buffer._SetAttrByJson(index, token);
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public static void SetValue(this IRowBuffer buffer, int index, object value)
        {
            if (index > -1 && index < buffer.Fields.FieldCount)
            {
                buffer.Value[index] = value;
            }
            else {
                throw new Exception("IRowBuffer.SetValue索引越界");
            }
        }
        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SetValue(this IRowBuffer buffer, string name, object value)
        {
            int index = buffer.Fields.FindField(name);
            buffer.SetValue(index, value);
        }
        /// <summary>
        /// 传入Json设置属性值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SetValues(this IRowBuffer buffer, string json)
        {
            if (!string.IsNullOrWhiteSpace(json))
            {
                JObject attrs = JObject.Parse(json);
                foreach (var pair in attrs)
                {
                    buffer.SetValue(pair.Key, pair.Value);
                }
            }
        }
        /// <summary>
        /// 传入另一个对象设置属性值
        /// </summary>
        /// <param name="zRowBuffer"></param>
        public static void _SetValues(this IRowBuffer buffer, IRowBuffer otherBuffer)
        {
            for (int i = 0; i < buffer.Fields.FieldCount; i++)
            {
                if (buffer.Fields.Field[i].Editable)
                {
                    string name = buffer.Fields.Field[i].Name;
                    object obj = otherBuffer.GetValue(name);
                    buffer.SetValue(name, obj);
                }
            }
        }
        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static object GetValue(this IRowBuffer buffer, int index)
        {
            if (index > -1 && index < buffer.Fields.FieldCount)
            {
                string xx = buffer.Value[index].ToString();
                return buffer.Value[index];
            }
            else {
                throw new Exception("IRowBuffer索引越界");
            }
        }
        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="name">字段名</param>
        /// <returns></returns>
        public static object GetValue(this IRowBuffer buffer, string name)
        {
            int index = buffer.Fields.FindField(name);
            return buffer.GetValue(index);
        }
        /// <summary>
        /// 获取多个属性值
        /// </summary>
        /// <param name="nameArr">字段名数组</param>
        /// <returns></returns>
        public static object[] GetValues(this IRowBuffer buffer, string[] nameArr)
        {
            return nameArr.Select(n => buffer.GetValue(n)).ToArray();
        }
    }
}
