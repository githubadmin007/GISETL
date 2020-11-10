using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using FSSG.EsriGIS.Geodatabase;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSSG.EsriGIS.Extend
{
    public static class IFeatureBufferEx
    {
        /// <summary>
        /// 通过json更新图形
        /// </summary>
        /// <param name="geometryJson"></param>
        public static void SetShapeByJson(this IFeatureBuffer buffer, JObject geometry) {
            try
            {
                IGeometry geo = XGeometry.Parse(geometry.ToString());
                buffer.Shape = geo;
            }
            catch (Exception e)
            {
                buffer.Shape = null;
            }
        }
    }
}
