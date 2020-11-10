using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSSG.EsriGIS.Geodatabase
{
    public class XSpatialReference
    {

        /// <summary>
        /// 位置坐标系
        /// </summary>
        static ISpatialReference _Unknown = null;
        static public ISpatialReference Unknown
        {
            get
            {
                if (_Unknown == null)
                {
                    ISpatialReference sr = new UnknownCoordinateSystemClass();
                    sr.SetDomain(0, 99999999, 0, 99999999);//设置空间范围
                    sr.SetZDomain(-999999, 999999);
                    _Unknown = sr;
                }
                return _Unknown;
            }
        }
        /// <summary>
        /// 创建投影坐标系
        /// </summary>
        /// <param name="pcsType"></param>
        /// <returns></returns>
        static public ISpatialReference CreateProjectedCoordinateSystem(int pcsType)
        {
            SpatialReferenceEnvironmentClass sfe = new SpatialReferenceEnvironmentClass();
            IProjectedCoordinateSystem pcs = sfe.CreateProjectedCoordinateSystem(pcsType);
            return pcs;
        }
        /// <summary>
        /// 创建投影坐标系
        /// </summary>
        /// <param name="pcsType"></param>
        /// <returns></returns>
        static public ISpatialReference CreateProjectedCoordinateSystem(esriSRProjCS4Type pcsType)
        {
            return CreateProjectedCoordinateSystem((int)pcsType);
        }
        /// <summary>
        /// 创建地理坐标系
        /// </summary>
        /// <param name="gcsType"></param>
        /// <returns></returns>
        static public ISpatialReference CreateGeographicCoordinateSystem(int gcsType)
        {
            SpatialReferenceEnvironmentClass sfe = new SpatialReferenceEnvironmentClass();
            IGeographicCoordinateSystem gcs = sfe.CreateGeographicCoordinateSystem(gcsType);
            return gcs;
        }
        /// <summary>
        /// 创建地理坐标系
        /// </summary>
        /// <param name="gcsType"></param>
        /// <returns></returns>
        static public ISpatialReference CreateGeographicCoordinateSystem(esriSRGeoCSType gcsType)
        {
            return CreateGeographicCoordinateSystem((int)gcsType);
        }
    }
}
