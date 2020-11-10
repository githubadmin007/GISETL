using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSSG.EsriGIS
{
    public class LicenseHelper
    {
        public static void CheckOutLicense()
        {
            if (!RuntimeManager.Bind(ProductCode.EngineOrDesktop))
            {
                throw new Exception("不能绑定ArcGIS runtime，应用程序即将关闭.");
            }
            IAoInitialize aoInit = new AoInitialize();
            try
            {
                aoInit.Initialize(esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
