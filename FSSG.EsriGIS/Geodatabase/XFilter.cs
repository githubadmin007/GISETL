using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSSG.EsriGIS.Geodatabase
{
    public class XFilter
    {
        static public IQueryFilter AllRecords = CreateQueryFilter("1=1");
        static public IQueryFilter NoneRecords = CreateQueryFilter("1=2");
        /// <summary>
        /// 创建属性查询
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public static IQueryFilter CreateQueryFilter(string where) {
            QueryFilter filter = new QueryFilter();
            if (!string.IsNullOrWhiteSpace(where)) filter.WhereClause = where;
            return filter;
        }
        /// <summary>
        /// 创建空间查询
        /// </summary>
        /// <param name="where"></param>
        /// <param name="geo"></param>
        /// <param name="fieldName"></param>
        /// <param name="rel"></param>
        /// <returns></returns>
        public static ISpatialFilter CreateSpatialFilter(string where, IGeometry geo = null, string fieldName = "SHAPE", esriSpatialRelEnum rel = esriSpatialRelEnum.esriSpatialRelIntersects)
        {
            ISpatialFilter filter = new SpatialFilter();
            filter.Geometry = geo;
            filter.GeometryField = fieldName;
            filter.SpatialRel = (esriSpatialRelEnum)rel;
            if (!string.IsNullOrWhiteSpace(where)) filter.WhereClause = where;
            return filter;
        }
        /// <summary>
        /// 创建查询
        /// </summary>
        /// <param name="where"></param>
        /// <param name="geo"></param>
        /// <param name="fieldName"></param>
        /// <param name="rel"></param>
        /// <returns></returns>
        public static IQueryFilter CreateFilter(string where, IGeometry geo = null, string fieldName = "SHAPE", esriSpatialRelEnum rel = esriSpatialRelEnum.esriSpatialRelIntersects) {
            if (geo == null)
            {
                return CreateQueryFilter(where);
            }
            else {
                return CreateSpatialFilter(where, geo, fieldName, rel);
            }
        }
    }
}
