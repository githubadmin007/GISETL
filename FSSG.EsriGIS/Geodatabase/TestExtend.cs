using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FSSG.EsriGIS.Geodatabase
{
    public static class TestExtend
    {
        /// <summary>
        /// 打开FeatureClass
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IFeatureClass _OpenFeatureClass(this IFeatureWorkspace workspace, string name) {
            try
            {
                IFeatureWorkspace space = workspace as IFeatureWorkspace;
                IFeatureClass fc = space.OpenFeatureClass(name);
                return fc;
            }
            catch (Exception ex)
            {
                return null;
                //throw new Exception($"不存在此FeatureClass:“{name}”", ex);
            }
        }


    }
}
