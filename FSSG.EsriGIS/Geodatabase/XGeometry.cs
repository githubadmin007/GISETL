using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSSG.EsriGIS.Geodatabase
{
    public class XGeometry
    {
        /// <summary>
        /// //根据json字符串判断图形类型
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static esriGeometryType GetGeometryType(string json)
        {
            //字符串若包含“rings”，若包含则返回面
            if (json.IndexOf("rings") > -1)
                return esriGeometryType.esriGeometryPolygon;
            //字符串若包含“paths”，若包含则返回线
            if (json.IndexOf("paths") > -1)
                return esriGeometryType.esriGeometryPolyline;
            //字符串若包含“x”，若包含则返回点
            if (json.IndexOf("x") > -1)
                return esriGeometryType.esriGeometryPoint;
            //若均不包含，返回未知类型
            return esriGeometryType.esriGeometryNull;
        }
        /// <summary>
        /// 将json字符串转为Geometry对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static IGeometry Parse(string json, bool bHasZ = false, bool bHasM = false)
        {
            IGeometry result;
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }
            else
            {
                esriGeometryType type = GetGeometryType(json);
                IJSONReader jsonReader = new JSONReaderClass();
                jsonReader.ReadFromString(json);
                JSONConverterGeometryClass jsonCon = new JSONConverterGeometryClass();
                result = jsonCon.ReadGeometry(jsonReader, (esriGeometryType)type, bHasZ, bHasM);
                ITopologicalOperator topo = result as ITopologicalOperator;
                topo.Simplify();
            }
            return result;
        }

        /// <summary>
        /// 分割多边形
        /// </summary>
        /// <param name="PolygonJSON"></param>
        /// <param name="PolylineJSON"></param>
        /// <returns></returns>
        public static List<IGeometry> CutGeometry(object PolygonObj, object PolylineObj)
        {
            List<IGeometry> geos = new List<IGeometry>();
            try
            {
                IGeometry polygon, polyline;
                // 处理面
                if (PolygonObj is string)
                {
                    polygon = XGeometry.Parse((string)PolygonObj);
                }
                else if (PolygonObj is IGeometry)
                {
                    polygon = (IGeometry)PolygonObj;
                }
                else {
                    throw new Exception("PolygonObj参数格式错误");
                }
                // 处理线
                if (PolylineObj is string)
                {
                    polyline = XGeometry.Parse((string)PolylineObj);
                }
                else if (PolylineObj is IGeometry)
                {
                    polyline = (IGeometry)PolylineObj;
                }
                else {
                    throw new Exception("PolylineObj参数格式错误");
                }
                geos = CutGeometry(polygon, polyline);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return geos;
        }
        /// <summary>
        /// 分割多边形
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="polyline"></param>
        /// <returns></returns>
        public static List<IGeometry> CutGeometry(IGeometry polygon, IGeometry polyline)
        {
            List<IGeometry> geos = new List<IGeometry>();
            try
            {
                IGeometry pLeftGeo, pRightGeo;
                ITopologicalOperator pTopologBoundary = polygon as ITopologicalOperator;
                pTopologBoundary.Simplify();
                pTopologBoundary.Cut((IPolyline)polyline, out pLeftGeo, out pRightGeo);
                if (pLeftGeo != null && pRightGeo != null)
                {
                    geos.Add(pLeftGeo);
                    geos.Add(pRightGeo);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return geos;
        }

        /// <summary>  
        /// IGeometry转成JSON字符串  
        /// </summary>  
        public static string GeometryToJsonString(IGeometry geometry)
        {
            IJSONWriter jsonWriter = new JSONWriterClass();
            jsonWriter.WriteToString();
            JSONConverterGeometryClass jsonCon = new JSONConverterGeometryClass();
            jsonCon.WriteGeometry(jsonWriter, null, geometry, false);
            return Encoding.UTF8.GetString(jsonWriter.GetStringBuffer());
        }
    }
}
