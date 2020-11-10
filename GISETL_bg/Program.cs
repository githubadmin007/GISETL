using FSSG.EsriGIS;
using FSSG.EsriGIS.Geodatabase;
using GISETL_bg.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZJH.BaseTools;

namespace GISETL_bg
{
    class Program
    {
        static void Main(string[] args)
        {
            LicenseHelper.CheckOutLicense();
            CheckAndCreateFolder();
            TaskMonitor monitor = new TaskMonitor();
            //monitor.Start();
            monitor.CheckImmediately();
            //20200826展示注释
            while (true)
            {
                ConsoleKeyInfo info = Console.ReadKey();
                if (info.Key == ConsoleKey.Escape)
                {
                    return;
                }
            };
        }

        static void CheckAndCreateFolder()
        {
            // GIS数据文件夹
            string GisDataFolder = GlobalConfig.AppCfg.getValueByPath("configuration/Custom/GisDataFolder");
            System.IO.Directory.CreateDirectory(GisDataFolder);
            // 临时文件夹
            string TempDataFolder = GlobalConfig.AppCfg.getValueByPath("configuration/Custom/TempDataFolder");
            System.IO.Directory.CreateDirectory(TempDataFolder);
            // 临时GDB
            string TempGDB = GlobalConfig.AppCfg.getValueByPath("configuration/Custom/TempGDB");
            XWorkspace.CreateGDB(TempGDB, open: false);
        }
    }
}
